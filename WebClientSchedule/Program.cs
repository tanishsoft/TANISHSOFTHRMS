using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebClientSchedule
{
    public class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://infonet.fernandezhospital.com/");
            // Add an Accept header for JSON format.    
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // List all Names.    
            HttpResponseMessage response = client.GetAsync("CallCenter/SendemailtoNextlevel").Result;  // Blocking call!    
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Success");
                //var products = response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            HttpResponseMessage response1 = client.GetAsync("Task/SendRemaindersToUser").Result;  // Blocking call!    
            if (response1.IsSuccessStatusCode)
            {
                Console.WriteLine("Success");
                //var products = response1.Content.ReadAsStringAsync().Result;
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response1.StatusCode, response1.ReasonPhrase);
            }

            HttpResponseMessage response2 = client.GetAsync("TaskSchedule/UpdatePatientInfo15minutes").Result;  // Blocking call!    
            if (response2.IsSuccessStatusCode)
            {
                Console.WriteLine("Success");
                //var products = response1.Content.ReadAsStringAsync().Result;
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response2.StatusCode, response2.ReasonPhrase);
            }
        }
    }
}
