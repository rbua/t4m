using System;
using AutoMapper;
using t4m.DTOs;
using t4m.DTOs.TranslationInfoDTOs;
using t4m.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace t4m.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TranslationResponseInfoDTO, TextTranslationModel>()
            //.ForMember(dest => dest.ExtraTranslations, opt => opt.MapFrom(src => src.ExtraTranslations.ToArray()))
            .ForMember(dest => dest.TranslationDefinitions, opt => opt.MapFrom(src => src.Definitions));

        CreateMap<TextTranslationModel, TranslationResponseInfoDTO>()
            //.ForMember(dest => dest.ExtraTranslations, opt => opt.MapFrom(src => src.ExtraTranslations.ToList()))
            .ForMember(dest => dest.Definitions, opt => opt.MapFrom(src => src.TranslationDefinitions));

        CreateMap<TranslationDefinitionDTO, TranslationDefinitionModel>();
        CreateMap<TranslationDefinitionModel, TranslationDefinitionDTO>();

        CreateMap<ExtraTranslationDTO, ExtraTranslationModel>();
        CreateMap<ExtraTranslationModel, ExtraTranslationDTO>();

        CreateMap<PronunciationDto, PronunciationModel>()
            .ForMember(dest => dest.CacheRecordId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AudioData, opt => opt.MapFrom(src => src.Audio));
        CreateMap<PronunciationModel, PronunciationDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CacheRecordId))
            .ForMember(dest => dest.Audio, opt => opt.MapFrom(src => src.AudioData));

        CreateMap<TranslationDTO, TranslationModel>();
        CreateMap<TranslationModel, TranslationDTO>();
    }
}