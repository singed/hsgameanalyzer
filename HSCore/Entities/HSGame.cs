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
        public HSGameEventTypes EventType { get; set; }
        public string OpponentClass { get; set; }
        public string ProbableOpponentDeckId { get; set; }
        //public IEnumerable<HSTurn> Turns { get; set; }
    }

    public class HSGameDto
    {
        public string GameId { get; set; }
        public HSGameEventTypes EventType { get; set; }
        public string OpponentClass { get; set; }
        public string ProbableOpponentDeckId { get; set; }
        //public IEnumerable<HSTurn> Turns { get; set; }
    }
}
