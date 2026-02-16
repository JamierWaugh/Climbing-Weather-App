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

namespace Climbing_Weather_App.Weather;
public class Weather
{
    //TO DO
    //Find a new way to test now it has become a web api (main removed)
    public  async Task<List<int>> GetWeatherValues(string rock_type, string postcode)
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

                //Turn precipitation element into double[] by enumerating the array and converting each element into an integer
                double[] hourly_precipitation = precipitationElement.EnumerateArray().Select(e => e.GetDouble()).ToArray();

                Console.WriteLine(hourly_precipitation.Length);

                //TO DO: implement rain score here

                return await GetSeverityList(current_hour, hourly_precipitation, rock_type);

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

    static async Task<List<int>> GetSeverityList(int current_hour, double[] hourly_precipitation, string rock_type)
    {
        //Rain score array
        List<int> severity_list = new List<int>();
        //167 is the index representing the start of present day, every 24 indexes is the start of the next day
        for(int i=current_hour+167; i < hourly_precipitation.Length-24; i += 24)
        {
            int severity = -1;
            //Last 24 hours
            double pr_24 = GetTotalPrecipitation(i, 24, hourly_precipitation);
            //Last 72 hours
            double pr_72 = GetTotalPrecipitation(i, 72, hourly_precipitation);
            //Last 7 days
            double pr_168 = GetTotalPrecipitation(i, 168, hourly_precipitation);

            //TO DO: Read from json for actual RPF and weights, for now we use placeholders

           try
            {
                //Read from rocks.json
                string rocks_json = await File.ReadAllTextAsync(
                    Path.Combine(AppContext.BaseDirectory + "rocks.json"));
                JsonDocument doc = JsonDocument.Parse(rocks_json);
                JsonElement rock = doc.RootElement
                    .GetProperty("rocks")
                    .GetProperty(rock_type); 
                //Get max severity of all rainfall windows
                severity = Math.Max(GetSeverity(rock, "24", pr_24), Math.Max(GetSeverity(rock, "72", pr_72), GetSeverity(rock, "168", pr_168)));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with exception {e}");
                return [];
            }
            severity_list.Add(severity);
        }
        DateTime now =  DateTime.Now;
        //Output here
        //Output into console, eventually into web app
        foreach(int severity in severity_list)
        {  
            string now_output = now.ToString("yyyy-MM-dd");

            Console.Write(now_output + ": " + severity + ", ");
            now = now.AddDays(1); //Fixed output issue where days got progressively larger
        }
        Console.WriteLine();
        return severity_list;
    }

    static double GetTotalPrecipitation(int current_hour_index, int hours, double[] hourly_precipitation)
    {
        double total_precipitation = 0.00;

        //168: last 7 days, 72: last 3 days, 23: last day

        //We want rainfall from + 6 hours into the future of present day to x hours from present hour in past
        //Hourly Future rainfall not currently included

        for (int i = current_hour_index-hours; i <= current_hour_index; i+=1)
        {
            total_precipitation += hourly_precipitation[i];
        }

        return total_precipitation;
    }

    static int GetSeverity(JsonElement rock_json, string hour_time, double total_precipitation)
    {
        int severity = 0;
        //For every range in the hours, 24 hours, 72 hours, etc. Find which severity is applicable
        JsonElement hours = rock_json.GetProperty("thresholds").GetProperty(hour_time);
        foreach(JsonElement range in hours.EnumerateArray())
        {
            //If the total precipitation is inside the min and max, take that severity find the max of all severities and return
            if (range.GetProperty("min").GetDouble() < total_precipitation && total_precipitation < range.GetProperty("max").GetDouble())
            {
                severity = Math.Max(severity, range.GetProperty("severity").GetInt32());
            }
        }  
        return severity;
    }
}
