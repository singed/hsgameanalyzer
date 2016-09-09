using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HSCore.Entities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using MongoRepository;
using Newtonsoft.Json;

namespace HSGameAnalyzer
{
    [HubName("hshub")]
    public class HSGameHub : Hub
    {
        /* public void GetDecks(string name)
         {
             var _deckRepository = new MongoRepository<HSDeck>();
             /*var decks = _deckRepository.Where(d => d.Class == deckClass.ToUpper()).ToList();#1#
             string[] decks = { "Item1", "Item2", "Item3", "Item4" }; 

             Clients.All.applyDecks(decks);
         }*/

        public IEnumerable<HSDeckDto> GetDecks(string className)
        {
            MongoRepository<HSDeck>_deckRepository = new MongoRepository<HSDeck>();

            var decks = _deckRepository.Where(d => d.Class == className.ToUpper()).ToList();
            var decksDto = Mapper.Map<IEnumerable<HSDeckDto>>(decks);

           
           // string serialized = JsonConvert.SerializeObject(decks);

            return decksDto;
            //Clients.All.applyDecks(decks);
        }

        public void SaveTurn(HSTurnDto turnDto)
        {
            var turn = Mapper.Map<HSTurn>(turnDto);
            MongoRepository<HSTurn> turnRepository = new MongoRepository<HSTurn>();
            turnRepository.Add(turn);
        }
    }
}
