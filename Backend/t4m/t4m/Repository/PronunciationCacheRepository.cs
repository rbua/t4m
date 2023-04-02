using System;
using System.Collections;
using SharpCompress.Common;
using t4m.Helpers;
using t4m.Providers.FileSystemProviders;
using t4m.Providers.GuidProviders;

namespace t4m.Repository;

/// <summary>
//
// This class manages the pronunciation cache that is stored on disk. The cache has
// a root folder with nested folders inside it. The names of the nested folders are
// based on the first two letters of the pronunciation ID. These nested folders are
// necessary for efficient OS indexing. Pronunciation IDs are GUID strings with a
// '.bin' extension that store pronunciation audio data.
//
// Folder structure example:
//
// pronunciation_cache/
// ├── 36/
// │   ├── 3622ad9e-1f41-43f5-b950-103015299383.bin
// │   └── ...
// ├── d6/
// │   ├── d670d2e4-5088-40e7-b6a1-2571af323528.bin
// │   ├── d6923c58-2610-44fe-8984-b4963c3cddd3.bin
// │   └── ...
// ├── 7d/
// │   ├── 7d4e46f2-74ac-48a7-9567-5165421a7223.bin
// │   └── ...
// └── bf/
//     ├── bfe37c83-2c14-46b7-9e70-b57282fb2a9f.bin
//     └── ...
//
/// </summary>
public class PronunciationCacheRepository : IPronunciationCacheRepository
{
    private IGuidProvider _guidProvider;
    private ILogger<PronunciationCacheRepository> _logger;
    private IDirectoryProvider _directoryProvider;
    private IFileProvider _fileProvider;

    public PronunciationCacheRepository(IGuidProvider guidProvider,
        ILogger<PronunciationCacheRepository> logger,
        IDirectoryProvider directoryProvider,
        IFileProvider fileProvider)
    {
        _guidProvider = guidProvider;
        _logger = logger;
        _directoryProvider = directoryProvider;
        _fileProvider = fileProvider;
    }

    public async Task<byte[]> GetPronunciationAudioData(Guid pronunciationId)
    {
        var cacheFilePath = GetCacheFileFullPath(pronunciationId);

        if (!_fileProvider.Exists(cacheFilePath))
        {
            var errorMessage = $"File with path: '{cacheFilePath}' does not exist.";
            _logger.LogError(errorMessage);
            throw new IOException(errorMessage);
        }

        byte[] fileBytes = await File.ReadAllBytesAsync(cacheFilePath);
        return fileBytes;
    }

    public async Task<Guid> SavePronunciationAudioData(byte[] pronunciationAudioData)
    {
        var pronunciationId = _guidProvider.NewGuid();
        var prononciationCacheFolder = pronunciationId.ToString().Substring(0, 2);

        EnsureRootPronunciationCacheFolderExist();
        EnsureCacheFolderExist(prononciationCacheFolder);
        await CreateCacheFile(pronunciationAudioData, pronunciationId);

        return pronunciationId;
    }

    private async Task CreateCacheFile(byte[] pronunciationAudioData, Guid pronunciationId)
    {
        string fullPath = GetCacheFileFullPath(pronunciationId);

        if (pronunciationAudioData == null || pronunciationAudioData.Length == 0)
        {
            _logger.LogInformation($"Pronunciation cache file is empty {pronunciationId}");

            return;
        }

        if (_fileProvider.Exists(fullPath))
        {
            var errorMessage = $"{fullPath} file already exist.";
            _logger.LogWarning(errorMessage);
            throw new IOException(errorMessage);

            return;
        }

        try
        {
            using (var fileStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write))
            {
                await fileStream.WriteAsync(pronunciationAudioData, 0, pronunciationAudioData.Length);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during file creation: " + ex.Message);
        }
    }

    private void EnsureCacheFolderExist(string prononciationFolderName)
    {
        var folderPath = Path.Combine(Constants.FileSystem.PronunciationRootFolderName, prononciationFolderName);

        if (!_directoryProvider.Exists(folderPath))
        {
            var prononciationFolder = _directoryProvider.CreateDirectory(folderPath);

            _logger.LogInformation($"Pronunciation cache folder created at {prononciationFolder.FullName}");
        }
    }

    private void EnsureRootPronunciationCacheFolderExist()
    {
        if (!_directoryProvider.Exists(Constants.FileSystem.PronunciationRootFolderName))
        {
            var pronunciationRootFolder = _directoryProvider.CreateDirectory(Constants.FileSystem.PronunciationRootFolderName);

            _logger.Log(LogLevel.Information, $"Root pronunciation cache folder created at {pronunciationRootFolder.FullName}");
        }
    }

    private string GetCacheFileFullPath(Guid pronunciationId)
    {
        var fileName = $"{pronunciationId.ToString()}.bin";
        var prononciationCacheFolder = pronunciationId.ToString().Substring(0, 2);
        string fullPath = Path.Combine(Constants.FileSystem.PronunciationRootFolderName, prononciationCacheFolder, fileName);

        return fullPath;
    }
}