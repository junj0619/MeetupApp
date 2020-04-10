using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MeetupApp.API.Data;
using MeetupApp.API.Dtos;
using MeetupApp.API.Helpers;
using MeetupApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MeetupApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users/{userId}/photos")]
    public class PhotosController : ControllerBase
    {
        private readonly IMeetupRepository _meetupRepository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly Cloudinary _cloudinary;

        public PhotosController(IMeetupRepository meetupRepository, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._meetupRepository = meetupRepository;
            this._mapper = mapper;
            this._cloudinaryConfig = cloudinaryConfig;

            Account account = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto photoForCreation)
        {
            /* If given userId is not matching to Token user. Then return Unauthorized */
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await _meetupRepository.GetUser(userId);
            var file = photoForCreation.File;
            var uploadResult = new ImageUploadResult();

            /* Upload photo to cloudinary */
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var imagicUploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(imagicUploadParams);
                };

                photoForCreation.Url = uploadResult.Uri.ToString();
                photoForCreation.PublicId = uploadResult.PublicId;
            }

            var photo = _mapper.Map<Photo>(photoForCreation);
            /* If there is no Main photo yet, set up current uploaded photo as main */
            if (!userFromRepo.Photos.Any(p => p.IsMain))
            {
                photo.IsMain = true;
            }

            /* Save photo information to DB */
            userFromRepo.Photos.Add(photo);
            if (await _meetupRepository.SaveAll())
            {
                var photoForReturn = _mapper.Map<PhotoForReturnDto>(photo);

                /*return created at route (Route Name, Route Values, Value) */
                return CreatedAtRoute("GetPhoto", new { UserId = userId, Id = photo.Id }, photoForReturn);
            }
            return BadRequest("Could not add the photo.");
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photo = await _meetupRepository.GetPhoto(id);
            return Ok(photo);
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            /* If given userId is not matching to Token user. Then return Unauthorized */
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            /* Check user contains given photoId */
            var user = await _meetupRepository.GetUser(userId);
            if (!user.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }

            /* Check targeting photo already set to Main */
            var photoFromRepo = await _meetupRepository.GetPhoto(id);
            if (photoFromRepo.IsMain)
            {
                return BadRequest("This ia already the main photo.");
            }

            /* 
               1) Get current user's main photo 
               2) Set the photo main flag to FALSE 
               3) Set new photo main flag to TRUE
            */
            var currentMainPhoto = await _meetupRepository.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await _meetupRepository.SaveAll())
            {
                return NoContent();
            }
            return BadRequest("Could not set photo to main");
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            /* If given userId is not matching to Token user. Then return Unauthorized */
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            /* Check user contains given photoId */
            var user = await _meetupRepository.GetUser(userId);
            if (!user.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }

            /* Stop deleting main photo */
            var photoFromRepo = await _meetupRepository.GetPhoto(id);
            if (photoFromRepo.IsMain)
            {
                return BadRequest("Cannot delete main photo.");
            }

            /* Delete photo from cloudinary */
            if (photoFromRepo.PublicId != null)
            {
                var delParam = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(delParam);
                if (result.Result == "ok")
                {
                    /* Delete photo from DB */
                    user.Photos.Remove(photoFromRepo);
                }
            }
            else /* delete photo that only in DB not in cloudinary */
            {
                /* Delete photo from DB */
                user.Photos.Remove(photoFromRepo);
            }

            if (await _meetupRepository.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Could not delete photo");

        }

    }
}
