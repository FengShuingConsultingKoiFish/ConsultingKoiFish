using ConsultingKoiFish.BLL.DTOs;
using ConsultingKoiFish.BLL.DTOs.Response;
using ConsultingKoiFish.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ConsultingKoiFish.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPondController : BaseAPIController
    {
        private readonly IUserPondService _userPondService;

        public UserPondController(IUserPondService userPondService)
        {
            _userPondService = userPondService;
        }

        // POST api/userpond/add
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddUserPond([FromBody] UserPondDTOs userPondDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = UserId;

            var result = await _userPondService.AddUserPond(userPondDto, userId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // PUT api/userpond/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUserPond([FromBody] UserPondDTOs userPondDto, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userPondService.UpdateUserPond(userPondDto, id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // GET api/userpond/getall/{userId}
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllUserPonds()
        {
            var userId = UserId;
            var result = await _userPondService.GetAllUserPond(userId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // DELETE api/userpond/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUserPond(int id)
        {
            var result = await _userPondService.DeleteUserPond(id);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // POST api/userpond/adddetails
        [HttpPost("adddetails")]
        public async Task<IActionResult> AddKoiAndPondDetails([FromBody] KoiAndPondDetailRequestDTOs.KoiAndPondDetailRequestDTO requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userPondService.AddKoiAndPondDetails(requestDto);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // PUT api/userpond/updatedetails/{userPondId}
        [HttpPut("updatedetails/{userPondId}")]
        public async Task<IActionResult> UpdateKoiAndPondDetails([FromBody] KoiAndPondDetailRequestDTOs.KoiAndPondDetailRequestDTO requestDto, int userPondId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userPondService.UpdateKoiAndPondDetails(userPondId, requestDto);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        // GET api/userpond/viewdetails/{userPondId}
        [HttpGet("viewdetails/{userPondId}")]
        public async Task<IActionResult> ViewKoiAndPondDetails(int userPondId)
        {
            var result = await _userPondService.ViewKoiAndPondDetails(userPondId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        
        // DELETE api/userpond/deletekoibreed
        [HttpDelete("deletekoibreed")]
        public async Task<IActionResult> DeleteKoiBreedFromUserPond([FromForm] int userPondId, [FromForm] int koiBreedId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userPondService.DeleteKoiBreedFromUserPond(userPondId, koiBreedId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }


        // DELETE api/userpond/deletepond
        [HttpDelete("deletepond")]
        public async Task<IActionResult> DeletePondFromUserPond([FromForm] int userPondId, [FromForm] int pondId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userPondService.DeletePondFromUserPond(userPondId, pondId);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }




    }
}
