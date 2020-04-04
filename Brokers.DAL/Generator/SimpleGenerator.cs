using Brokers.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Brokers.DAL.Generator
{
    public class SimpleGenerator<T> : IGenerator<T> where T : class, new()
    {
        private const string alphabet = " abcefghijklmnopqrstuvwxyz";
        static Random rand = new Random();
        static List<PropertyInfo> props;

        static SimpleGenerator()
        {
            props = typeof(T).GetProperties().Where(p => p.CanWrite
                    && p.PropertyType == typeof(int)
                    || p.PropertyType == typeof(string)).ToList();
        }

        public T GenerateNew()
        {
            var ent = new T();
            props.ForEach(p =>
            {
                if (p.PropertyType == typeof(int))
                {
                    p.SetValue(ent, rand.Next(1, 100));
                }
                else if (p.PropertyType == typeof(string))
                {
                    var genString = new string(Enumerable.Repeat(0, rand.Next(1, 30)).Select(a => alphabet[rand.Next(alphabet.Length)]).ToArray());
                    p.SetValue(ent, genString);
                }
            });
            return ent;
        }
    }
}
