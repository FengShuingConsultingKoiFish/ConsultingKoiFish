using ConsultingKoiFish.BLL.DTOs.PondDTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConsultingKoiFish.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PondController : BaseAPIController
    {
        private readonly IPondService _pondService;

        public PondController(IPondService pondService)
        {
            _pondService = pondService;
        }

        // POST api/pond/Add-PondCategory
        [HttpPost("Add-PondCategory")]
        public async Task<IActionResult> AddPondCategory([FromBody] PondCategoryDTO pondCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _pondService.AddPondCategory(pondCategoryDto);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // GET api/pond/Get-All-PondCategories
        [HttpGet("Get-All-PondCategories")]
        public async Task<IActionResult> GetAllPondCategories()
        {
            var result = await _pondService.GetAllPondCategory();
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // PUT api/pond/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePondCategory([FromBody] PondCategoryDTO pondCategoryDto, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _pondService.UpdatePondCategory(pondCategoryDto, id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // DELETE api/pond/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePondCategory(int id)
        {
            var result = await _pondService.DeletePondCategory(id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // POST api/pond/Add-PondCharacteristic
        [HttpPost("Add-PondCharacteristic")]
        public async Task<IActionResult> AddPondCharacteristic([FromBody] PondCharacteristicDTO pondCharacteristicDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _pondService.AddPondCharacteristic(pondCharacteristicDto);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // GET api/pond/Get-All-PondCharacteristics
        [HttpGet("Get-All-PondCharacteristics")]
        public async Task<IActionResult> GetAllPondCharacteristics()
        {
            var result = await _pondService.GetAllPondCharacteristic();
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // PUT api/pond/{id}/PondCharacteristic
        [HttpPut("{id}/PondCharacteristic")]
        public async Task<IActionResult> UpdatePondCharacteristic([FromBody] PondCharacteristicDTO pondCharacteristicDto, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _pondService.UpdatePondCharacteristic(pondCharacteristicDto, id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // DELETE api/pond/{id}/PondCharacteristic
        [HttpDelete("{id}/PondCharacteristic")]
        public async Task<IActionResult> DeletePondCharacteristic(int id)
        {
            var result = await _pondService.DeletePondCharacteristic(id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        [HttpPost("Add-Pond-Zodiac")]
        public async Task<IActionResult> AddPondZodiac([FromBody] ZodiacPondDTO zodiacPondDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _pondService.AddSuitablePondZodiac(zodiacPondDto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // PUT api/zodiac/Update-Pond-Zodiac/{id}
        [HttpPut("Update-Pond-Zodiac/{id}")]
        public async Task<IActionResult> UpdatePondZodiac([FromBody] ZodiacPondDTO zodiacPondDto, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _pondService.UpdatePondZodiac(zodiacPondDto, id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // GET api/zodiac/Get-All-Pond-Zodiac
        [HttpGet("Get-All-Pond-Zodiac")]
        public async Task<IActionResult> GetAllPondZodiac()
        {
            var result = await _pondService.GetAllPondZodiac();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // DELETE api/zodiac/Delete-Pond-Zodiac/{id}
        [HttpDelete("Delete-Pond-Zodiac/{id}")]
        public async Task<IActionResult> DeletePondZodiac(int id)
        {
            var result = await _pondService.DeletePondZodiac(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("Get-Suitable-Pond-For-User/{userId}")]
        public async Task<IActionResult> GetSuitablePondForUser(string userId)
        {
            var result = await _pondService.GetSuitablePondForUser(userId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
