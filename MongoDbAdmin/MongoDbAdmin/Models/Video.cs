using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace MongoDbAdmin.Models
{
    public class Video
    {
      
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        //public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }

    }

    //public class Video
    //{
    //    public ObjectId Id { get; set; }
    //    public string Title { get; set; }
    //    public string Url { get; set; }
    //}
}
