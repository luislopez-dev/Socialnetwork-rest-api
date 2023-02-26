using AutoMapper;
using datingApp.Data;
using datingApp.DTOs;
using datingApp.Entities;
using datingApp.Extensions;

namespace datingApp.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => 
            src.Photos.FirstOrDefault(x => x.IsMain).Url))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDto, AppUser>();
        CreateMap<RegisterDto, AppUser>();
        CreateMap<Message, MessageDto>()
            .ForMember(dto => dto.SenderPhotoUrl,
                options => options.MapFrom(s => s.Sender.Photos
                    .FirstOrDefault(x => x.IsMain).Url));
    }
}