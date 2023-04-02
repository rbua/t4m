using System;

namespace t4m.Providers.FileSystemProviders;

public class DirectoryProvider : IDirectoryProvider
{
    public bool Exists(string? path) => Directory.Exists(path);

    public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);
}
