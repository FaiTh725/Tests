namespace Test.Application.Contracts.File
{
    public class FileModel
    {
        public string Name { get; set; } = string.Empty;

        public required Stream Stream { get; set; }

        public string ContentType { get; set; } = string.Empty;
    }
}
