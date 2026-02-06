using System.ComponentModel.DataAnnotations;

namespace Climbing_Weather_App.Models;
//Scores model with post code and rock type
public class ScoresUserRequest
{
    //Make this data requried
    [Required(ErrorMessage = "Postcode is required")]
    public string postcode {get; set;} = string.Empty;
    
    [Required(ErrorMessage = "Rock type is required")]
    public string rock_type {get; set;} = string.Empty;
}

/* TO DO
Server side validation of rock type
*/

/* 
public async Task CheckRockType()
    {
        //Read through and check there is a rock type in rocks.json that matches
    }

*/