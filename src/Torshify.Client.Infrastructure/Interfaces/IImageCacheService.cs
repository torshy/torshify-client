namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IImageCacheService
    {
        IImageCacheEntry GetImage(IImage image, int decodeWidth = 300, int decodeHeight = 300);
    }
}