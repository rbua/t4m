using System;
using System.Transactions;

namespace t4m.DTOs.TranslationInfoDTOs;

public class ExtraTranslationDTO
{
    public string? Type { get; set; }
    public List<TranslationDTO>? List { get; set; }
}