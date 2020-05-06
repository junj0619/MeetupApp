using System;
using System.Linq;
using AutoMapper;
using MeetupApp.API.Dtos;
using MeetupApp.API.Models;

namespace MeetupApp.API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserForDetailDto>()
            .ForMember(dest => dest.PhotoUrl,
                       option => option.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(dest => dest.Age,
                       option => option.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<User, UserForListDto>()
            .ForMember(dest => dest.PhotoUrl,
                       option => option.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(dest => dest.Age,
                       option => option.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<Photo, PhotoForDetailDto>();

            CreateMap<UserForUpdateDto, User>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap();


            CreateMap<Message, MessageToReturnDto>()
            .ForMember(dest => dest.SenderPhotoUrl,
                       option => option.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(dest => dest.RecipientPhotoUrl,
                       option => option.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}