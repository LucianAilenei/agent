using System;
using System.Collections.Generic;
using System.Linq;

using ActressMas;

namespace TicketPurchasingAgent
{
    public class AirlineAgent : Agent
    {
        // Proprietati pentru numele si locatia companiei aeriene, precum si o lista de obiecte Flight care reprezinta zborurile disponibile
        public string Location { get; set; }
        public List<Flight> Flights { get; set; }
        private int connectRequest = 0;
        public string connectedAssistantAgent;
        // Obiect care stocheaza rutele de zbor disponibile AirlineAgent-ului curent, accesibile prin tupla <DepartureCity, ArrivalCity>
        private List<Flight> flightRoutes;
        public override void Setup()
        {
            //Flights = new List<Flight>();
            connectedAssistantAgent = "assistant";
        }

        public override void Act(Message receivedMessage)
        {
            // Procesati mesajele primite de la alti agenti aici (asistenti)

            if (receivedMessage != null)
            {
                string action; string parameters;
                Utils.ParseMessage(receivedMessage.Content, out action, out parameters);
                // Verificati daca mesajul este o cerere pentru cele mai mici costuri intre doua orase
                switch (action)
                {

                    case "lowest-cost":
                        //a way to check if the message is from AssistantAgent
                        if (receivedMessage.Sender == "assistant")
                        {
                            Connect(receivedMessage.Sender);
                            // Extragem orasele
                            string[] cities = receivedMessage.Content.Split(' ');
                            string city1 = cities[0];
                            string city2 = cities[1];
                            DateTime departureTime = DateTime.Parse(cities[2]);
                            // Gasim cele mai mici costuri intre cele doua orase folosind o abordare avida
                            //greedy1 + greedy2 implementation
                            //record full routes for both 

                            Flight greedy1 = GreedyRoute(city1, city2, departureTime, 0);
                            Flight greedy2 = GreedyRoute(city1, city2, departureTime, 1);

                            //select which greedy is preferred 
                            int heuristics = 0;
                            Flight lowestCostFlight = (heuristics == 0) ? greedy1 : greedy2;
                            if (greedy1.Cost != greedy2.Cost)
                            {
                                lowestCostFlight = (greedy1.Cost < greedy2.Cost) ? greedy1 : greedy2;
                            }
                            // Trimitem raspunsul catre agentul care ne-a trimis cererea
                            this.Send(receivedMessage.Sender, "lowest-cost-response " + city1 + " " + city2 + " " + lowestCostFlight.Cost);
                            connectRequest = 0;
                        }
                        break;
                    default:
                        break;

                }
            }
        }
        // Greedy1 : Construieste ruta ieftina; Greedy2 : Alege ruta in functie de un calculul euristic (o pondere intre distanta dintre orase si pret)
        private Flight GreedyRoute(string city1, string city2, DateTime departureTime, int heuristics)
        {
            //initializare variabile folosite la totalFlight
            int totalCost = 0;
            string departureCity = city1;
            DateTime tempArrivalTime = departureTime;

            // ruta afisata la consola
            string routeString = city1 + " - ";

            while (departureCity != city2)
            {
                // tempFlight inregistreaza ruta aleasa intre 2 orase pe drumul ales 
                Flight tempFlight = FindNextCity(departureCity, departureTime, tempArrivalTime, city2, heuristics);
                if (tempFlight == null)
                    return null;
                routeString += tempFlight.ArrivalCity + " - ";

                // actualizam variabilele cu ruta aleasa
                departureCity = tempFlight.ArrivalCity;
                tempArrivalTime = tempFlight.ArrivalTime;
                totalCost += (int)tempFlight.Cost;
            }
            // totalFlight inregistreaza ruta completa; costul si timpul de sosire sunt updatate in timpul rularii metodei
            Flight totalFlight = new Flight { DepartureCity = city1, ArrivalCity = city2, DepartureTime = departureTime, ArrivalTime = tempArrivalTime, Cost = totalCost, RouteString = routeString };

            return totalFlight;
        }

        internal void Connect(string connectString)
        {
            //Simulate a connection with the AssistantAgent (blocking until disconnect?)
            connectedAssistantAgent = connectString;
            connectRequest = 1;
        }

        // Metoda pentru adaugarea unui nou zbor la lista noastra de zboruri
        public void AddFlight(Flight flight)
        {
            Flights.Add(flight);
        }

        // Metoda pentru gasirea zborurilor cu cele mai mici costuri intre doua orase
        public Flight FindNextCity(string departureCity, DateTime departureTime, DateTime tempArrivalTime, string finalCity, int heuristics)
        {
            // Gasim cele mai mici costuri intre cele doua orase folosind o abordare avida
            //decimal lowestCost = Decimal.MaxValue;
            //double lowestDistance = double.MaxValue;
            Flight lowestCostFlight = null;

            foreach (Flight flight in Flights)
            {
                //if (flight.DepartureCity == city1 && flight.ArrivalCity == city2 && flight.Cost < lowestCost)
                if (heuristics == 1)
                {
                    // greedy euristic, compara cost, distanta intre orase ( functie distanta Cities[departureCity].X , Y la Cities[flight.ArrivalCity].X , Y; valoarea returnata compara cu lowestDistance)
                    // verifica daca mai sunt alte conditii necesare
                     lowestCostFlight = flightRoutes.Where(route => route.DepartureCity == flight.ArrivalCity)
                                .OrderBy(route => (double)route.Cost / City.Distance(route.DepartureCity, finalCity))
                                .First();
                }
                
                else
                {
                    // greedy simplu, compara cost si timpii sa fie corecti
                    // verifica daca mai sunt alte conditii necesare
                    
                    lowestCostFlight = flightRoutes.Where(route => route.DepartureCity == flight.ArrivalCity)
                                .OrderBy(route => route.Cost)
                                .First();
                }

            }

            return lowestCostFlight;
        }

    }
}
