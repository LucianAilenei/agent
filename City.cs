using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketPurchasingAgent
{
    class City
    {
        public string X { get; set; }
        public string Y { get; set; }
        

        public static Dictionary<string, City> Cities { get; set; } = new Dictionary<string, City>();

        // Gasirea coordonatelor orasului prin apelarea City.GetCityByName(name)
        public static City GetCoords(string name)
        {
            if (Cities.ContainsKey(name))
            {
                return Cities[name];
            }
            else
                return null;
        }
        public static double Distance(City city1, City city2)
        {
            return Math.Sqrt(Math.Pow(int.Parse(city2.X) - int.Parse(city1.X), 2) + Math.Pow(int.Parse(city2.Y) - int.Parse(city1.Y), 2));
        }
        public static double Distance(string city1, string city2)
        {
            return Math.Sqrt(Math.Pow(int.Parse(Cities[city2].X) - int.Parse(Cities[city1].X), 2) + Math.Pow(int.Parse(Cities[city2].Y) - int.Parse(Cities[city1].Y), 2));
        }
        //constructor apelat la initializarea programului; orasele sunt accesibile in toate clasele
        static City()
        {
            Cities.Add("New York", new City { X = "40.730610", Y = "-73.935242"});
            Cities.Add("Chicago", new City { X = "41.878114", Y = "-87.629799" });
            Cities.Add("San Francisco", new City { X = "37.774929", Y = "-122.419416" });
            Cities.Add("Houston", new City { X = "29.760427", Y = "-95.369803" });
            Cities.Add("Los Angeles", new City { X = "34.052235", Y = "-118.243683" });
        }
    }

}
