using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace MongoDbAdmin.Models
{
    // 模型类
    public class Image
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string Id { get; set; }

        //public ObjectId Id { get; set; }
        //public string Title { get; set; }
        //public string Url { get; set; }

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("FileName")]
        public string FileName { get; set; }

        [BsonElement("ContentType")]
        public string ContentType { get; set; }

        [BsonElement("FileData")]
        public string FileData { get; set; }
    }

}
