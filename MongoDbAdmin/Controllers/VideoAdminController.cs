using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbAdmin.Models;
using MongoDbAdmin.Services;

namespace MongoDbAdmin.Controllers
{
    /// <summary>
    /// 视频后台管理页
    /// </summary>
    public class VideoAdminController : Controller
    {
        private readonly IMongoCollection<Video> _videoCollection;
        private readonly ILogger<VideoAdminController> _logger;
        public VideoAdminController(IMongoDatabase database, ILogger<VideoAdminController> logger)
        {
            _videoCollection = database.GetCollection<Video>("Video");
            _logger = logger;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var videos = await _videoCollection.Find(_ => true).ToListAsync();
            return View(videos);
        } 
    }
}
