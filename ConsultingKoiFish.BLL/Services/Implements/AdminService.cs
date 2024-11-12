using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.AdminDTOs;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConsultingKoiFish.BLL.Services.Implements;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<TotalStasticInYearDTO> GetTotalStatisticInYear()
    {
        var paymentRepo =  _unitOfWork.GetRepo<Payment>();
        var currentYear = DateTime.Now.Year;

        var payments = paymentRepo.Get(new QueryBuilder<Payment>()
            .WithPredicate(x => x.CreatedDate.Year == currentYear)
            .WithTracking(false)
            .Build());

        var totalRevenueInYear = await payments.SumAsync(x => x.Amount);

        var userDetailRepo = _unitOfWork.GetRepo<UserDetail>();
        var totalUserInYear = await userDetailRepo.Get(new QueryBuilder<UserDetail>()
            .WithPredicate(x => x.CreatedDate.Value.Year == currentYear && x.IsActive == true)
            .WithTracking(false)
            .Build()).CountAsync() - 1;

        var adRepo = _unitOfWork.GetRepo<Advertisement>();
        var totalAdsInYear = await adRepo.Get(new QueryBuilder<Advertisement>()
            .WithPredicate(x => x.IsActive == true &&
                                x.CreatedDate.Year == currentYear &&
                                x.Status == (int)AdvertisementStatus.Approved)
            .WithTracking(false)
            .Build()).CountAsync();

        var totalPurchasedPacakge = await payments.CountAsync();

        return new TotalStasticInYearDTO
        {
            TotalRevenueInYear = totalRevenueInYear,
            TotalUserInYear = totalUserInYear,
            TotalAdsInYear = totalAdsInYear,
            TotalPackageInYear = totalPurchasedPacakge
        };

    }
}