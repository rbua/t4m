using System;

namespace t4m.DTOs.TranslationInfoDTOs;

public class TranslationDTO
{
    public string? Word { get; set; }
    public List<string>? Meanings { get; set; }
    public int? Frequency { get; set; }
}
