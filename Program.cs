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
using System.Runtime.InteropServices;
/* 
Logic for getting the historical precipitation
*/

class Weather
{

    static async Task Main(string[] args)
    {
        //Test GetWeatherValues
        List<int> score_test_1 = await GetWeatherValues("limestone", "NE1 7RU");
        List<int> score_test_2 = await GetWeatherValues("gritstone", "NOTAPOSTCODE*");
        List<int> score_test_3 = await GetWeatherValues("sandstone", "EH1 2EN");

        Debug.Assert(score_test_1.Count() == 10, "Test 1 must be equal to 10, as postcode does exist and data is found so we expect 10 days of rain scores to be returned ");
        Debug.Assert(score_test_2.Count() == 0, "Test 2 must be equal to 0, as postcode does not exist");
        Debug.Assert(score_test_3.Count() == 10 , "Test 3 must be equal to 10, as postcode does exist and data is found so we expect 10 days of rain scores to be returned "); //Need to update test to check highscores
        
        Console.WriteLine("Tests passed");
    }
    //Return rain score
    static async Task<List<int>> GetWeatherValues(string rock_type, string postcode)
    {

        double[] location = await GetLatLon(postcode);

        //If location list is empty, flag postcode error
        if (location.Length == 0)
        {
            Console.WriteLine("Error with postcode");
            //TO DO: Ask for post code again
            return [];
        }

        //Calculate rain score
        else
        {
            DateTime now = DateTime.Now;
            string current_hour_string = now.ToString("HH");
            //Should be safe as current_hour string will always only ever be castable
            int current_hour = int.Parse(current_hour_string);
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

                //TO DO: implement rain score here

                return GetRainScore(current_hour, hourly_precipitation);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with exception {e}");
                return [];
            }
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

    static List<int> GetRainScore(int current_hour, double[] hourly_precipitation)
    {
        //Rain score array
        List<int> rain_score_list = new List<int>();
        //167 is the index representing the start of present day, every 24 indexes is the start of the next day
        for(int i=current_hour+167; i < hourly_precipitation.Length-24; i += 24)
        {
            int rain_score;
            //Last 24 hours
            double pr_24 = GetHoursAveragePrecipitation(i, 24, hourly_precipitation);
            //Last 72 hours
            double pr_72 = GetHoursAveragePrecipitation(i, 72, hourly_precipitation);
            //Last 7 days
            double pr_168 = GetHoursAveragePrecipitation(i, 168, hourly_precipitation);

            //TO DO: Read from json for actual RPF and weights, for now we use placeholders
            double w_24 = 2;
            double w_72 = 1;
            double w_168 = 0.5;
            double RPF = 1;

            rain_score = (int)Math.Ceiling((w_24 * pr_24 + w_72 * pr_72 + w_168 * pr_168)/RPF);

            rain_score_list.Add(rain_score);
        }

        foreach(int score in rain_score_list)
        {
            Console.Write(score + ", ");
        }
        Console.WriteLine();
        return rain_score_list;
    }

    static double GetHoursAveragePrecipitation(int current_hour_index, int hours, double[] hourly_precipitation)
    {
        double total_average_precipitation = 0.00;

        //168: last 7 days, 72: last 3 days, 23: last day

        //We want rainfall from + 6 hours into the future of present day to x hours from present hour in past
        //Hourly Future rainfall not currently included

        for (int i = current_hour_index-hours; i <= current_hour_index; i+=1)
        {
            total_average_precipitation += hourly_precipitation[i];
        }

        return total_average_precipitation/hours;
    }
}
