using System;
using ConsultingKoiFish.BLL.DTOs.EmailDTOs;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace ConsultingKoiFish.BLL.Helpers.Validations.PurchasePackagedValidation;

public class PurchasedPackageExpirationChecker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PurchasedPackageExpirationChecker> _logger;

    public PurchasedPackageExpirationChecker(IServiceScopeFactory scopeFactory, ILogger<PurchasedPackageExpirationChecker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckForExpiringPackagesAsync();
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Chạy hàng ngày
        }
    }

    private async Task CheckForExpiringPackagesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        try
        {
            var purchasedPackageRepo = unitOfWork.GetRepo<PurchasedPackage>();
            var soonToExpirePackages = await purchasedPackageRepo.GetAllAsync(new QueryBuilder<PurchasedPackage>()
                                                                            .WithPredicate(up => up.Status == (int)PurchasedPackageStatus.Available
                                                                                            && up.ExpireDate <= DateTime.Now.AddDays(3))
                                                                            .WithInclude(x => x.User)
                                                                            .WithTracking(false)
                                                                            .Build());

            foreach (var package in soonToExpirePackages)
            {
                var user = package.User;
                var packageName = package.Name;
                var expirationDate = package.ExpireDate.ToString("dd/MM/yyyy");

                var message = new EmailDTO
                    (
                        new string[] { user.Email! },
                        "Thông báo hết hạn gói quảng cáo",
                        $@"
<p>- Hệ thống nhận thấy gói quảng cáo <b>{packageName}</b> của bạn sắp hết hạn trong <b>3 ngày tới</b>.</p>
<p>- Hãy tiếp tục mua lại gói này hoặc trải nghiệm những gói khác để tận hưởng những tính năng mới nhé!!!!</p>
<p>- Chúng tôi xin chân thành cảm ơn.</p>"
                    );
                emailService.SendEmail(message);

                _logger.LogInformation($"Gửi email cảnh báo sắp hết hạn cho người dùng {user.Email} về gói {packageName}");

                if (package.ExpireDate <= DateTime.Now)
                {
                    await unitOfWork.BeginTransactionAsync();
                    package.Status = (int)PurchasedPackageStatus.Unavailable;
                    await purchasedPackageRepo.UpdateAsync(package);
                    var saver = await unitOfWork.SaveAsync();
                    await unitOfWork.CommitTransactionAsync();
                    if (!saver) await unitOfWork.RollBackAsync();
                    var expriedMessage = new EmailDTO
                            (
                                new string[] { user.Email! },
                                "Thông báo hết hạn gói quảng cáo",
                                $@"
<p>- Hệ thống nhận thấy gói quảng cáo <b>{packageName}</b> của bạn sắp hết hạn trong <b>3 ngày tới</b>.</p>
<p>- Hãy tiếp tục mua lại gói này hoặc trải nghiệm những gói khác để tận hưởng những tính năng mới nhé!!!!</p>
<p>- Chúng tôi xin chân thành cảm ơn.</p>"
                            );
                    emailService.SendEmail(message);
                    _logger.LogInformation($"Gửi email hết hạn cho người dùng {user.Email} về gói {packageName}");
                }
            }

        }
        catch (Exception ex)
        {
            await unitOfWork.RollBackAsync();
            _logger.LogError($"Lưu trạng thái của gói thất bại\n {ex.Message}\n {ex.StackTrace}");
        }
    }
}
