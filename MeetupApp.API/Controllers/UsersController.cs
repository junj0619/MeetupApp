using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MeetupApp.API.Data;
using MeetupApp.API.Dtos;
using MeetupApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace MeetupApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
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

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _meetupRepository.GetUser(id);
            var userForReturn = _mapper.Map<UserForDetailDto>(user);
            return Ok(userForReturn);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _meetupRepository.GetUsers();
            var usersForReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersForReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdate)
        {
            /* If given userId is not matching to Token user. Then return Unauthorized */
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await _meetupRepository.GetUser(id);
            _mapper.Map(userForUpdate, userFromRepo);

            if (await _meetupRepository.SaveAll())
            {
                return NoContent();
            }

            throw new Exception($"Update user {id} failed on save.");
        }

    }
}