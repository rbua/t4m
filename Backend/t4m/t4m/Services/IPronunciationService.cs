using System;
using t4m.DTOs;
using t4m.Models;

namespace t4m.Services;

public interface IPronunciationService
{
    Task<PronunciationModel?> GetPronunciationById(Guid pronunciationId);

    Task<PronunciationModel?> GetNewPronunciation(string language, string? textToPronunciate, bool? saveToCache = null);
}
