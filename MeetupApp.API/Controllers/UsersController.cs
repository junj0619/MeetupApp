using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MeetupApp.API.Data;
using MeetupApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace MeetupApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMeetupRepository _meetupRepository;
        private readonly IMapper _mapper;

        public UsersController(IMeetupRepository meetupRepository, IMapper mapper)
        {
            _meetupRepository = meetupRepository;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _meetupRepository.GetUser(id);
            var userForReturn = _mapper.Map<UserForDetailDto>(user);
            return Ok(userForReturn);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _meetupRepository.GetUsers();
            var usersForReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersForReturn);
        }

        // [HttpPost]
        // public async Task<IActionResult> Add(User user)
        // {
        //     await _meetupRepository.Add(user);
        //     return Ok();
        // }

        // [HttpDelete]
        // public async Task<IActionResult> Delete(User user)
        // {
        //     await _meetupRepository.Delete(user);
        //     return Ok();
        // }
    }
}