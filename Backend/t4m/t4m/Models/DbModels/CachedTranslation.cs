using System;
using MongoDB.Bson;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace t4m.Models.DbModels;

public class CachedTranslation
{
    public ObjectId _id { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int TimesTranslated { get; set; }

    public string FromLanguage { get; set; }

    public string ToLanguage { get; set; }

    public Guid? PronunciationId { get; set; }

    public TextTranslationModel TextTranslation { get; set; }
}