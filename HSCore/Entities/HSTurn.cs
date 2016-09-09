using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoRepository;

namespace HSCore.Entities
{
    [CollectionName("Turns")]
    public class HSTurn: Entity
    {
        public string CardId{ get; set; }
        public int TurnNumber { get; set; }
        public string GameId { get; set; }
    }

    public class HSTurnDto 
    {
        public string CardId { get; set; }
        public int TurnNumber { get; set; }
        public string GameId { get; set; }
    }
}
