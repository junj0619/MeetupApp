using System;
using System.ComponentModel.DataAnnotations;

namespace MeetupApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Gender { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "You must specify password between 6 and 8 characters")]
        public string Password { get; set; }

        [Required]
        public string KnownAs { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public UserForRegisterDto()
        {
            this.Created = DateTime.Now;
            this.LastActive = DateTime.Now;
        }

    }
}