using APItest.Data;
using APItest.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace APItest.Services.ImageService
{
    public class ImageService : IImageService
    {

        private readonly DataContext _context;
        private static readonly object pblock = new object();

        public ImageService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Models.Image>> GetAllImages()
        {
            var images = await _context.Images.ToListAsync();
            //_logger.LogInformation("It works!");
            return images;
        }

        public async Task<Models.Image> GetSingleImage(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image is null)
            {
                return null;
            }
            return image;
        }

        public async Task<Models.Image> AddImage(PathToImage pathToImage)
        {
            const string imageFormats = @"(?i)\.(png|jpg|webp|svg|tif)$";
            Models.Image image = new Models.Image();

            if (!Regex.IsMatch(pathToImage.Path, imageFormats))
            {
                return null;
            }

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(pathToImage.Path);
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

            
            image.Path = "http://localhost/" + UrlPrettier(pathToImage.Path);
            image.ImageInBytes = bytes;

            bool isWeightMoreThan5MB = bytes.Length > 5 * 1048576;
            if(isWeightMoreThan5MB)
            {
                return null;
            }
            lock (pblock)
            {
                _context.Images.Add(image);
            }
            await _context.SaveChangesAsync();
            return image;
        }

        private static string UrlPrettier(string url)
        {
            bool hasWWWSubdomain = url.Contains("www.");
            if (hasWWWSubdomain)
            {
                url = url.Replace("www.", "");
            }

            bool hasProtocolPrefix = url.Contains("https://");
            if(hasProtocolPrefix)
            {
                url = url.Replace("https://", "");
            }
            return url;
        }

        public async Task<Models.Image> CutImage(int id, int size)
        {
            var image = await _context.Images.FindAsync(id);
            if (image is null || size != 100 && size != 300)
            {
                return null;
            }
            image.ImageInBytes = Cut(image.ImageInBytes, size);

            return image;
        }
        public static byte[] Cut(byte[] imageData, int size)
        {
            MemoryStream ms = new MemoryStream(imageData);
            Bitmap bitmap = new Bitmap(ms);
            Bitmap resizedBitmap = new Bitmap(size, size);

            Graphics graphics = Graphics.FromImage(resizedBitmap);
            graphics.InterpolationMode = InterpolationMode.Bilinear;
            graphics.DrawImage(bitmap, 0, 0, size, size);

            MemoryStream ms2 = new MemoryStream();
            resizedBitmap.Save(ms2, bitmap.RawFormat);
            imageData = ms2.ToArray();
            return imageData;
        }

        public async Task<List<Models.Image>> RemoveImage(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image is null)
            {
                return null;
            }

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            return await _context.Images.ToListAsync();
        }
    }
}
