using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;

namespace RocksStartService.RockTypeService;
/* 
Create rock type service that flattens json into rock types for search
*/
public class RockTypeService
{
    private readonly HashSet<string> _rockTypes;

    //Creates rocktypes as a hashset upon server run so calls are fast
    public RockTypeService()
    {
         //Read through and check there is a rock type in rocks.json that matches
            //for all types in rocks, check if there is a match
            string rocks_json = File.ReadAllText(
                Path.Combine(AppContext.BaseDirectory, "rocks.json"));
                JsonDocument doc = JsonDocument.Parse(rocks_json);
                JsonElement rock = doc.RootElement
                .GetProperty("rocks");
        
            // => is a lambda function used to convert all elements e into their names
            //Enumerates all objects and converts all element values into a string of object names
            _rockTypes = rock.EnumerateObject().Select(e => e.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
    }


    //Uses hashset to quickly check if there is a match
    public bool IsValidRock(string rockType)
    {
        return _rockTypes.Contains(rockType);
    }

    public HashSet<string> GetRocks()
    {
        return _rockTypes;
    }

}