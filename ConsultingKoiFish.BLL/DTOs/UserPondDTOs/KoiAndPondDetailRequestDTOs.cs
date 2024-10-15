using ConsultingKoiFish.DAL.Entities;

namespace ConsultingKoiFish.BLL.DTOs;

public class KoiAndPondDetailRequestDTOs
{
    public class KoiAndPondDetailRequestDTO
    {
        public int PondId { get; set; }
        public List<KoiDetailDTO> KoiDetails { get; set; }
        public List<PondDetailDTO> PondDetails { get; set; }
    }
}