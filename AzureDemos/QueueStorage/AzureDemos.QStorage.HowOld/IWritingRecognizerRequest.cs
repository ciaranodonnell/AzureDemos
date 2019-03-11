using System;

namespace AzureDemos.QStorage.WritingRecognizer
{
    public interface IWritingRecognizerRequest
    {
        byte[] Image { get; set; }
        string OriginLocation { get; set; }
        Guid RequestId { get; set; }
    }
}