using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SurveyHeaven.Domain.Common
{
    public abstract class MongoDbEntity : IEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        [BsonElement(Order = 0)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement(Order = 101)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
