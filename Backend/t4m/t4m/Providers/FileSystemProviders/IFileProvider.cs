using System;

namespace t4m.Providers.FileSystemProviders;

public interface IFileProvider
{
    bool Exists(string path);
}
