using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using AutoMapper;
using HSCore.Entities;
using MongoRepository;

namespace HsImageDownloader
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new MappingsConfiguration());
            });

            var cardRepository = new MongoRepository<HSCard>();

            Console.WriteLine("Getting cards");
            var cards = Mapper.Map<IEnumerable<HSCardDto>>(cardRepository.ToList());

            Console.WriteLine("Cards fetched: {0}", cards.Count());

            foreach (var card in cards)
            {
                string cardName = card.Name.Replace(" ", "-").ToLower();
                using (var client = new WebClient())
                {
                    string uri = "https://cdn-tempostorm.netdna-ssl.com/cards/" + cardName + ".small.png";
                    try
                    {
                        client.DownloadFile(
                        new Uri(uri),
                        @"C:\_Projects\hsgameanalyzer\hsgameanalyzer\HSCardUploader\images\" + card.CardId + ".png");
                       // Console.WriteLine("{0} was downloaded", cardName);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(string.Format("Image with name:{0}, id:{1} was NOT FOUND! I", cardName, card.CardId));
                    }
                    
                }
                Thread.Sleep(1500);
            }
        }
    }
}