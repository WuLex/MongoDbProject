using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDbAdmin.Models;
using MongoDbAdmin.Services;
using static System.Net.Mime.MediaTypeNames;
using Image = MongoDbAdmin.Models.Image;

namespace MongoDbAdmin.Controllers
{
    public class ImageController : Controller
    { 
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<GridFSFileInfo> _imageCollection;
        private readonly IGridFSBucket _gridFSBucket;

        public ImageController(IMongoClient mongoClient)           
        {
            _database = mongoClient.GetDatabase("CenterDb");
            _imageCollection = _database.GetCollection<GridFSFileInfo>("fs.files");
            _gridFSBucket = new GridFSBucket(_database);
        }

        public IActionResult Index()
        {
            var images = _database.GetCollection<Image>("Image").Find(new BsonDocument()).ToList();
           
            return View(images); 
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var image = new Image
                    {
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        FileData = Convert.ToBase64String(memoryStream.ToArray())
                    };
                    await _database.GetCollection<Image>("Image").InsertOneAsync(image);
                }
            }
            return RedirectToAction("Index");
        }
         
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var image = _database.GetCollection<Image>("Image")
                .Find(i => i.Id == new ObjectId(id))
                .FirstOrDefault();

            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, IFormFile file)
        {
            var image = _database.GetCollection<Image>("Image")
                .Find(i => i.Id == new ObjectId(id))
                .FirstOrDefault();

            if (image == null)
            {
                return NotFound();
            }

            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    image.FileName = file.FileName;
                    image.ContentType = file.ContentType;
                    image.FileData =  Convert.ToBase64String(memoryStream.ToArray());
                    await _database.GetCollection<Image>("Image")
                        .ReplaceOneAsync(i => i.Id == new ObjectId(id), image);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            var image = _database.GetCollection<Image>("Image")
                .Find(i => i.Id == new ObjectId(id))
                .FirstOrDefault();

            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(string id)
        {
            var objectId = new ObjectId(id);
            _database.GetCollection<Image>("Image").DeleteOne(i => i.Id == objectId);
            _gridFSBucket.Delete(objectId);
            return RedirectToAction("Index");
        }

        #region Gridfs保存上传的图片

        //public async Task<IActionResult> ViewImage(string id)
        //{

        //    var objectId = new ObjectId(id);
        //    var image = await _gridFSBucket.OpenDownloadStreamAsync(objectId);
        //    if (image == null)
        //    {
        //        return NotFound();
        //    }
        //    return File(image, "application/octet-stream", "image.png");
        //}

        //[HttpPost]
        //public async Task<IActionResult> Upload()
        //{
        //    // 处理上传图片的逻辑，将图片保存至GridFS

        //    return Ok(new { message = "上传成功" });
        //}

        //[HttpPost]
        //public async Task<IActionResult> UploadImage(IFormFile imageFile)
        //{
        //    if (imageFile != null && imageFile.Length > 0)
        //    {
        //        using (var stream = imageFile.OpenReadStream())
        //        {
        //            var options = new GridFSUploadOptions
        //            {
        //                Metadata = new BsonDocument("filename", imageFile.FileName)
        //            };
        //            await _gridFSBucket.UploadFromStreamAsync(imageFile.FileName, stream, options);
        //        }
        //    }

        //    return RedirectToAction("Index");
        //}

        ////[HttpPost("uploadImage")]
        ////public async Task<IActionResult> UploadImage(IFormFile imageFile)
        ////{
        ////    if (imageFile == null || imageFile.Length == 0)
        ////    {
        ////        return BadRequest("Please provide a valid image file.");
        ////    }

        ////    var imageId = await _gridFSService.UploadImageAsync(imageFile.OpenReadStream(), imageFile.FileName, imageFile.ContentType);

        ////    // 保存图像的元数据或其他信息到数据库
        ////    // 这里可以根据需要执行操作，例如保存图像的URL等信息

        ////    return Ok("Image uploaded successfully.");
        ////}

        //public async Task<string> UploadImageAsync(Stream stream, string fileName, string contentType)
        //{
        //    var options = new GridFSUploadOptions
        //    {
        //        Metadata = new BsonDocument
        //{
        //    { "ContentType", contentType }
        //}
        //    };

        //    var fileId = await _gridFSBucket.UploadFromStreamAsync(fileName, stream, options);
        //    return fileId.ToString();
        //}

        //[HttpDelete]
        //public async Task<IActionResult> ImageList()
        //{
        //    //var images = _imageCollection.Find(new BsonDocument()).ToList();
        //    //var images = _imageCollection.Find(image => true).ToList();
        //    //return View(images);
        //    return View();
        //}

        //[HttpDelete]
        //public async Task<IActionResult> DeleteImage(string imageId)
        //{
        //    // 删除指定图片
        //    return Ok(new { message = "删除成功" });
        //} 
        #endregion
    }
}
