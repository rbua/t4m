using System;

namespace t4m.DTOs.TranslationInfoDTOs;

public class TranslationResponseDTO
{
    public string? Translation { get; set; }
    public TranslationResponseInfoDTO? Info { get; set; }
}
