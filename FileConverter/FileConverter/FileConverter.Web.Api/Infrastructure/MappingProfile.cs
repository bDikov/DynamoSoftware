namespace FileConverter.Web.Api.Infrastructure
{
    using AutoMapper;
    using FileConverter.Domain.Models;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IFormFile, FileData>()
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.ContentType))
                .ForMember(dest => dest.Length, opt => opt.MapFrom(src => src.Length))
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.OpenReadStream()));
        }
    }
}
