using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MeetupApp.API.Data;
using MeetupApp.API.Dtos;
using MeetupApp.API.Helpers;
using MeetupApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetupApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/users/{userId}/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMeetupRepository _repo;
        private readonly IMapper _mapper;

        public MessagesController(IMeetupRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }


        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            /* If given userId is not matching to Token user. Then return Unauthorized */
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageForRepo = await _repo.GetMessage(id);

            if (messageForRepo == null)
            {
                return NotFound();
            }

            return Ok(messageForRepo);
        }



        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreation)
        {
            var sender = await _repo.GetUser(userId, true);
            /* If given userId is not matching to Token user. Then return Unauthorized */
            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }


            var recipient = await _repo.GetUser(messageForCreation.RecipientId, false);
            if (recipient == null)
            {
                return NotFound("Couldn't find Recipient.");
            }

            messageForCreation.SenderId = userId;

            var message = _mapper.Map<Message>(messageForCreation);

            _repo.Add(message);

            if (await _repo.SaveAll())
            {
                var messageForReturn = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new { userId, id = message.Id }, messageForReturn);
            }
            return BadRequest("Failed to send message");

        }

        [HttpGet]
        public async Task<IActionResult> GetMessageForUser(int userId, [FromQuery] MessageParams messageParams)
        {

            /* If given userId is not matching to Token user. Then return Unauthorized */
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageParams.UserId = userId;
            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

            var messagesToReturn = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize,
                                   messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messagesToReturn);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {

            /* If given userId is not matching to Token user. Then return Unauthorized */
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messagesFromRepo = await _repo.GetMessageThread(userId, recipientId);

            var messageToReturn = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            return Ok(messageToReturn);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int userId, int id)
        {
            /* If given userId is not matching to Token user. Then return Unauthorized */
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var message = await _repo.GetMessage(id);

            if (message.SenderId == userId)
            {
                message.SenderDeleted = true;
            }

            if (message.RecipientId == userId)
            {
                message.RecipientDeleted = true;
            }

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _repo.Delete(message);
            }

            if (await _repo.SaveAll())
            {
                return NoContent();
            }
            return BadRequest("Cannot delete the message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkRead(int userId, int id)
        {
            /* If given userId is not matching to Token user. Then return Unauthorized */
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var message = await _repo.GetMessage(id);

            if (message.RecipientId != userId)
            {
                return Unauthorized();
            }


            message.IsRead = true;
            message.DateRead = DateTime.Now;

            if (await _repo.SaveAll())
            {
                return NoContent();
            }
            return BadRequest("Cannot mark message read.");
        }


    }
}