using ConsultingKoiFish.BLL.DTOs.KoiDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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
        [HttpPost("Add-Koi-Zodiac")]
        public async Task<IActionResult> AddKoiZodiac([FromBody] ZodiacKoiBreedDTO zodiacKoiBreedDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _koiService.AddSuitableKoiZodiac(zodiacKoiBreedDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // PUT api/zodiac/Update-Koi-Zodiac/{id}
        [HttpPut("Update-Koi-Zodiac/{id}")]
        public async Task<IActionResult> UpdateKoiZodiac([FromBody] ZodiacKoiBreedDTO zodiacKoiBreedDto, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _koiService.UpdateKoiZodiac(zodiacKoiBreedDto, id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // GET api/zodiac/Get-All-Koi-Zodiac
        [HttpGet("Get-All-Koi-Zodiac")]
        public async Task<IActionResult> GetAllKoiZodiac()
        {
            var result = await _koiService.GetAllKoiZodiac();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // DELETE api/zodiac/Delete-Koi-Zodiac/{id}
        [HttpDelete("Delete-Koi-Zodiac/{id}")]
        public async Task<IActionResult> DeleteKoiZodiac(int id)
        {
            var result = await _koiService.DeleteKoiZodiac(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [Authorize]
        [HttpGet("Get-Suitable-Koi-For-User")]
        public async Task<IActionResult> GetSuitableKoiForUser()
        {
            var userId = UserId;
            var result = await _koiService.GetSuitableKoiForUser(userId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // GET api/koi/Get-KoiBreeds-By-KoiCategory/{categoryId}
        [HttpGet("Get-KoiBreeds-By-KoiCategory/{categoryId}")]
        public async Task<IActionResult> GetKoiBreedsByKoiCategory(int categoryId)
        {
            var result = await _koiService.GetKoiBreedByKoiCategory(categoryId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

    }
}
