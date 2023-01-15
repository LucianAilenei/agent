using System;
using System.Collections.Generic;
using System.Linq;

using ActressMas;

namespace TicketPurchasingAgent
{
    public class AssistantAgent : Agent
    {
        private Queue<Message> Inbox;
        private List<string> Airlines;
        private List<string> AirlinesCheck; //only used for checking against the list of Airlines
        private string connectedUserAgent;
        private int connectRequest = 0;
        private string[] citiesAndDates;
        private Dictionary<Flight, decimal> options;
        public override void Setup()
        {
            Inbox = new Queue<Message>();
            Airlines = new List<string>();
            AirlinesCheck = new List<string>();
            options = new Dictionary<Flight, decimal>();
            Connect("user");
        }

        public override void Act(Message message)
        {
            Inbox.Enqueue(message);
            // Procesati mesajele primite de la alti agenti aici
            while (this.Inbox.Count > 0)
            {
                Message receivedMessage = this.Inbox.Dequeue();
                string action; string parameters;
                Utils.ParseMessage(receivedMessage.Content, out action, out parameters);

                // Verificati daca mesajul este o cerere de la un obiect UserAgent pentru cele mai convenabile optiuni de calatorie
                switch (action)
                {
                    //UserAgents send this message and wait for the assistant to send back up to 4 most convenient travel options
                    case "convenient-options":
                        if (connectRequest == 0)
                        {
                            // Extragem orasele si datele din mesaj
                            citiesAndDates = parameters.Split(' ');
                            string city1 = citiesAndDates[0];
                            string city2 = citiesAndDates[1];
                            DateTime startDate = DateTime.Parse(citiesAndDates[2]);
                            string costPreference = citiesAndDates[3];
                            string timePreference = citiesAndDates[4];

                            // Trimitem cerere catre fiecare agent AirlineAgent pentru cele mai mici costuri intre cele doua orase
                            SendToMany(Airlines, "lowest-cost " + city1 + " " + city2 + " " + startDate);
                        }
                        break;

                    //AirlineAgents send this message after they finish processing the lowest-cost request from the AssistantAgent 
                    case "lowest-cost-response":
                        // Procesam raspunsurile primite de la agentii AirlineAgent
                        // Verificati daca mesajul este un raspuns la cererea noastra pentru cele mai mici costuri intre cele doua orase
                        citiesAndDates = parameters.Split(' ');

                        // Extragem costul din mesaj

                        decimal cost = Decimal.Parse(citiesAndDates[2]);

                        // Adaugam optiunea la lista noastra de optiuni
                        Flight flight = new Flight();
                        flight.DepartureCity = citiesAndDates[0];
                        flight.ArrivalCity = citiesAndDates[1];
                        flight.Cost = cost;


                        options.Add(flight, cost);
                        AirlinesCheck.Add(message.Sender);
                       

                        // Daca am primit de la toti agentii airline din lista un raspuns
                        if (AirlinesResponseCheck())
                        {
                            // Sortam lista de optiuni dupa cost si timpul de calatorie
                            List<Flight> sortedOptions = options.Keys.OrderBy(o => options[o]).ThenBy(o => o.TravelTime()).ToList();
                            // Trimitem raspunsul catre agentul care ne-a trimis cererea (limitat la max. 4 optiuni in raspuns)
                            string response = "convenient-options-response ";
                            int count = 0;
                            foreach (Flight f in sortedOptions)
                            {
                                count++;
                                response += f.DepartureCity + " " + f.ArrivalCity + " " + f.Cost.ToString() + " " + f.TravelTime().ToString() + " ";
                                if(count == 4)
                                   break;
                            }
                            this.Send(receivedMessage.Sender, response);
                            connectRequest = 0;
                            AirlinesCheck.Clear();
                        }
                        break;
                    default:
                        break;
                }
                
                
            }
        }

        private bool AirlinesResponseCheck()
        {
            // Daca lista de agenti de la care am primit mesaje in sesiunea curenta are toate airline-urile din lista contactata
            if (AirlinesCheck.All(i => Airlines.Contains(i)))
                return true;
            else
                return false;
        }

        internal void Connect(string connectString)
        {
            //Simulate a connection with the UserAgent (assumes the agent exists, otherwise simulated ping should be implemented in Act())
            connectedUserAgent = connectString;
            //Assistant is blocked (users cannot make requests to this agent) until it sends the final response to the requesting user
            connectRequest = 1;
           
        }
    }

    
}
