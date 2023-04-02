using System;

namespace t4m.DTOs.TranslationInfoDTOs;

public class TranslationDefinitionDTO
{
    public string? Definition { get; set; }
    public string? Example { get; set; }
    public List<string>? Synonyms { get; set; }
}

