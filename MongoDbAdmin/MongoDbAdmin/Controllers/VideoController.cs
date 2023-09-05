using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using MongoDbAdmin.Models;
using MongoDbAdmin.Services;
using Microsoft.Extensions.Options;

namespace MongoDbAdmin.Controllers
{

    /// <summary>
    /// 视频后台管理Api
    /// </summary>
    [Route("api/videos")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly GridFSService _gridFSService;
        private readonly IMongoCollection<Video> _videoCollection;
        private readonly MongoSettings _mongoSettings;

        public VideoController(GridFSService gridFSService, IMongoDatabase database, IOptions<MongoSettings>  mongoSettings)
        {
            _gridFSService = gridFSService;
            _videoCollection = database.GetCollection<Video>("Video");
            _mongoSettings = mongoSettings.Value;

            //string connectionString = _mongoSettings.ConnectionString;
            //string databaseName = _mongoSettings.DatabaseName;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadVideoOrImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("请提供正确的图像或视频文件。");
            }

            var fileId = await _gridFSService.UploadVideoOrImageAsync(file.OpenReadStream(), file.FileName, file.ContentType);

            // 保存图像的元数据或其他信息到数据库
            // 这里可以根据需要执行操作，例如保存视频的URL等信息
            var video = new Video
            {
                Title = file.FileName,
                Description = "此处为视频描述", // 您可以根据您的要求添加描述
                FilePath = fileId
            };

            await _videoCollection.InsertOneAsync(video);

            return Ok("Video uploaded successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> GetVideos()
        {
            var videos = await _videoCollection.Find(_ => true).ToListAsync();
            return Ok(videos);
        }

        [HttpGet("{id}")]
        public IActionResult DownloadVideo(string id)
        {
            var videoStream = _gridFSService.DownloadVideoOrImage(id);
            if (videoStream == null)
            {
                return NotFound("找不到视频");
            }

            return File(videoStream, "video/mp4"); // Adjust the content type based on the video format.
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(string id)
        {
            var video = await _videoCollection.FindOneAndDeleteAsync(v => v.Id == id);
            if (video == null)
            {
                return NotFound("找不到视频");
            }

            await _gridFSService.DeleteVideoOrImageAsync(video.FilePath);

            return Ok("视频删除成功");
        }
    }

    //private readonly VideoDbContext _context;

    //public VideosController(VideoDbContext context)
    //{
    //    _context = context;
    //}


    //[HttpGet]
    //public async Task<IActionResult> GetVideos()
    //{
    //    var videos = await _context.Videos.ToListAsync();
    //    return Ok(videos);
    //}

    //[HttpPost]
    //public async Task<IActionResult> CreateVideo(Video video)
    //{
    //    _context.Videos.Add(video);
    //    await _context.SaveChangesAsync();
    //    return Ok(video);
    //}

    //[HttpPut("{id}")]
    //public async Task<IActionResult> UpdateVideo(int id, Video video)
    //{
    //    if (id != video.Id)
    //    {
    //        return BadRequest();
    //    }

    //    _context.Entry(video).State = EntityState.Modified;
    //    await _context.SaveChangesAsync();
    //    return NoContent();
    //}

    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeleteVideo(int id)
    //{
    //    var video = await _context.Videos.FindAsync(id);
    //    if (video == null)
    //    {
    //        return NotFound();
    //    }

    //    _context.Videos.Remove(video);
    //    await _context.SaveChangesAsync();
    //    return NoContent();
    //}

}
