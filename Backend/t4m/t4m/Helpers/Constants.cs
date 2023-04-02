using System;

namespace t4m.Helpers;

public static class Constants
{
    public static class Appsettings
    {
        public static string DatabaseNameKey { get => "Mongo:DatabaseName"; }
        public static string ConnectionStringKey { get => "Mongo:ConnectionString"; }
        public static string TranslatorApiConfigKey { get => "TranslatorApiConfig"; }
        public static string TranslatorApiBaseApi { get => "TranslatorApiConfig:BaseApi"; }
    }

    public static class Database
    {
        public static string TranslationsCollectionName { get => "translations"; }
    }

    public static class FileSystem
    {
        public static string PronunciationRootFolderName { get => "pronunciation_cache"; }
    }

    public static class API
    {
        public static string DefaultHttpClientName { get => "translationApiHttpClient"; }
        public static string GetPronunciationUrl { get => "api/v1/audio/{0}/{1}"; }
        public static string GetTranslationUrl { get => "api/v1/{0}/{1}/{2}"; }
    }
}
