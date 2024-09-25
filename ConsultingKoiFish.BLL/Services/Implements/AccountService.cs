using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.AccountDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.DAL.Enums;
using ConsultingKoiFish.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;

namespace ConsultingKoiFish.BLL.Services.Implements;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<IdentityUser> userManager,
                            RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }
    public async Task<BaseResponse> SignUpAsync(AccountCreateRequestDTO accRequest)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var existedUser = await _userManager.FindByEmailAsync(accRequest.EmailAddress);
            if (existedUser != null)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "The email is existed in the system. Please try again witn another email."
                };
            }

            var user = new IdentityUser
            {
                Email = accRequest.EmailAddress,
                UserName = accRequest.UserName
            };

            var createResult = await _userManager.CreateAsync(user, accRequest.Password);
            if (!createResult.Succeeded)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Một số lỗi xảy ra trong quá trình đăng kí tài khoản. Vui lòn thử lại sau ít phút"
                };
            }

            await _userManager.AddToRoleAsync(user, Role.Member.ToString());

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return new BaseResponse
            {
                IsSuccess = true,
                Message = $"Đăng kí thành công. Vui lòng xác thực thông tin đã gửi ở Email: {user.Email}"
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        } 
    }
}