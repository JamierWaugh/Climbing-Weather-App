using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.Json;
using RockTypeValidation.RockValid;

namespace Climbing_Weather_App.Models;
//Scores model with post code and rock type
public class ScoresUserRequest
{
    //Make this data requried
    [Required(ErrorMessage = "Postcode is required")]
    public string postcode {get; set;} = string.Empty;
    
    [Required(ErrorMessage = "Rock type is required")]
    [ValidateRockType()] //Server side rock type check
    public string rock_type {get; set;} = string.Empty;
}