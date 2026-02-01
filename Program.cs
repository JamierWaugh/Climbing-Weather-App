using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Microsoft.VisualBasic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Net;
/* 
Logic for getting the historical precipitation
*/

class Weather
{

    static async Task Main(string[] args)
    {
        //Test GetWeatherValues
        int score_test_1 = await GetWeatherValues("limestone", "NE1 7RU");
        int score_test_2 = await GetWeatherValues("gritstone", "NOTAPOSTCODE*");

        Debug.Assert(score_test_1 == 999, "Test 1 must be equal to 999, as postcode does exist");
        Debug.Assert(score_test_2 == -1, "Test 2 must be equal to -1, as postcode does not exist");

        Console.WriteLine("Tests passed");
    }
    //Return rain score
    static async Task<int> GetWeatherValues(string rock_type, string postcode)
    {
        int rain_score = -1;

        double[] location = await GetLatLon(postcode);

        //If location list is empty, flag postcode error
        if (location.Length == 0)
        {
            Console.WriteLine("Error with postcode");
            //TO DO: Ask for post code again
            return rain_score;
        }

        //Calculate rain score
        else
        {
            DateTime now = DateTime.Now;
            //Get a week from today, we know that the present day is index 167 in the json
            DateTime week_ago = now.AddDays(-7);
            //Get 10 days into the future (for a forecast)
            now = now.AddDays(10);

            string now_str = now.ToString("yyyy-MM-dd");
            string week_ago_str = week_ago.ToString("yyyy-MM-dd");

            string url = $"https://historical-forecast-api.open-meteo.com/v1/forecast?latitude={location[0]}&longitude={location[1]}&start_date={week_ago_str}&end_date={now_str}&hourly=precipitation";
            Console.WriteLine(url);
            //Create HttpClient
            HttpClient client = new HttpClient();

            try
            {
                //Get response
                HttpResponseMessage response = await client.GetAsync(url);

                //Throw exception
                response.EnsureSuccessStatusCode();

                //Read response content
                string responseBody = await response.Content.ReadAsStringAsync();

                //Parse into Json Element
                JsonDocument doc = JsonDocument.Parse(responseBody);
                JsonElement root = doc.RootElement;

                JsonElement precipitationElement = root.GetProperty("hourly").GetProperty("precipitation");

                //Turn precipitation element into int[] by enumerating the array and converting each element into an integer
                double[] hourly_precipitation = precipitationElement.EnumerateArray().Select(e => e.GetDouble()).ToArray();

                Console.WriteLine(hourly_precipitation.Length);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with exception {e}");
            }
            
            return 999;
        }

        //TO DO: Now we have lat and lon, get weather data.
    }

    static async Task<double[]> GetLatLon(string postcode)
    {
        string url = $"https://api.postcodes.io/postcodes/{postcode}";
        Console.WriteLine(url);

        //Create httpclient
        HttpClient client = new HttpClient();
        
        try
        {
            //Send request
            HttpResponseMessage response = await client.GetAsync(url);

            //Throw exception
            response.EnsureSuccessStatusCode();

            //Read response content

            string responseBody = await response.Content.ReadAsStringAsync();

            //Parse string json into JsonDocument object
            using JsonDocument doc = JsonDocument.Parse(responseBody);
            //Get json element
            JsonElement root = doc.RootElement; 

            //Get lat and lon from json
            double lat = root.GetProperty("result").GetProperty("latitude").GetDouble();
            double lon = root.GetProperty("result").GetProperty("longitude").GetDouble();

            double[] location = [lat,lon];

            return location;
            
        }

        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error with exception {e}");
            return [];
        }


    }
}
