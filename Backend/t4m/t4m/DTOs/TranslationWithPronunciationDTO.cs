using System;
using t4m.DTOs.TranslationInfoDTOs;

namespace t4m.DTOs;

public class TranslationWithPronunciationDTO
{
    public TranslationResponseDTO? Translation { get; set; }

    public PronunciationDto? Pronunciation { get; set; }
}
