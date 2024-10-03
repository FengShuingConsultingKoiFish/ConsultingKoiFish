using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.DTOs.ZodiacDTO;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ConsultingKoiFish.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZodiacController : BaseAPIController
    {
        private readonly IZodiacService _zodiacService;

        public ZodiacController(IZodiacService zodiacService)
        {
            _zodiacService = zodiacService;
        }

        // POST api/zodiac
        [HttpPost]
        public async Task<IActionResult> AddZodiac([FromBody] ZodiacRequestDTO zodiacRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _zodiacService.AddZodiac(zodiacRequestDto);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // GET api/zodiac
        [HttpGet]
        public async Task<IActionResult> GetAllZodiacs()
        {
            var result = await _zodiacService.GetAllZodiacs();
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // PUT api/zodiac/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateZodiac([FromBody] ZodiacRequestDTO zodiacRequestDto, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _zodiacService.UpdateZodiac(zodiacRequestDto, id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // DELETE api/zodiac/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteZodiac(int id)
        {
            var result = await _zodiacService.DeleteZodiac(id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        [Authorize]
        [HttpGet("birthdate")]
        public async Task<IActionResult> GetZodiacByBirthDate([FromQuery] DateTime birthDate)
        {
            var result = await _zodiacService.GetZodiacByBirthDate(birthDate, UserId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
