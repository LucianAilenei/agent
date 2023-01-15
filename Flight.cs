using System;
using ActressMas;

namespace TicketPurchasingAgent
{
    public class Flight
    {
        // Proprietati pentru orasul de plecare, orasul de sosire, momentul de plecare, momentul de sosire si cost; X si Y sunt utilizate la calculul greedy
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public decimal Cost { get; set; }

        // string de afisare la consola a rutei complete selectate de greedy
        public string RouteString { get; set; }
        

        // Metoda pentru calcularea timpului de calatorie intre cele doua orase pe baza momentelor de plecare si sosire
        public TimeSpan TravelTime()
        {
            return ArrivalTime - DepartureTime;
        }
    }
}
