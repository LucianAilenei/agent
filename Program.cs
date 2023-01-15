using System;
using System.Collections.Generic;
//using ActressMas;


namespace TicketPurchasingAgent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var env = new ActressMas.EnvironmentMas(0, 500);
            // Creeaza o instanta a clasei AirlineAgent
            AirlineAgent airline = new AirlineAgent();
            airline.Flights = new List<Flight>();
            // Adauga zboruri la lista de zboruri disponibile a agentului de linie aeriana
            
            airline.Flights.Add(new Flight { DepartureCity = "New York", ArrivalCity = "Chicago", DepartureTime = new DateTime(2022, 1, 1, 8, 0, 0), ArrivalTime = new DateTime(2022, 1, 1, 10, 0, 0), Cost = 200 });
            airline.Flights.Add(new Flight { DepartureCity = "New York", ArrivalCity = "Chicago", DepartureTime = new DateTime(2022, 1, 1, 9, 0, 0), ArrivalTime = new DateTime(2022, 1, 1, 11, 0, 0), Cost = 250 });
            airline.Flights.Add(new Flight { DepartureCity = "New York", ArrivalCity = "Chicago", DepartureTime = new DateTime(2022, 1, 1, 10, 0, 0), ArrivalTime = new DateTime(2022, 1, 1, 12, 0, 0), Cost = 300 });

            env.Add(airline, "airline1");

            // Creeaza o instanta a clasei AssistantAgent
            AssistantAgent assistant = new AssistantAgent();


            env.Add(assistant, "assistant");

            // Creeaza o instanta a clasei UserAgent
            UserAgent user = new UserAgent("New York", "Chicago", new DateTime(2022, 1, 1, 10, 0, 0), 300, 2);

            // Conecteaza agentii intre ei
            //airline.Connect(assistant);
            //assistant.Connect(user);

           

            env.Add(user, "user");
            env.Start();
        }

    }
}
