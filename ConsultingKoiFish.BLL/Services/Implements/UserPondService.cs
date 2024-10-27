using AutoMapper;
using ConsultingKoiFish.BLL.DTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;
using static ConsultingKoiFish.BLL.DTOs.KoiAndPondDetailRequestDTOs;

namespace ConsultingKoiFish.BLL.Services.Implements
{
    public class UserPondService : IUserPondService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserPondService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<BaseResponse> AddUserPond(UserPondDTOs userPondDtOs, string userId)
        {
            try
            {
                var repo = _unitOfWork.GetRepo<UserPond>();
                var userPond = new UserPond()
                {
                    UserId = userId,
                    PondName = userPondDtOs.PondName,
                    Quantity = userPondDtOs.Quantity,
                    Description = userPondDtOs.Description,
                    Image = userPondDtOs.Image,
                    Score = 0, // Initialize score to 0
                    ScoreDetail = "Pond Compatibility: 100%, Koi Compatibility: 100%" // Default compatibility
                };

                await repo.CreateAsync(userPond);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResponse { IsSuccess = true, Message = "Thêm hồ thành công" };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse { IsSuccess = false, Message = "Có lỗi xảy ra khi thêm hồ." };
            }
        }

        public async Task<BaseResponse> UpdateUserPond(UserPondDTOs userPondDtOs, int userPondId)
        {
            try
            {
                var repo = _unitOfWork.GetRepo<UserPond>();

                var userPond = await repo.GetSingleAsync(new QueryBuilder<UserPond>()
                    .WithPredicate(x => x.Id.Equals(userPondId))
                    .Build());

                if (userPond == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Hồ không tồn tại." };
                }

                userPond.PondName = userPondDtOs.PondName;
                userPond.Quantity = userPondDtOs.Quantity;
                userPond.Description = userPondDtOs.Description;
                userPond.Image = userPondDtOs.Image;

                await repo.UpdateAsync(userPond);
                var isSaved = await _unitOfWork.SaveAsync();

                if (!isSaved)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Lưu dữ liệu thất bại" };
                }

                return new BaseResponse { IsSuccess = true, Message = "Cập nhật hồ thành công" };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse { IsSuccess = false, Message = "Có lỗi xảy ra khi cập nhật hồ." };
            }
        }

        public async Task<ResponseApiDTO> GetAllUserPond(string userId)
        {
            try
            {
                var repo = _unitOfWork.GetRepo<UserPond>();

                var userPonds = await repo.GetAllAsync(new QueryBuilder<UserPond>()
                    .WithPredicate(x => x.UserId == userId)
                    .Build());

                if (userPonds == null || !userPonds.Any())
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy hồ nào cho người dùng này.",
                        Result = null
                    };
                }

                var userPondDtos = _mapper.Map<IEnumerable<UserPond>>(userPonds);

                return new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách hồ thành công.",
                    Result = userPondDtos
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách hồ.",
                    Result = null
                };
            }
        }

        public async Task<BaseResponse> DeleteUserPond(int userPondId)
        {
            try
            {
                var userPondRepo = _unitOfWork.GetRepo<UserPond>();
                var koiDetailRepo = _unitOfWork.GetRepo<KoiDetail>();
                var pondDetailRepo = _unitOfWork.GetRepo<PondDetail>();

                var userPond = await userPondRepo.GetSingleAsync(new QueryBuilder<UserPond>()
                    .WithPredicate(x => x.Id == userPondId)
                    .Build());

                if (userPond == null)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Hồ không tồn tại."
                    };
                }

                var koiDetails = await koiDetailRepo.GetAllAsync(new QueryBuilder<KoiDetail>()
                    .WithPredicate(x => x.UserPondId == userPondId)
                    .Build());

                foreach (var koiDetail in koiDetails)
                {
                    await koiDetailRepo.DeleteAsync(koiDetail);
                }

                var pondDetails = await pondDetailRepo.GetAllAsync(new QueryBuilder<PondDetail>()
                    .WithPredicate(x => x.UserPondId == userPondId)
                    .Build());

                foreach (var pondDetail in pondDetails)
                {
                    await pondDetailRepo.DeleteAsync(pondDetail);
                }

                await userPondRepo.DeleteAsync(userPond);

                var isDeleted = await _unitOfWork.SaveAsync();

                if (!isDeleted)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Xóa hồ thất bại."
                    };
                }

                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "Xóa hồ và các chi tiết liên quan thành công."
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa hồ và các chi tiết liên quan."
                };
            }
        }

        public async Task<ResponseApiDTO> AddKoiAndPondDetails(KoiAndPondDetailRequestDTO requestDto)
        {
            try
            {
                var userPondRepo = _unitOfWork.GetRepo<UserPond>();
                var koiDetailRepo = _unitOfWork.GetRepo<KoiDetail>();
                var pondDetailRepo = _unitOfWork.GetRepo<PondDetail>();
                var koiBreedZodiacRepo = _unitOfWork.GetRepo<KoiBreedZodiac>();
                var pondZodiacRepo = _unitOfWork.GetRepo<PondZodiac>();
                var userZodiacRepo = _unitOfWork.GetRepo<UserZodiac>();

                var userPond = await userPondRepo.GetSingleAsync(new QueryBuilder<UserPond>()
                    .WithPredicate(x => x.Id == requestDto.PondId)
                    .Build());

                if (userPond == null)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        Message = "UserPond không tồn tại."
                    };
                }

                var userZodiac = await userZodiacRepo.GetSingleAsync(new QueryBuilder<UserZodiac>()
                    .WithPredicate(x => x.UserId == userPond.UserId)
                    .Build());

                if (userZodiac == null)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        Message = "Zodiac không tồn tại cho người dùng này."
                    };
                }

                var existingKoiDetails = await koiDetailRepo.GetAllAsync(new QueryBuilder<KoiDetail>()
                    .WithPredicate(x => x.UserPondId == requestDto.PondId)
                    .Build());

                var existingKoiCount = existingKoiDetails.Count();

                if (requestDto.KoiDetails != null && requestDto.KoiDetails.Any())
                {
                    int totalKoiToAdd = requestDto.KoiDetails.Count;
                    if (existingKoiCount + totalKoiToAdd > userPond.Quantity)
                    {
                        return new ResponseApiDTO
                        {
                            IsSuccess = false,
                            Message = $"Không thể thêm quá số lượng cá Koi cho phép trong hồ, số lượng tối đa: {userPond.Quantity}"
                        };
                    }

                    foreach (var koiDetailDto in requestDto.KoiDetails)
                    {
                        var koiDetail = new KoiDetail
                        {
                            UserPondId = requestDto.PondId,
                            KoiBreedId = koiDetailDto.KoiBreedId
                        };
                        await koiDetailRepo.CreateAsync(koiDetail);
                    }
                }

                if (requestDto.PondDetails != null && requestDto.PondDetails.Any())
                {
                    foreach (var pondDetailDto in requestDto.PondDetails)
                    {
                        var pondDetail = new PondDetail
                        {
                            UserPondId = requestDto.PondId,
                            PondId = pondDetailDto.PondId
                        };
                        await pondDetailRepo.CreateAsync(pondDetail);
                    }
                }

                // Save all changes
                await _unitOfWork.SaveChangesAsync();

                // **Calculate Pond Compatibility Percentage**
                double pondCompatibilityPercentage = 100; // Default to 100%
                var pondDetails = await pondDetailRepo.GetAllAsync(new QueryBuilder<PondDetail>()
                    .WithPredicate(x => x.UserPondId == requestDto.PondId)
                    .Build());

                if (pondDetails.Any())
                {
                    int totalPonds = pondDetails.Count();

                    var compatiblePondIds = (await pondZodiacRepo.GetAllAsync(new QueryBuilder<PondZodiac>()
                        .WithPredicate(x => x.ZodiacId == userZodiac.ZodiacId)
                        .Build())).Select(pz => pz.PondId).ToList();

                    int matchingPondsCount = pondDetails
                        .Count(pd => compatiblePondIds.Contains(pd.PondId));

                    pondCompatibilityPercentage = totalPonds > 0
                        ? (matchingPondsCount / (double)totalPonds) * 100
                        : 0;
                }
                else
                {
                    pondCompatibilityPercentage = 0; // No pond details
                }

                // **Calculate Koi Compatibility Percentage**
                double koiCompatibilityPercentage = 100; // Default to 100%
                var koiDetails = await koiDetailRepo.GetAllAsync(new QueryBuilder<KoiDetail>()
                    .WithPredicate(x => x.UserPondId == requestDto.PondId)
                    .Build());

                if (koiDetails.Any())
                {
                    int totalKoi = koiDetails.Count();

                    var compatibleKoiBreedIds = (await koiBreedZodiacRepo.GetAllAsync(new QueryBuilder<KoiBreedZodiac>()
                        .WithPredicate(x => x.ZodiacId == userZodiac.ZodiacId)
                        .Build())).Select(kbz => kbz.KoiBreedId).ToList();

                    int matchingKoiCount = koiDetails
                        .Count(kd => compatibleKoiBreedIds.Contains(kd.KoiBreedId));

                    koiCompatibilityPercentage = totalKoi > 0
                        ? (matchingKoiCount / (double)totalKoi) * 100
                        : 0;
                }
                else
                {
                    koiCompatibilityPercentage = 0; // No koi details
                }

                // **Calculate Final Compatibility Percentage**
                double finalCompatibilityPercentage = (pondCompatibilityPercentage * 0.5) + (koiCompatibilityPercentage * 0.5);

                if (double.IsNaN(finalCompatibilityPercentage) || double.IsInfinity(finalCompatibilityPercentage))
                {
                    finalCompatibilityPercentage = 0;
                }

                finalCompatibilityPercentage = Math.Max(0, Math.Min(100, finalCompatibilityPercentage));

                // Update ScoreDetail and Score
                userPond.ScoreDetail = $"Pond Compatibility: {pondCompatibilityPercentage}%, Koi Compatibility: {koiCompatibilityPercentage}%";
                userPond.Score = finalCompatibilityPercentage;

                // Update the UserPond in the database
                await userPondRepo.UpdateAsync(userPond);
                var isSaved = await _unitOfWork.SaveAsync();

                if (!isSaved)
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        Message = "Không thể cập nhật phần trăm hợp sau khi thêm chi tiết Koi và hồ."
                    };
                }

                return new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = $"Thêm chi tiết Koi và hồ thành công, phần trăm hợp cho hồ cá Koi của bạn là {finalCompatibilityPercentage}%"
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi thêm chi tiết Koi và hồ."
                };
            }
        }

        public async Task<BaseResponse> UpdateKoiAndPondDetails(int userPondId, KoiAndPondDetailRequestDTO requestDto)
        {
            try
            {
                var userPondRepo = _unitOfWork.GetRepo<UserPond>();
                var koiDetailRepo = _unitOfWork.GetRepo<KoiDetail>();
                var pondDetailRepo = _unitOfWork.GetRepo<PondDetail>();

                var userPond = await userPondRepo.GetSingleAsync(new QueryBuilder<UserPond>()
                    .WithPredicate(x => x.Id == userPondId)
                    .Build());

                if (userPond == null)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "UserPond không tồn tại."
                    };
                }

                // Delete existing KoiDetails and PondDetails
                var existingKoiDetails = await koiDetailRepo.GetAllAsync(new QueryBuilder<KoiDetail>()
                    .WithPredicate(x => x.UserPondId == userPondId)
                    .Build());

                var existingPondDetails = await pondDetailRepo.GetAllAsync(new QueryBuilder<PondDetail>()
                    .WithPredicate(x => x.UserPondId == userPondId)
                    .Build());

                foreach (var koiDetail in existingKoiDetails)
                    await koiDetailRepo.DeleteAsync(koiDetail);

                foreach (var pondDetail in existingPondDetails)
                    await pondDetailRepo.DeleteAsync(pondDetail);

                // Add updated KoiDetails
                if (requestDto.KoiDetails != null && requestDto.KoiDetails.Any())
                {
                    foreach (var koiDetailDto in requestDto.KoiDetails)
                    {
                        var koiDetail = new KoiDetail
                        {
                            UserPondId = userPondId,
                            KoiBreedId = koiDetailDto.KoiBreedId
                        };
                        await koiDetailRepo.CreateAsync(koiDetail);
                    }
                }

                // Add updated PondDetails
                if (requestDto.PondDetails != null && requestDto.PondDetails.Any())
                {
                    foreach (var pondDetailDto in requestDto.PondDetails)
                    {
                        var pondDetail = new PondDetail
                        {
                            UserPondId = userPondId,
                            PondId = pondDetailDto.PondId
                        };
                        await pondDetailRepo.CreateAsync(pondDetail);
                    }
                }

                // Save changes to database
                await _unitOfWork.SaveChangesAsync();

                // Optionally, you may want to recalculate the compatibility percentages here
                // Similar to the AddKoiAndPondDetails method

                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "Cập nhật chi tiết Koi và hồ thành công."
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật chi tiết Koi và hồ."
                };
            }
        }

        public async Task<ResponseApiDTO> ViewKoiAndPondDetails(int userPondId)
        {
            try
            {
                var koiDetailRepo = _unitOfWork.GetRepo<KoiDetail>();
                var pondDetailRepo = _unitOfWork.GetRepo<PondDetail>();
                var pondCategoryRepo = _unitOfWork.GetRepo<PondCategory>();
                var koiCategoryRepo = _unitOfWork.GetRepo<KoiCategory>();

                var koiDetails = await koiDetailRepo.GetAllAsync(new QueryBuilder<KoiDetail>()
                    .WithPredicate(x => x.UserPondId == userPondId)
                    .WithInclude(x => x.KoiBreed)
                    .Build());

                var pondDetails = await pondDetailRepo.GetAllAsync(new QueryBuilder<PondDetail>()
                    .WithPredicate(x => x.UserPondId == userPondId)
                    .WithInclude(x => x.Pond)
                    .Build());

                if ((koiDetails == null || !koiDetails.Any()) && (pondDetails == null || !pondDetails.Any()))
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy chi tiết Koi hoặc hồ cho UserPond này.",
                        Result = null
                    };
                }

                var pondCategoryIds = pondDetails.Select(pd => pd.Pond.PondCategoryId).Distinct().ToList();
                var pondCategories = await pondCategoryRepo.GetAllAsync(new QueryBuilder<PondCategory>()
                    .WithPredicate(pc => pondCategoryIds.Contains(pc.Id))
                    .Build());

                var koiCategoryIds = koiDetails.Select(kd => kd.KoiBreed.KoiCategoryId).Distinct().ToList();
                var koiCategories = await koiCategoryRepo.GetAllAsync(new QueryBuilder<KoiCategory>()
                    .WithPredicate(kc => koiCategoryIds.Contains(kc.Id))
                    .Build());

                var koiDetailDtos = koiDetails.Select(koiDetail =>
                {
                    var koiCategoryName = koiCategories.FirstOrDefault(kc => kc.Id == koiDetail.KoiBreed?.KoiCategoryId)?.Name ?? "Unknown Category";
                    return new
                    {
                        KoiDetailId = koiDetail.Id,
                        KoiBreedName = koiDetail.KoiBreed?.Name ?? "Unknown Breed",
                        KoiCategory = koiCategoryName,
                        Image = koiDetail.KoiBreed?.Image,
                    };
                }).ToList();

                var pondDetailDtos = pondDetails.Select(pondDetail =>
                {
                    var pondCategoryName = pondCategories.FirstOrDefault(pc => pc.Id == pondDetail.Pond?.PondCategoryId)?.Name ?? "Unknown Category";
                    return new
                    {
                        PondDetailId = pondDetail.Id,
                        PondName = pondDetail.Pond?.Name ?? "Unknown Pond",
                        PondCategory = pondCategoryName,
                        Image = pondDetail.Pond?.Image,
                    };
                }).ToList();

                return new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Lấy chi tiết Koi và hồ thành công.",
                    Result = new
                    {
                        KoiDetails = koiDetailDtos,
                        PondDetails = pondDetailDtos
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "Đã xảy ra lỗi khi lấy chi tiết Koi và hồ.",
                    Result = null
                };
            }
        }

        public async Task<BaseResponse> DeleteKoiBreedFromUserPond(int userPondId, int koiBreedId)
        {
            try
            {
                var koiDetailRepo = _unitOfWork.GetRepo<KoiDetail>();
                var koiBreedZodiacRepo = _unitOfWork.GetRepo<KoiBreedZodiac>();
                var userPondRepo = _unitOfWork.GetRepo<UserPond>();
                var userZodiacRepo = _unitOfWork.GetRepo<UserZodiac>();
                var pondDetailRepo = _unitOfWork.GetRepo<PondDetail>();
                var pondZodiacRepo = _unitOfWork.GetRepo<PondZodiac>();

                var koiDetail = await koiDetailRepo.GetSingleAsync(new QueryBuilder<KoiDetail>()
                    .WithPredicate(x => x.UserPondId == userPondId && x.KoiBreedId == koiBreedId)
                    .Build());

                if (koiDetail == null)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy chi tiết cá Koi để xóa."
                    };
                }

                // Delete the koi detail
                await koiDetailRepo.DeleteAsync(koiDetail);
                await _unitOfWork.SaveChangesAsync();

                // Recalculate the compatibility percentages
                var userPond = await userPondRepo.GetSingleAsync(new QueryBuilder<UserPond>()
                    .WithPredicate(x => x.Id == userPondId)
                    .Build());

                var userZodiac = await userZodiacRepo.GetSingleAsync(new QueryBuilder<UserZodiac>()
                    .WithPredicate(x => x.UserId == userPond.UserId)
                    .Build());

                if (userZodiac == null)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Zodiac không tồn tại cho người dùng này."
                    };
                }

                // **Calculate Koi Compatibility Percentage**
                var koiDetails = await koiDetailRepo.GetAllAsync(new QueryBuilder<KoiDetail>()
                    .WithPredicate(x => x.UserPondId == userPondId)
                    .Build());

                double koiCompatibilityPercentage = 100; // Default to 100% if no koi details
                if (koiDetails.Any())
                {
                    int totalKoi = koiDetails.Count();

                    var compatibleKoiBreedIds = (await koiBreedZodiacRepo.GetAllAsync(new QueryBuilder<KoiBreedZodiac>()
                        .WithPredicate(x => x.ZodiacId == userZodiac.ZodiacId)
                        .Build())).Select(kbz => kbz.KoiBreedId).ToList();

                    int compatibleKoiCount = koiDetails
                        .Count(kd => compatibleKoiBreedIds.Contains(kd.KoiBreedId));

                    koiCompatibilityPercentage = totalKoi > 0
                        ? (compatibleKoiCount / (double)totalKoi) * 100
                        : 0;
                }
                else
                {
                    koiCompatibilityPercentage = 0; // No koi details
                }

                // **Retrieve Pond Compatibility Percentage**
                double pondCompatibilityPercentage = 100; // Default to 100%
                var pondDetails = await pondDetailRepo.GetAllAsync(new QueryBuilder<PondDetail>()
                    .WithPredicate(x => x.UserPondId == userPondId)
                    .Build());

                if (pondDetails.Any())
                {
                    int totalPonds = pondDetails.Count();

                    var compatiblePondIds = (await pondZodiacRepo.GetAllAsync(new QueryBuilder<PondZodiac>()
                        .WithPredicate(x => x.ZodiacId == userZodiac.ZodiacId)
                        .Build())).Select(pz => pz.PondId).ToList();

                    int compatiblePondsCount = pondDetails
                        .Count(pd => compatiblePondIds.Contains(pd.PondId));

                    pondCompatibilityPercentage = totalPonds > 0
                        ? (compatiblePondsCount / (double)totalPonds) * 100
                        : 0;
                }
                else
                {
                    pondCompatibilityPercentage = 0; // No pond details
                }

                // **Calculate Final Compatibility Percentage**
                double finalCompatibilityPercentage = (pondCompatibilityPercentage * 0.5) + (koiCompatibilityPercentage * 0.5);

                if (double.IsNaN(finalCompatibilityPercentage) || double.IsInfinity(finalCompatibilityPercentage))
                {
                    finalCompatibilityPercentage = 0;
                }

                finalCompatibilityPercentage = Math.Max(0, Math.Min(100, finalCompatibilityPercentage));


                // Update ScoreDetail and Score
                userPond.ScoreDetail = $"Pond Compatibility: {pondCompatibilityPercentage}%, Koi Compatibility: {koiCompatibilityPercentage}%";
                userPond.Score = finalCompatibilityPercentage;

                // Update the UserPond in the database
                await userPondRepo.UpdateAsync(userPond);
                var isSaved = await _unitOfWork.SaveAsync();

                if (!isSaved)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Xóa cá Koi thành công nhưng không thể cập nhật phần trăm hợp."
                    };
                }

                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = $"Xóa cá Koi thành công, phần trăm hợp đã được cập nhật là {finalCompatibilityPercentage}%."
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa cá Koi."
                };
            }
        }

        public async Task<BaseResponse> DeletePondFromUserPond(int userPondId, int pondId)
        {
            try
            {
                var pondDetailRepo = _unitOfWork.GetRepo<PondDetail>();
                var pondZodiacRepo = _unitOfWork.GetRepo<PondZodiac>();
                var userPondRepo = _unitOfWork.GetRepo<UserPond>();
                var userZodiacRepo = _unitOfWork.GetRepo<UserZodiac>();
                var koiDetailRepo = _unitOfWork.GetRepo<KoiDetail>();
                var koiBreedZodiacRepo = _unitOfWork.GetRepo<KoiBreedZodiac>();

                var pondDetail = await pondDetailRepo.GetSingleAsync(new QueryBuilder<PondDetail>()
                    .WithPredicate(x => x.UserPondId == userPondId && x.PondId == pondId)
                    .Build());

                if (pondDetail == null)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy chi tiết hồ để xóa."
                    };
                }

                // Delete the pond detail
                await pondDetailRepo.DeleteAsync(pondDetail);
                await _unitOfWork.SaveChangesAsync();

                // Recalculate the compatibility percentages
                var userPond = await userPondRepo.GetSingleAsync(new QueryBuilder<UserPond>()
                    .WithPredicate(x => x.Id == userPondId)
                    .Build());

                var userZodiac = await userZodiacRepo.GetSingleAsync(new QueryBuilder<UserZodiac>()
                    .WithPredicate(x => x.UserId == userPond.UserId)
                    .Build());

                if (userZodiac == null)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Zodiac không tồn tại cho người dùng này."
                    };
                }

                // **Calculate Pond Compatibility Percentage**
                var pondDetails = await pondDetailRepo.GetAllAsync(new QueryBuilder<PondDetail>()
                    .WithPredicate(x => x.UserPondId == userPondId)
                    .Build());

                double pondCompatibilityPercentage = 100; // Default to 100%
                if (pondDetails.Any())
                {
                    int totalPonds = pondDetails.Count();

                    var compatiblePondIds = (await pondZodiacRepo.GetAllAsync(new QueryBuilder<PondZodiac>()
                        .WithPredicate(x => x.ZodiacId == userZodiac.ZodiacId)
                        .Build())).Select(pz => pz.PondId).ToList();

                    int compatiblePondsCount = pondDetails
                        .Count(pd => compatiblePondIds.Contains(pd.PondId));

                    pondCompatibilityPercentage = totalPonds > 0
                        ? (compatiblePondsCount / (double)totalPonds) * 100
                        : 0;
                }
                else
                {
                    pondCompatibilityPercentage = 0; // No pond details
                }

                // **Retrieve Koi Compatibility Percentage**
                double koiCompatibilityPercentage = 100; // Default to 100%
                var koiDetails = await koiDetailRepo.GetAllAsync(new QueryBuilder<KoiDetail>()
                    .WithPredicate(x => x.UserPondId == userPondId)
                    .Build());

                if (koiDetails.Any())
                {
                    int totalKoi = koiDetails.Count();

                    var compatibleKoiBreedIds = (await koiBreedZodiacRepo.GetAllAsync(new QueryBuilder<KoiBreedZodiac>()
                        .WithPredicate(x => x.ZodiacId == userZodiac.ZodiacId)
                        .Build())).Select(kbz => kbz.KoiBreedId).ToList();

                    int compatibleKoiCount = koiDetails
                        .Count(kd => compatibleKoiBreedIds.Contains(kd.KoiBreedId));

                    koiCompatibilityPercentage = totalKoi > 0
                        ? (compatibleKoiCount / (double)totalKoi) * 100
                        : 0;
                }
                else
                {
                    koiCompatibilityPercentage = 0; // No koi details
                }

                // **Calculate Final Compatibility Percentage**
                double finalCompatibilityPercentage = (pondCompatibilityPercentage * 0.5) + (koiCompatibilityPercentage * 0.5);

                if (double.IsNaN(finalCompatibilityPercentage) || double.IsInfinity(finalCompatibilityPercentage))
                {
                    finalCompatibilityPercentage = 0;
                }

                finalCompatibilityPercentage = Math.Max(0, Math.Min(100, finalCompatibilityPercentage));

                // Update ScoreDetail and Score
                userPond.ScoreDetail = $"Pond Compatibility: {pondCompatibilityPercentage}%, Koi Compatibility: {koiCompatibilityPercentage}%";
                userPond.Score = finalCompatibilityPercentage;

                // Update the UserPond in the database
                await userPondRepo.UpdateAsync(userPond);
                var isSaved = await _unitOfWork.SaveAsync();

                if (!isSaved)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Xóa hồ thành công nhưng không thể cập nhật phần trăm hợp."
                    };
                }

                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = $"Xóa hồ thành công, phần trăm hợp đã được cập nhật là {finalCompatibilityPercentage}%."
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa hồ."
                };
            }
        }
    }
}
