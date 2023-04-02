using System;

namespace t4m.Providers.PronunciationData.DateTimeProviders;

public interface IDateTimeProvider
{
    DateTime Now { get; }
}
