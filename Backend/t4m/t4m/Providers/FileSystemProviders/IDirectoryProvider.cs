using System;

namespace t4m.Providers.FileSystemProviders;

public interface IDirectoryProvider
{
    bool Exists(string? path);

    DirectoryInfo CreateDirectory(string path);
}
