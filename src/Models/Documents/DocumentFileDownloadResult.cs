namespace EasierDocuware.Models.Documents
{
    public class DocumentFileDownloadResult
    {
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public long? ContentLength { get; set; }
        public Stream Stream { get; set; }
    }
}