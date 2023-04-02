using System;

namespace t4m.Models.Configuration;

public class ConfigurationModel
{
    // TODO: If something from config is null - should throw exception. Check do clr does not throws exception by it's own.

    public string BaseApi { get; set; }
}
