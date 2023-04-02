using System;

namespace t4m.Providers.FileSystemProviders;

public class FileProvider : IFileProvider
{
    public bool Exists(string path) =>
        File.Exists(path);
}
