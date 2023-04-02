using System;
using t4m.Models;
using t4m.Models.DbModels;

namespace t4m.Repository;

public interface ITranslationRepository
{
    Task<TextTranslationModel?> GetNewTranslation(string fromLanguage, string toLanguage, string text);

    Task<CachedTranslation?> TryGetCachedTranslation(string fromLanguage, string toLanguage, string text);

    Task CacheTranslation(string fromLanguage, string toLanguage, Guid? pronunciationId, TextTranslationModel translation);
}
