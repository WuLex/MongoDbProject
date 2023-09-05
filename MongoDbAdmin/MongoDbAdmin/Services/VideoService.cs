using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using MongoDbAdmin.Models;
using Microsoft.Extensions.Options;

namespace MongoDbAdmin.Services
{
    public class VideoService
    {
        private readonly IMongoDatabase _database;
        private readonly IGridFSBucket _gridFS;
        private readonly IMongoCollection<Video> _videoCollection;
        private readonly MongoSettings _mongoSettings;
        public VideoService(IMongoClient client, IOptions<MongoSettings> mongoSettings)
        {
            _mongoSettings = mongoSettings.Value;
            _database = client.GetDatabase(_mongoSettings.DatabaseName);
            _gridFS = new GridFSBucket(_database);
            _videoCollection = _database.GetCollection<Video>("Video");
        }
        public async Task<List<Video>> GetAllVideos()
        {
            var videos =(await _videoCollection.FindAsync(_ => true)).ToList();
            return videos;
        }

        public async Task<ObjectId> UploadVideoAsync(string title, Stream videoStream)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument("title", title)
            };
            var fileId = await _gridFS.UploadFromStreamAsync(title, videoStream, options);
            return fileId;
        }

        public async Task<Stream> DownloadVideoAsync(ObjectId fileId)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", fileId);
            var fileInfo = await _database.GetCollection<GridFSFileInfo>("fs.files").Find(filter).FirstOrDefaultAsync();

            if (fileInfo != null)
            {
                var stream = new MemoryStream();
                await _gridFS.DownloadToStreamAsync(fileId, stream);
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }

            return null;
        }
    }
}
