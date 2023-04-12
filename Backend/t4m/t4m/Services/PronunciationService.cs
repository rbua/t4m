using System;
using System.Text.Json;
using t4m.DTOs;
using t4m.DTOs.TranslationInfoDTOs;
using t4m.Models;
using System.IO;
using System.Text;
using t4m.Helpers;
using AutoMapper;
using System.Web;
using System.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.Mime.MediaTypeNames;
using t4m.Repository;

namespace t4m.Services;

public class PronunciationService : IPronunciationService
{
    private readonly IPronunciationCacheRepository _pronunciationService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMapper _mapper;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public PronunciationService(IPronunciationCacheRepository pronunciationService,
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            JsonSerializerOptions jsonSerializerOptions)
    {
        _pronunciationService = pronunciationService;
        _httpClientFactory = httpClientFactory;
        _mapper = mapper;
        _jsonSerializerOptions = jsonSerializerOptions;
    }


    public async Task<PronunciationModel?> GetPronunciationById(Guid pronunciationId)
    {
        var audioData = await _pronunciationService.GetPronunciationAudioData(pronunciationId);

        return new PronunciationModel
        {
            CacheRecordId = pronunciationId,
            AudioData = audioData
        };
    }


    public async Task<PronunciationModel?> GetNewPronunciation(string language, string textToPronunciate, bool? saveToCache = null)
    {
        var pronunciation = new PronunciationModel();

        if (string.IsNullOrEmpty(textToPronunciate))
        {
            return null;
        }

        saveToCache ??= IsPronunciationCachedByDefault(textToPronunciate);

        var audioData = await GetAudioDataAsync(language, textToPronunciate);

        if (audioData?.Audio == null)
        {
            throw new ApplicationException($"{nameof(audioData.Audio)} were null");
        }

        pronunciation.AudioData = NumericTypesHelper.ConvertArray(audioData.Audio);

        if (saveToCache == true)
        {
            pronunciation.CacheRecordId = await _pronunciationService.SavePronunciationAudioData(NumericTypesHelper.ConvertArray(audioData.Audio));
        }

        return pronunciation;
    }


    protected bool IsPronunciationCachedByDefault(string textToPronunciate)
    {
        // TODO: Try to use regex here and analyze it's performace. Maybe it's gonna be better.

        if (string.IsNullOrEmpty(textToPronunciate))
        {
            throw new ArgumentException($"{nameof(textToPronunciate)} is null of empty.");
        }

        textToPronunciate = textToPronunciate.Trim();

        bool containsTwoOrMoreDots = textToPronunciate.Split('.').Length >= 3;
        bool containsFiveOrMoreSpaces = textToPronunciate.Split(' ').Length >= 6;
        bool isLongerThanSeventyCharacters = textToPronunciate.Length >= 70;

        var isTextSimple = new[] { containsTwoOrMoreDots, containsFiveOrMoreSpaces, isLongerThanSeventyCharacters }
            .All(x => x == false);

        return isTextSimple;
    }


    private async Task<PronunciationDto?> GetAudioDataAsync(string language, string textToPronunciate)
    {
        var requestUrl = string.Format(Constants.API.GetPronunciationUrl, language, textToPronunciate);

        var client = _httpClientFactory.CreateClient(Constants.API.DefaultHttpClientName);
        var httpResponse = await client.GetAsync(requestUrl);
        httpResponse.EnsureSuccessStatusCode();

        var responseContent = await httpResponse.Content.ReadAsStringAsync();
        PronunciationDto? translatedResponseDto = JsonSerializerHelper.Deserialize<PronunciationDto>(responseContent, _jsonSerializerOptions);

        return translatedResponseDto;
    }
}