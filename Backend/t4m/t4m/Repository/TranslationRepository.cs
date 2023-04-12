using System;
using System.Text;
using System.Text.Json;
using System.Transactions;
using AutoMapper;
using MongoDB.Driver;
using t4m.DTOs;
using t4m.DTOs.TranslationInfoDTOs;
using t4m.Helpers;
using t4m.Models;
using t4m.Models.DbModels;
using t4m.Providers.PronunciationData.DateTimeProviders;
using static System.Net.Mime.MediaTypeNames;

namespace t4m.Repository;

public class TranslationRepository : ITranslationRepository
{
    public string CollectionName { get => "Translations"; }

    private readonly IMongoCollection<CachedTranslation> _cachedTranslationCollection;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IMapper _mapper;

    public TranslationRepository(IMongoDatabase database,
        IDateTimeProvider dateTimeProvider,
        IHttpClientFactory httpClientFactory,
        JsonSerializerOptions jsonSerializerOptions,
        IMapper mapper)
    {
        _cachedTranslationCollection = database.GetCollection<CachedTranslation>(Constants.Database.TranslationsCollectionName);
        _dateTimeProvider = dateTimeProvider;
        _httpClient = httpClientFactory.CreateClient(Constants.API.DefaultHttpClientName);
        _jsonSerializerOptions = jsonSerializerOptions;
        _mapper = mapper;
    }

    public async Task<TextTranslationModel?> GetNewTranslation(string fromLanguage, string toLanguage, string text)
    {
        var getTranslationUrl = string.Format(Constants.API.GetTranslationUrl, fromLanguage, toLanguage, text);
        var httpResponse = await _httpClient.GetAsync(getTranslationUrl);

        httpResponse.EnsureSuccessStatusCode();
        var responseContent = await httpResponse.Content.ReadAsStringAsync();
        var translatedResponseInfoDto = JsonSerializerHelper.Deserialize<TranslationResponseDTO>(responseContent, _jsonSerializerOptions);

        var translatedModel = _mapper.Map<TextTranslationModel>(translatedResponseInfoDto.Info);
        translatedModel.Text = text;
        translatedModel.Translation = translatedResponseInfoDto.Translation;

        return translatedModel;
    }

    public async Task<CachedTranslation?> TryGetCachedTranslation(string fromLanguage, string toLanguage, string text)
    {
        text = text.Normalize(NormalizationForm.FormD);

        var cachedTranslations = await _cachedTranslationCollection.Find(x => x.FromLanguage == fromLanguage &&
            x.ToLanguage == toLanguage &&
            x.TextTranslation.Text == text)
            .ToListAsync();

        if (!cachedTranslations.Any())
        {
            return null;
        }

        var cachedTranslation = cachedTranslations.Single();
        await IncrementTimeTranslatedCounter(cachedTranslation);

        return cachedTranslation;
    }

    public async Task CacheTranslation(string fromLanguage,
        string toLanguage,
        Guid? pronunciationId,
        TextTranslationModel translation)
    {
        var alreadyCachedTranslations = await _cachedTranslationCollection.Find(x => x.FromLanguage == fromLanguage &&
            x.ToLanguage == toLanguage &&
            x.TextTranslation.Text == translation.Translation)
            .ToListAsync();

        if(alreadyCachedTranslations.Any())
        {
            return;
        }

        var cachedTranslation = new CachedTranslation
        {
            CreatedAt = _dateTimeProvider.Now,
            FromLanguage = fromLanguage,
            ToLanguage = toLanguage,
            TextTranslation = translation,
            TimesTranslated = 1,
            PronunciationId = pronunciationId
        };

        _cachedTranslationCollection.InsertOne(cachedTranslation);
    }

    /// <summary>
    /// TimeTranslated counter is used for analythical purposes.
    /// Also it's usefull to delete big cached strings and pronunciations
    /// that used to be translated only once to save disc space.
    /// </summary>
    private async Task IncrementTimeTranslatedCounter(CachedTranslation cachedTranslation)
    {
        var filter = Builders<CachedTranslation>.Filter.Eq(x => x._id, cachedTranslation._id);
        var timesTranslatedIncrement = Builders<CachedTranslation>.Update.Set(x => x.TimesTranslated, cachedTranslation.TimesTranslated + 1);

        await _cachedTranslationCollection.UpdateOneAsync(filter, timesTranslatedIncrement);
    }
}