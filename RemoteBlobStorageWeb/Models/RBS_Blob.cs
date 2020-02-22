using System;
using System.Web.Script.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace RBS.Models
{
    public class RBS_Blob
    {
        public RBS_Blob()
        {
        }
        [ScriptIgnore]
        [JsonIgnore]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedOn { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ModifiedOn { get; set; }

        public string uuid { get; set; }
        public string contents { get; set; }
        public string purpose { get; set; }
        public string tag { get; set; }
    }
}
