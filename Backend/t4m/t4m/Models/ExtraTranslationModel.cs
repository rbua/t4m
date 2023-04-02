using System;
using t4m.DTOs.TranslationInfoDTOs;

namespace t4m.Models;

public class ExtraTranslationModel
{
    public string? Type { get; set; }
    public List<TranslationModel>? List { get; set; }
}