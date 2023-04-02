using System;

namespace t4m.Providers.PronunciationData.DateTimeProviders;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now { get => DateTime.Now; }
}

