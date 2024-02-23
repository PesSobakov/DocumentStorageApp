namespace DocumentStorageApp.Services
{
    public interface IAzureStorage
    {
        public Task UploadAsync(string name, Stream content,string email);
    }
}
