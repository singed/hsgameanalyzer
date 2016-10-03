using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MongoRepository;

namespace HSCore.Entities
{
    [CollectionName("Games")]
    public class HSGame : Entity
    {
        public string GameId { get; set; }
        public string OpponentClass { get; set; }
        public string ProbableDeckId { get; set; }
        public string ProbableDeckType { get; set; }
        public bool Won { get; set; }
    }

    public class HSGameDto
    {
        public string GameId { get; set; }
        public string OpponentClass { get; set; }
        public string ProbableDeckId { get; set; }
        public string ProbableDeckType { get; set; }
        public bool Won { get; set; }
    }
}
