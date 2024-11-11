using System.Net;
using AutoMapper;
using ConsultingKoiFish.BLL.DTOs.PondDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.Entities;
using ConsultingKoiFish.DAL.Queries;
using ConsultingKoiFish.DAL.UnitOfWork;

namespace ConsultingKoiFish.BLL.Services.Implements
{
    public class PondService : IPondService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PondService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        // Add Pond Category
        public async Task<BaseResponse> AddPondCategory(PondCategoryDTO pondCategoryDto)
        {
            try
            {
                var repo = _unitOfWork.GetRepo<PondCategory>();
                // var pondCategory = _mapper.Map<PondCategory>(pondCategoryDto);
                var pondCategory = new PondCategory()
                {
                    Name = pondCategoryDto.Name,
                    Description = pondCategoryDto.Description
                };
                await repo.CreateAsync(pondCategory);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResponse { IsSuccess = true, Message = "Pond category added successfully." };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse { IsSuccess = false, Message = "Failed to add pond category." };
            }
        }

        // Update Pond Category
        public async Task<BaseResponse> UpdatePondCategory(PondCategoryDTO pondCategoryDto, int pondCategoryId)
        {
            try
            {
                var repo = _unitOfWork.GetRepo<PondCategory>();

                var pondCategoryExists = await repo.AnyAsync(new QueryBuilder<PondCategory>()
                    .WithPredicate(x => x.Id.Equals(pondCategoryId))
                    .Build());

                if (!pondCategoryExists)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Pond category not found." };
                }

                var pondCategory = await repo.GetSingleAsync(new QueryBuilder<PondCategory>()
                    .WithPredicate(x => x.Id.Equals(pondCategoryId))
                    .Build());

                pondCategory.Name = pondCategoryDto.Name;
                pondCategory.Description = pondCategory.Description;

                await repo.UpdateAsync(pondCategory);
                var isSaved = await _unitOfWork.SaveAsync();

                if (!isSaved)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Failed to save pond category." };
                }

