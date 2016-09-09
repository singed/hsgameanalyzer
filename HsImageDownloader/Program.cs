using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

            StreamWriter file2 = new StreamWriter(@"C:\_Projects\HSGameAnalyzer\HSCardUploader\ids.txt", true);

            Console.WriteLine("Cards fetched: {0}", cards.Count());
            int counter = 0;
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
                        @"C:\_Projects\HSGameAnalyzer\HSCardUploader\images\" + card.CardId + ".png");
                        counter++;
                    }
                    catch (Exception)
                    {
                        file2.WriteLine(card.CardId);
                        Console.WriteLine(string.Format("Image with name:{0}, id:{1} was NOT FOUND! I", cardName, card.CardId));
                    }
                    
                }
                Thread.Sleep(1500);
            }
            file2.Close();
            Console.WriteLine("Downloading finished. Cards founded:{0}. Total Cards:{1}", counter, cards.Count());
        }
    }
}