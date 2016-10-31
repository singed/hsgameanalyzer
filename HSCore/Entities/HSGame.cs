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
        public DateTime Date { get; set; }
        public string Region { get; set; }
        public string GameMode { get; set; }
        public string OpponentClass { get; set; }
        public string OpponentName { get; set; }
        public string OpponentRank { get; set; }
        public string PlayerClass { get; set; }
        public string PlayerName { get; set; }
        public string PlayerRank { get; set; }
        public string OpponentDeckId { get; set; }
        public string OpponentDeckType { get; set; }
        public string OpponentDeckMatch { get; set; }
        public string PlayerDeckId { get; set; }
        public string PlayerDeckType { get; set; }
        public bool PlayerHasCoin { get; set; }
        public bool Won { get; set; }
    }

    public class HSGameDto
    {
        public string GameId { get; set; }
        public DateTime Date { get; set; }
        public string Region { get; set; }
        public string GameMode { get; set; }
        public string OpponentClass { get; set; }
        public string OpponentName { get; set; }
        public string OpponentRank { get; set; }
        public string PlayerClass { get; set; }
        public string PlayerName { get; set; }
        public string PlayerRank { get; set; }
        public string OpponentDeckId { get; set; }
        public string OpponentDeckType { get; set; }
        public string OpponentDeckMatch { get; set; }
        public string PlayerDeckId { get; set; }
        public string PlayerDeckType { get; set; }
        public bool PlayerHasCoin { get; set; }
        public IEnumerable<string> PlayerCardsIds { get; set; }
        public bool Won { get; set; }
    }
}
