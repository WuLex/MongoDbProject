using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDbAdmin.Models;
using MongoDbAdmin.Services;

namespace MongoDbAdmin.Controllers
{
    /// <summary>
    /// 视频展示卡片页
    /// </summary>
    public class VideoWebController : Controller
    {
        private readonly VideoService _videoService;
        private readonly GridFSService _gridFSService;
        private readonly IMongoCollection<Video> _videoCollection;
        private readonly ILogger<VideoWebController> _logger;
        private readonly IGridFSBucket _gridFSBucket;
        public VideoWebController(VideoService videoService, GridFSService gridFSService, IMongoDatabase database, ILogger<VideoWebController> logger)
        {
            _videoService = videoService;
            _gridFSService = gridFSService;
            _videoCollection = database.GetCollection<Video>("Video");
            _gridFSBucket = new GridFSBucket(database);
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            // 显示视频列表
            var videos =await _videoService.GetAllVideos();
            return View(videos);
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(string title, Stream videoStream)
        {
            // 上传视频
            var fileId = await _videoService.UploadVideoAsync(title, videoStream);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Download(string id)
        {
            // 下载视频
            var fileId = ObjectId.Parse(id);
            var videoStream = await _videoService.DownloadVideoAsync(fileId);

            if (videoStream != null)
            {
                return File(videoStream, "video/mp4"); // 假设视频是MP4格式
            }

            return NotFound();
        }


        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Play(string id)
        {
            // 根据视频ID获取视频文件
            var fileStream =await _gridFSBucket.OpenDownloadStreamAsync(ObjectId.Parse(id));

            // 返回视频文件流
            return File(fileStream, "video/mp4"); // 适配视频文件类型 


        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DownloadVideo(string id)
        {
            // 根据视频ID获取视频文件
            var fileStream = await _gridFSBucket.OpenDownloadStreamAsync(ObjectId.Parse(id));

            // 返回视频文件供下载
            return File(fileStream, "application/octet-stream", "video.mp4"); // 适配视频文件类型和文件名
        }

    }
}
