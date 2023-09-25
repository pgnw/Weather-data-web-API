using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Weather_data_web_api.Models
{
    public class User
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string UserName { get; set; }
        public string HashedPassword { get; set; }
        public bool Active { get; set; }
        public string AcessLevel { get; set; }
        public DateTime LastAccessed { get; set; }
        public string ApiKey { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
