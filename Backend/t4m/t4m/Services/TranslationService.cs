using System;
using System.Text.Json;
using System.Transactions;
using System.Web;
using AutoMapper;
using t4m.DTOs;
using t4m.DTOs.TranslationInfoDTOs;
using t4m.Models;
using t4m.Repository;
using static System.Net.Mime.MediaTypeNames;

namespace t4m.Services;

public class TranslationService : ITranslationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IPronunciationService _pronunciationService;
    private readonly ITranslationRepository _translationRepository;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IMapper _mapper;

    public TranslationService(IPronunciationService pronunciationService,
        ITranslationRepository translationRepository,
        JsonSerializerOptions jsonSerializerOptions,
        IMapper mapper)
    {
        _pronunciationService = pronunciationService;
        _translationRepository = translationRepository;
        _jsonSerializerOptions = jsonSerializerOptions;
        _mapper = mapper;
    }

    public async Task<TranslationWithPronunciationDTO> Translate(string fromLanguage, string toLanguage, string textToTranslate)
    {
        if (string.IsNullOrWhiteSpace(textToTranslate))
        {
            throw new ArgumentException($"{nameof(textToTranslate)} is empty of null");
        }

        TextTranslationModel translation = await GetTextTranslation(fromLanguage, toLanguage, textToTranslate);
        PronunciationModel? pronunciation = await GetTextPronunciation(translation, toLanguage);

        await _translationRepository.CacheTranslation(fromLanguage, toLanguage,
            pronunciation?.CacheRecordId, translation);

        var translationWithPronunciationDto = MapToTranslationWithPronunciationDTO(translation, pronunciation);

        return translationWithPronunciationDto;
    }

    private async Task<TextTranslationModel> GetTextTranslation(string fromLanguage, string toLanguage, string textToTranslate)
    {
        TextTranslationModel? translation = null;

        var cachedTranslation = await _translationRepository.TryGetCachedTranslation(fromLanguage, toLanguage, textToTranslate);
        if (cachedTranslation != null)
        {
            translation = cachedTranslation.TextTranslation;
        }
        else
        {
            translation = await _translationRepository.GetNewTranslation(fromLanguage, toLanguage, textToTranslate);
        }

        if (translation == null)
        {
            throw new ApplicationException("Translation is not available.");
        }

        return translation;
    }

    private async Task<PronunciationModel?> GetTextPronunciation(TextTranslationModel translation, string language)
    {
        PronunciationModel? pronunciation = null;

        if (translation.PronunciationCacheRecordId.HasValue)
        {
            pronunciation = await _pronunciationService.GetPronunciationById(translation.PronunciationCacheRecordId.Value);
        }
        else
        {
            pronunciation = await _pronunciationService.GetNewPronunciation(language, translation?.Translation);
        }

        return pronunciation;
    }

    private TranslationWithPronunciationDTO MapToTranslationWithPronunciationDTO(TextTranslationModel textTranslationModel, PronunciationModel? pronunciation)
    {
        PronunciationDto? pronunciationDto = null;

        var translationDto = new TranslationResponseDTO
        {
            Translation = textTranslationModel.Translation,
            Info = _mapper.Map<TranslationResponseInfoDTO>(textTranslationModel)
        };

        if (pronunciation != null)
        {
            pronunciationDto = _mapper.Map<PronunciationDto>(pronunciation);
        }

        return new TranslationWithPronunciationDTO
        {
            Translation = translationDto,
            Pronunciation = pronunciationDto
        };
    }
}