using System;

namespace t4m.DTOs.TranslationInfoDTOs;

public class TranslationResponseInfoDTO
{
    public List<TranslationDefinitionDTO>? Definitions { get; set; }

    public List<string>? Examples { get; set; }

    public List<string>? Similar { get; set; }

    public List<ExtraTranslationDTO>? ExtraTranslations { get; set; }
}