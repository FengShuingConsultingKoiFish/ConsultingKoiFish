using ConsultingKoiFish.BLL.DTOs.ImageDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Interfaces
{
    public interface IImageService
    {
       Task<BaseResponse> CreateUpdateImage(ImageRequestDTO requestDTO, string userId);
    }
}
