using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;

namespace HSCore.Entities
{
    [CollectionName("Cards")]
    public class HSCard : Entity
    {
        public string cardId { get; set; }
        public string name { get; set; }
        public int cost { get; set; }
        public string text { get; set; }
        public int count { get; set; }
        public override int GetHashCode()
        {
            return cardId.GetHashCode() * 17 + name.GetHashCode();
        }
    }

    public class HSCardDto
    {
        public string CardId { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public string Text { get; set; }
        public int Count { get; set; }
    }
}
