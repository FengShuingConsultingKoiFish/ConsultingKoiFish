using ConsultingKoiFish.BLL.DTOs.KoiDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConsultingKoiFish.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KoiController : BaseAPIController
    {
        private readonly IKoiService _koiService;

        public KoiController(IKoiService koiService)
        {
            _koiService = koiService;
        }

        // POST api/koi/Add-KoiCategory
        [HttpPost("Add-KoiCategory")]
        public async Task<IActionResult> AddKoiCategory([FromBody] KoiCategoryDTO koiCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _koiService.AddKoiCategory(koiCategoryDto);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // GET api/koi/Get-All-KoiCategories
        [HttpGet("Get-All-KoiCategories")]
        public async Task<IActionResult> GetAllKoiCategories()
        {
            var result = await _koiService.GetAllKoiCategory();
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // PUT api/koi/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKoiCategory([FromBody] KoiCategoryDTO koiCategoryDto, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _koiService.UpdateKoiCategory(koiCategoryDto, id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // DELETE api/koi/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKoiCategory(int id)
        {
            var result = await _koiService.DeleteKoiCategory(id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        // POST api/koi/Add-KoiBreed
        [HttpPost("Add-KoiBreed")]
        public async Task<IActionResult> AddKoiBreed([FromBody] KoiBreedDTO koiBreedDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _koiService.AddKoiBreed(koiBreedDto);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // GET api/koi/Get-All-KoiBreeds
        [HttpGet("Get-All-KoiBreeds")]
        public async Task<IActionResult> GetAllKoiBreeds()
        {
            var result = await _koiService.GetAllKoiBreed();
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // PUT api/koi/{id}/KoiBreed
        [HttpPut("{id}/KoiBreed")]
        public async Task<IActionResult> UpdateKoiBreed([FromBody] KoiBreedDTO koiBreedDto, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _koiService.UpdateKoiBreed(koiBreedDto, id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // DELETE api/koi/{id}/KoiBreed
        [HttpDelete("{id}/KoiBreed")]
        public async Task<IActionResult> DeleteKoiBreed(int id)
        {
            var result = await _koiService.DeleteKoiBreed(id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
