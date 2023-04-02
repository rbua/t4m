using System;

namespace t4m.Providers.GuidProviders;

public interface IGuidProvider
{
    Guid NewGuid();
}