                return new BaseResponse { IsSuccess = true, Message = "Pond category updated successfully." };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse { IsSuccess = false, Message = "Failed to update pond category." };
            }
        }

        // Get All Pond Categories
        public async Task<ResponseApiDTO> GetAllPondCategory()
        {
            try
            {
                var repo = _unitOfWork.GetRepo<PondCategory>();
                var queryBuilder = new QueryBuilder<PondCategory>()
                    .WithOrderBy(q => q.OrderBy(p => p.Name)) // Assuming Name is a field to order by
                    .WithTracking(false);

                var pondCategories = await repo.GetAllAsync(queryBuilder.Build());
                var pondCategoryDtos = _mapper.Map<List<PondCategory>>(pondCategories);

                return new ResponseApiDTO { IsSuccess = true, Result = pondCategoryDtos };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ResponseApiDTO { IsSuccess = false, Message = "Failed to retrieve pond categories." };
            }
        }

        // Delete Pond Category
        public async Task<BaseResponse> DeletePondCategory(int pondCategoryId)
        {
            try
            {
                var repo = _unitOfWork.GetRepo<PondCategory>();
                var pondCategory = await repo.GetSingleAsync(new QueryBuilder<PondCategory>()
                    .WithPredicate(x => x.Id == pondCategoryId)
                    .Build());

                if (pondCategory == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Pond category not found." };
                }

                await repo.DeleteAsync(pondCategory);
                var saver = await _unitOfWork.SaveAsync();

                if (!saver)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Failed to delete pond category." };
                }

                return new BaseResponse { IsSuccess = true, Message = "Pond category deleted successfully." };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse { IsSuccess = false, Message = "Error occurred while deleting pond category." };
            }
        }

        // Add Pond Characteristic
        public async Task<BaseResponse> AddPondCharacteristic(PondCharacteristicDTO pondCharacteristicDto)
        {
            try
            {
                var repo = _unitOfWork.GetRepo<Pond>();
                //var pondCharacteristic = _mapper.Map<Pond>(pondCharacteristicDto);
                var pondCharacteristic = new Pond()
                {
                    Image = pondCharacteristicDto.Image,
                    Description = pondCharacteristicDto.Description,
                    PondCategoryId = pondCharacteristicDto.PondCategoryId,
                    Name = pondCharacteristicDto.Name
                };
                await repo.CreateAsync(pondCharacteristic);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResponse { IsSuccess = true, Message = "Pond characteristic added successfully." };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse { IsSuccess = false, Message = "Failed to add pond characteristic." };
            }
        }

        // Update Pond Characteristic
        public async Task<BaseResponse> UpdatePondCharacteristic(PondCharacteristicDTO pondCharacteristicDto, int pondCharacteristicId)
        {
            try
            {
                var repo = _unitOfWork.GetRepo<Pond>();

                var pondCharacteristicExists = await repo.AnyAsync(new QueryBuilder<Pond>()
                    .WithPredicate(x => x.Id.Equals(pondCharacteristicId))
                    .Build());

                if (!pondCharacteristicExists)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Pond characteristic not found." };
                }

                var pondCharacteristic = await repo.GetSingleAsync(new QueryBuilder<Pond>()
                    .WithPredicate(x => x.Id.Equals(pondCharacteristicId))
                    .Build());

                pondCharacteristic.Description = pondCharacteristicDto.Description;
                pondCharacteristic.Name = pondCharacteristicDto.Name;
                pondCharacteristic.PondCategoryId = pondCharacteristicDto.PondCategoryId;
                pondCharacteristic.Image = pondCharacteristicDto.Image;

                await repo.UpdateAsync(pondCharacteristic);
                var isSaved = await _unitOfWork.SaveAsync();

                if (!isSaved)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Failed to save pond characteristic." };
                }

                return new BaseResponse { IsSuccess = true, Message = "Pond characteristic updated successfully." };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse { IsSuccess = false, Message = "Failed to update pond characteristic." };
            }
        }

        // Get All Pond Characteristics
        public async Task<ResponseApiDTO> GetAllPondCharacteristic()
        {
            try
            {
                var repo = _unitOfWork.GetRepo<Pond>();
                var queryBuilder = new QueryBuilder<Pond>()
                    .WithOrderBy(q => q.OrderBy(p => p.Description)) // Assuming Description is a field to order by
                    .WithTracking(false);

                var pondCharacteristics = await repo.GetAllAsync(queryBuilder.Build());
                var pondCharacteristicDtos = _mapper.Map<List<Pond>>(pondCharacteristics);

                return new ResponseApiDTO { IsSuccess = true, Result = pondCharacteristicDtos };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ResponseApiDTO { IsSuccess = false, Message = "Failed to retrieve pond characteristics." };
            }
        }

        // Delete Pond Characteristic
        public async Task<BaseResponse> DeletePondCharacteristic(int pondCharacteristicId)
        {
            try
            {
                var repo = _unitOfWork.GetRepo<Pond>();

                // Kiểm tra nếu pondCharacteristic tồn tại
                var pondCharacteristic = await repo.GetSingleAsync(new QueryBuilder<Pond>()
                    .WithPredicate(x => x.Id == pondCharacteristicId)
                    .Build());

                if (pondCharacteristic == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Pond characteristic not found." };
                }

                // Xóa các bản ghi liên quan trong bảng PondZodiacs
                var pondZodiacRepo = _unitOfWork.GetRepo<PondZodiac>();
                var relatedPondZodiacs = await pondZodiacRepo.GetAllAsync(new QueryBuilder<PondZodiac>()
                    .WithPredicate(pz => pz.PondId == pondCharacteristicId)
                    .Build());

                foreach (var pondZodiac in relatedPondZodiacs)
                {
                    await pondZodiacRepo.DeleteAsync(pondZodiac);
                }

                // Sau khi xóa các bản ghi phụ thuộc, xóa pondCharacteristic
                await repo.DeleteAsync(pondCharacteristic);
                await _unitOfWork.SaveChangesAsync(); // Save without assigning result

                return new BaseResponse { IsSuccess = true, Message = "Pond characteristic deleted successfully." };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse { IsSuccess = false, Message = "Error occurred while deleting pond characteristic." };
            }
        }



        public async Task<BaseResponse> AddSuitablePondZodiac(ZodiacPondDTO zodiacPondDto)
        {
            try
            {
                var repo = _unitOfWork.GetRepo<PondZodiac>();
                var zodiacPond = new PondZodiac
                {
                    PondId = zodiacPondDto.PondId,
                    ZodiacId = zodiacPondDto.ZodiacId
                };

                await repo.CreateAsync(zodiacPond);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse { IsSuccess = true, Message = "Thêm tương hợp cung hoàng đạo-Hồ thành công" };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse { IsSuccess = false, Message = "Có lỗi xảy ra khi thêm tương hợp cung hoàng đạo-Hồ" };
            }
        }

        public async Task<BaseResponse> UpdatePondZodiac(ZodiacPondDTO zodiacPondDto, int zodiacPondId)
        {
            try
            {
                var repo = _unitOfWork.GetRepo<PondZodiac>();

                await _unitOfWork.BeginTransactionAsync();

                var zodiacExists = await repo.AnyAsync(new QueryBuilder<PondZodiac>()
                    .WithPredicate(x => x.Id.Equals(zodiacPondId))
                    .Build());

                if (!zodiacExists)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy tương hợp cung hoàng đạo-Hồ." };
                }

                var zodiacPondDetail = await repo.GetSingleAsync(new QueryBuilder<PondZodiac>()
                    .WithPredicate(x => x.Id.Equals(zodiacPondId))
                    .Build());

                zodiacPondDetail.PondId = zodiacPondDto.PondId;
                zodiacPondDetail.ZodiacId = zodiacPondDto.ZodiacId;

                await repo.UpdateAsync(zodiacPondDetail);
                var isSaved = await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitTransactionAsync();

                if (!isSaved)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Cập nhật thông tin thất bại." };
                }

                return new BaseResponse { IsSuccess = true, Message = "Cập nhật thông tin thành công." };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse { IsSuccess = false, Message = "Có lỗi xảy ra trong quá trình cập nhật." };
            }
        }

        public async Task<ResponseApiDTO> GetAllPondZodiac()
        {
            try
            {
                var queryBuilder = new QueryBuilder<PondZodiac>()
                    .WithOrderBy(q => q.OrderBy(p => p.PondId))
                    .WithTracking(false);

                var repo = _unitOfWork.GetRepo<PondZodiac>();
                var zodiacPonds = await repo.GetAllAsync(queryBuilder.Build());

                var zodiacPondDTOs = _mapper.Map<List<PondZodiac>>(zodiacPonds);
                return new ResponseApiDTO { IsSuccess = true, Result = zodiacPondDTOs };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ResponseApiDTO { IsSuccess = false, Message = "Không thể lấy danh sách tương hợp cung hoàng đạo-Hồ." };
            }
        }

        public async Task<BaseResponse> DeletePondZodiac(int zodiacPondId)
        {
            try
            {
                var repo = _unitOfWork.GetRepo<PondZodiac>();
                var zodiacPond = await repo.GetSingleAsync(new QueryBuilder<PondZodiac>()
                    .WithPredicate(x => x.Id == zodiacPondId)
                    .Build());

                if (zodiacPond == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy tương hợp cung hoàng đạo-Hồ." };
                }

                await repo.DeleteAsync(zodiacPond);
                var isSaved = await _unitOfWork.SaveAsync();

                if (!isSaved)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Xóa thất bại." };
                }

                return new BaseResponse { IsSuccess = true, Message = "Xóa thành công." };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse { IsSuccess = false, Message = "Có lỗi xảy ra khi xóa tương hợp cung hoàng đạo-Hồ." };
            }
        }

        public async Task<ResponseApiDTO> GetSuitablePondForUser(string userId)
        {
            try
            {
                // Retrieve user's zodiac sign from the UserZodiac repository
                var userZodiacRepo = _unitOfWork.GetRepo<UserZodiac>();
                var userZodiac = await userZodiacRepo.GetSingleAsync(new QueryBuilder<UserZodiac>()
                    .WithPredicate(x => x.UserId.Equals(userId))
                    .Build());

                if (userZodiac == null)
                {
                    return new ResponseApiDTO { IsSuccess = false, Message = "User's zodiac not found." };
                }

                // Retrieve matching ponds based on the user's zodiac sign, including pond and zodiac details
                var pondZodiacRepo = _unitOfWork.GetRepo<PondZodiac>();
                var queryBuilder = new QueryBuilder<PondZodiac>()
                    .WithPredicate(x => x.ZodiacId == userZodiac.ZodiacId)  // Match ZodiacId from PondZodiac
                    .WithInclude(x => x.Pond) // Include Pond details
                    .WithInclude(x => x.Zodiac) // Include Zodiac details
                    .Build();

                var matchingPonds = await pondZodiacRepo.GetAllAsync(queryBuilder);
                if (matchingPonds == null || !matchingPonds.Any())
                {
                    return new ResponseApiDTO { IsSuccess = false, Message = "No suitable ponds found for the user's zodiac sign." };
                }


                var suitablePondDTOs = matchingPonds.Select(pondZodiac => new
                {
                    PondId = pondZodiac.PondId,
                    PondName = pondZodiac.Pond?.Name,
                    ZodiacId = pondZodiac.ZodiacId,
                    ZodiacName = pondZodiac.Zodiac?.ZodiacName,
                    Image = pondZodiac.Pond?.Image,
                }).ToList();

                return new ResponseApiDTO { IsSuccess = true, Result = suitablePondDTOs, Message = "Suitable ponds retrieved successfully." };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ResponseApiDTO { IsSuccess = false, Message = "An error occurred while retrieving suitable ponds." };
            }
        }
        public async Task<ResponseApiDTO> GetPondByPondCategory(int pondCategoryId)
        {
            try
            {
                var pondRepo = _unitOfWork.GetRepo<Pond>();

                // Fetch all Pond that belong to the provided PondCategoryId
                var ponds = await pondRepo.GetAllAsync(new QueryBuilder<Pond>()
                    .WithPredicate(x => x.PondCategoryId == pondCategoryId)
                    .Build());

                if (ponds == null || !ponds.Any())
                {
                    return new ResponseApiDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy Pond theo PondCategoryId cung cấp."
                    };
                }

                return new ResponseApiDTO
                {
                    IsSuccess = true,
                    Result = ponds,
                    Message = "Lấy danh sách Pond thành công."
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách Pond."
                };
            }
        }



    }
}
