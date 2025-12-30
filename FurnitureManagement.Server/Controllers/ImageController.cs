using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using System.Net.Http.Headers;

namespace FurnitureManagement.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ImageController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            try
            {
                // 验证文件
                if (image == null || image.Length == 0)
                {
                    return BadRequest(new { Success = false, Message = "未选择图片文件" });
                }

                // 验证文件类型
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp" };
                var fileExtension = Path.GetExtension(image.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { Success = false, Message = "只允许上传JPG、JPEG、PNG和BMP格式的图片" });
                }

                // 验证文件大小（最大5MB）
                if (image.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { Success = false, Message = "图片大小不能超过5MB" });
                }

                // 确保图片存储目录存在
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // 生成唯一文件名
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // 保存文件
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                // 返回成功响应
                return Ok(new { Success = true, Message = "图片上传成功", FileName = fileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "图片上传失败", Error = ex.Message });
            }
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        [HttpGet("{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { Success = false, Message = "图片不存在" });
                }

                // 获取文件MIME类型
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                var fileStream = System.IO.File.OpenRead(filePath);
                return File(fileStream, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "获取图片失败", Error = ex.Message });
            }
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        [HttpDelete("{fileName}")]
        public IActionResult DeleteImage(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok(new { Success = true, Message = "图片删除成功" });
                }
                else
                {
                    return NotFound(new { Success = false, Message = "图片不存在" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "删除图片失败", Error = ex.Message });
            }
        }
    }
}