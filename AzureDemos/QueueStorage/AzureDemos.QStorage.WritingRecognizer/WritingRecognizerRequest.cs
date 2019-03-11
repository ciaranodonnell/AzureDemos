using System;
using System.Runtime.Serialization;

namespace AzureDemos.QStorage.WritingRecognizer
{
    [DataContract]
    public class WritingRecognizerRequest : IWritingRecognizerRequest
    {
        [DataMember]
        public Guid RequestId { get; set; }
        [DataMember]
        public string OriginLocation { get; set; }
        [DataMember]
        public byte[] Image { get; set; }
    }
}