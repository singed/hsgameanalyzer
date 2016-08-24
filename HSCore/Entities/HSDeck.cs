using System;
using System.Collections.Generic;
using MongoRepository;

namespace HSCore.Entities
{
    [CollectionName("Decks")]
    public class HSDeck : Entity
    {
        public string Name { get; set; }
      //  public string SiteName { get; set; }
        public string Link { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }
        public IList<HSCard> Cards { get; set; }
        public DateTime Date { get; set; }
    }

    public class HSDeckDto
    {
        public string Name { get; set; }
        //  public string SiteName { get; set; }
        public string Link { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }
        public IList<HSCardDto> Cards { get; set; }
        public DateTime Date { get; set; }
    }

 /*   public class CardInDeckDto
    {
        public HSCardDto Card { get; set; }
        public int Count { get; set; }
    }
    public class CardInDeck
    {
        public HSCard Card { get; set; }
        public int Count { get; set; }
    }*/
}
