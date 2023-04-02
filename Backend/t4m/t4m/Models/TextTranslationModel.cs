using System;
namespace t4m.Models;

public class TextTranslationModel
{
    public string Text { get; set; }

    public string Translation { get; set; }

    /// <summary>
    /// Null if pronunciation is not cached.
    /// </summary>
    public Guid? PronunciationCacheRecordId { get; set; }

    public TranslationDefinitionModel[]? TranslationDefinitions { get; set; }

    public string[]? Examples { get; set; }

    public string[]? Similar { get; set; }

    public ExtraTranslationModel[]? ExtraTranslations { get; set; }
}
