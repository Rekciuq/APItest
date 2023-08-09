using APItest.Services.ImageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace APItest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        public async Task<ActionResult<List<Image>>> GetAllImages()
        {
            return await _imageService.GetAllImages();
        }

        [Route("get-url/{id}")]
        public async Task<ActionResult<Image>> GetSingleImage(int id)
        {
            var result = await _imageService.GetSingleImage(id);
            if (result is null)
            {
                return NotFound("Id that you passed is not exist in Database");
            }
            string json = "{ Path: " + result.Path + " }";
            return Ok("200 ok " + json);
        }

        [Route("upload-by-url")]
        public async Task<ActionResult<Image>> AddImage(PathToImage pathToImage)
        {
            var result = await _imageService.AddImage(pathToImage);
            if(result is null)
            {
                return BadRequest("Check if you passed on image that less then 5MB");
            }
            string json = "{ Path: " + result.Path + " }";
            return Ok("200 ok " + json);
        }

        [Route("get-url/{id}/size/{size}")]
        public async Task<ActionResult<Image>> CutImage(int id, int size)
        {
            var result = await _imageService.CutImage(id, size);
            if (result is null)
            {
                return NotFound("Id that you passed is not exist in Database");
            }
            
            return Ok(result.Path + "/" + size + "x" + size);
        }

        [Route("remove/{id}")]
        public async Task<ActionResult<List<Image>>> RemoveImage(int id)
        {
            var result = await _imageService.RemoveImage(id);
            if (result is null)
                return NotFound("Id that you passed is not exist in Database");
            return Ok("200 ok");
        }

        
    }
}
