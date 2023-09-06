using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoDbAdmin.Services
{
    public class GridFSService
    {
        private readonly IMongoDatabase _database;
        private readonly IGridFSBucket _gridFSBucket;

        public GridFSService(IMongoDatabase database)
        {
            _database = database;
            _gridFSBucket = new GridFSBucket(_database);
        }

        public async Task<string> UploadVideoOrImageAsync(Stream stream, string fileName, string contentType)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument
            {
                { "ContentType", contentType }
            }
            };

            var fileId = await _gridFSBucket.UploadFromStreamAsync(fileName, stream, options);
            return fileId.ToString();
        }

        public Stream DownloadVideoOrImage(string fileId)
        {
            var id = new MongoDB.Bson.ObjectId(fileId);
            return _gridFSBucket.OpenDownloadStream(id);
        }

        public async Task DeleteVideoOrImageAsync(string fileId)
        {
            var id = new MongoDB.Bson.ObjectId(fileId);
            await _gridFSBucket.DeleteAsync(id);
        }
    }
}
