using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Model;

namespace Brokers.DAL.Generator
{
    public class RandomPublicationGenerator : IGenerator<Message>
    {
        private static readonly Random rnd = new Random();
        private static readonly List<string> TextPatterns;

        static RandomPublicationGenerator()
        {
            TextPatterns = File.ReadAllLines("TextPatterns.txt").OrderBy(line => line.GetHashCode()).ToList();
        }

        public Message GenerateNew()
        {
            Thread.Sleep(1); // To ensure unique Publication IDs generation
            var now = DateTime.UtcNow;
            var pubDate = new DateTime(2020, 1, 1).AddMinutes(rnd.Next(0, 14 * 24 * 60)); // all publications go to date range starting on Jan 1st, 2020, and lasting for 14 days
            var id = now.Ticks;
            var reactionCount = rnd.Next(1, 101);
            var message = new Message
            {
                PublicationDate = pubDate,
                PublicatonId = id,
                AuthorId = (int)(1 + id % 500), // Total of 500 different authors
                HostId = (int)(1 + id % 10),    // Total of 10 different hosts
                ReactionCount = reactionCount,
                ViewCount = reactionCount + rnd.Next(1, 1001),
                Content = GenerateRandomText(rnd.Next(10, 100)),
                Title = GenerateRandomText(rnd.Next(1, 3))
            };

            return message;
        }

        private string GenerateRandomText(int sectionCount)
        {
            var startFrom = rnd.Next(0, TextPatterns.Count);

            var result = new StringBuilder();
            for (var i = 0; i < sectionCount; i++)
            {
                var line = TextPatterns[(startFrom + i) % TextPatterns.Count];
                if (i > 0)
                    result.Append(" ").Append(line);
                else
                    result.Append(char.ToUpper(line[0])).Append(line.Substring(1));
            }

            result.Append(".");

            return result.ToString();
        }
    }
}
