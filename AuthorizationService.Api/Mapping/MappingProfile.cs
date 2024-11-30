using AuthorizationService.Api.Dtos;
using AuthorizationService.Domain.Entities;
using AutoMapper;

namespace AuthorizationService.Api.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, ShortUserModel>();
        CreateMap<RegisterModel, User>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        CreateMap<LoginModel, User>();
    }
}