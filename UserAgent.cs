using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ActressMas;

namespace TicketPurchasingAgent
{
    public class UserAgent : Agent
    {
        private string _startingPoint;
        private string _destination;
        private DateTime _dates;
        private int _costPreference;
        private int _timePreference;
        public UserAgent(string startingPoint, string destination, DateTime dates, int costPreference, int timePreference)
        {
            _startingPoint = startingPoint;
            _destination = destination;
            _dates = dates;
            _costPreference = costPreference;
            _timePreference = timePreference;
        }

        public override void Setup()
        {
            // Send a request to the AssistantAgent with the destination, dates, and preferences
            Send("assistant", $"convenient-options {_startingPoint} {_destination} {_dates.ToString("dd.MM.yyyy")} {_costPreference} {_timePreference}");
        }

        // Message received
        public override void Act(Message receivedMessage)
        {
            string action; string parameters;
            Utils.ParseMessage(receivedMessage.Content, out action, out parameters);
            City chicago = City.GetCoords("Chicago");
            
            // Verificati daca mesajul este o cerere de la un obiect UserAgent pentru cele mai convenabile optiuni de calatorie
            switch (action)
            {

                //AssistantAgent sends this message when it has finished constructing the response with the top 4 best travel routes
                case "convenient-options-response":

                    if (receivedMessage.Sender == "assistant")
                    {
                        // Parse the response from the AssistantAgent
                        string[] parts = receivedMessage.Content.Split(' ');

                        ShowTravelOptions(parts, receivedMessage.Sender);
                    }
                    break;
                default:
                    break;
            }
        }
        // Prints options received from assistant and checks if any are usable
        public void ShowTravelOptions(string[] parts, string assistantName)
        {
            Console.WriteLine("Received options from assistant [0]:", assistantName);
            Console.WriteLine(parts);

            //assuming parts formatting is the following:
            //parts[0] = f.DepartureCity, parts[1] = f.ArrivalCity, parts[2] = f.Cost.ToString(), parts[3] = f.TravelTime().ToString();
            int i, selectedOption = 0;
            for (i = 0; i < parts.Length; i += 4)
            {
                string departure = parts[i];
                string arrival = parts[i + 1];
                int cost = int.Parse(parts[i + 2]);
                int time = int.Parse(parts[i + 3]);

                // Compare the option with the desired arrival time +/- 3 days
                if (time <= _dates.AddDays(3).Subtract(_dates).TotalDays || time >= _dates.AddDays(-3).Subtract(_dates).TotalDays)
                {
                    Console.WriteLine($"Accepted option: {i} - Departure: {departure} - Arrival: {arrival} - Cost: {cost} - Time: {time}");
                    selectedOption = 1;
                }
            }
            if (selectedOption == 0)
            {
                Console.WriteLine("No acceptable options found.");
            }
        }
    }
    
    
}
