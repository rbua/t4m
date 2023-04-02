using System;

namespace t4m.Providers.GuidProviders;

public class GuidProvider : IGuidProvider
{
    public Guid NewGuid() => Guid.NewGuid();
}
