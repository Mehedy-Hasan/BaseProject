using System.Runtime.CompilerServices;
using AutoMapper;
using UserApp.Dtos;
using UserApp.Models;

namespace UserApp.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserForRegistrationDto>();
            CreateMap<UserForRegistrationDto, User>();
            CreateMap<User, UserForDetailDto>();
            CreateMap<UserForDetailDto, User>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<User, UserForUpdateDto>();
            CreateMap<User, UserForReturnDto>();
            CreateMap<UserForReturnDto, User>();
        }
    }
}