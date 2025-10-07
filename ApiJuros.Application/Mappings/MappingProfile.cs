using ApiJuros.Application.DTOs;
using AutoMapper;

namespace ApiJuros.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<InvestmentInput, InvestmentOutput>()
                .ForMember(dest => dest.InvestedAmount, opt => opt.MapFrom(src => src.InitialValue))
                .ForMember(dest => dest.FinalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.TotalInterest, opt => opt.Ignore());
        }
    }
}