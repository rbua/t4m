using System;

namespace t4m.Models;

public class PronunciationModel
{
    public Guid CacheRecordId { get; set; }

    public byte[]? AudioData { get; set; }
}
