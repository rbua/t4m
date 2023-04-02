using System;
namespace t4m.Models;

public class TranslationModel
{
    public string? Word { get; set; }
    public string[]? Meanings { get; set; }
    public int? Frequency { get; set; }
}