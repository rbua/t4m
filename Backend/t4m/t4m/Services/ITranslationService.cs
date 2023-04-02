using System;
using t4m.DTOs;

namespace t4m.Services;

public interface ITranslationService
{
    Task<TranslationWithPronunciationDTO> Translate(string fromLanguage, string toLanguage, string textToTranslate);
}