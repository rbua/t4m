using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using t4m.Services;

namespace t4m.Controllers;

[ApiController]
[Route("[controller]")]
public class TranslateController : ControllerBase
{
    private ILogger<TranslateController> _logger;
    private ITranslationService _translateService;

    public TranslateController(ILogger<TranslateController> logger,
        ITranslationService translateService)
    {
        _logger = logger;
        _translateService = translateService;
    }

    [HttpPost("{fromLanguage}/{toLanguage}/{text}")]
    public async Task<IActionResult> GetTranslation(string fromLanguage, string toLanguage, string text)
    {
        // TODO: add validation for fromLanguage and toLanguage. values should be in list of allowed languages
        return Ok(await _translateService.Translate(fromLanguage, toLanguage, text));
    }
}
