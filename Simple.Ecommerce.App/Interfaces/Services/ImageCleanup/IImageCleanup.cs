namespace Simple.Ecommerce.App.Interfaces.Services.ImageCleanup
{
    public interface IImageCleanup
    {
        Task<int> RemoveImages(List<string> imageNames);
        string RepositoryName { get; }
    }
}
