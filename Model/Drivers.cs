using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoCrud.Model
{
    public class Drivers
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public int Name { get; set; }
        [BsonElement("Number")]
        public int Number { get; set; }
        [BsonElement("Team")]
        public int Team { get; set; }
    }
}
