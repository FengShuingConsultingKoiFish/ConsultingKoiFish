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
                var pondCharacteristic = await repo.GetSingleAsync(new QueryBuilder<Pond>()
                    .WithPredicate(x => x.Id == pondCharacteristicId)
                    .Build());

                if (pondCharacteristic == null)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Pond characteristic not found." };
                }

                await repo.DeleteAsync(pondCharacteristic);
                var saver = await _unitOfWork.SaveAsync();

                if (!saver)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Failed to delete pond characteristic." };
                }

                return new BaseResponse { IsSuccess = true, Message = "Pond characteristic deleted successfully." };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BaseResponse { IsSuccess = false, Message = "Error occurred while deleting pond characteristic." };
            }
        }
    }
}
