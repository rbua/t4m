using System;

namespace t4m.Repository;

public interface IPronunciationCacheRepository
{
    Task<byte[]> GetPronunciationAudioData(Guid pronunciationId);

    Task<Guid> SavePronunciationAudioData(byte[] pronunciationAudioData);
}
