using Microsoft.AspNetCore.Mvc;

namespace APItest.Services.ImageService
{
    public interface IImageService
    {
        Task<List<Image>> GetAllImages();
        Task<Image> GetSingleImage(int id);
        Task<Image> AddImage(PathToImage pathToImage);
        Task<Image> CutImage(int id, int size);
        Task<List<Image>> RemoveImage(int id);
    }
}
