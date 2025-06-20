using AutoMapper;
using DapperPrac.Dtos;
using DapperPrac.Models;

namespace DapperPrac.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username)) // absolutely redundant btw as both properties(in src and dest) have the same name
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)); // also redundant

            CreateMap<User,UserDto>().ReverseMap();
        }
    }
}
