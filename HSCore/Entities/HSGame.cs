using System;
using System.Collections.Generic;

namespace HSCore.Entities
{
    public class HSGame
    {
        public string GameId { get; set; }
        public HSGameEventTypes EventType { get; set; }
        public int TurnNumber { get; set; }
        public int OpponentHandCount { get; set; }
        public int OpponentDeckCount { get; set; }
        public HSCard PlayedCard { get; set; }
        public List<HSCard> OpponentPlayedCards { get; set; }
    }
}
