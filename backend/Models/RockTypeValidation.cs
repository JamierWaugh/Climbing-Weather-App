using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RocksStartService.RockTypeService;

namespace RockTypeValidation.RockValid;
public class ValidateRockType : ValidationAttribute
{
    public ValidateRockType()
    {
        ErrorMessage = "Rock type not found";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string str)
        {
            return new ValidationResult(ErrorMessage);
        }

        var rockService = (RockTypeService)validationContext.GetService(typeof(RockTypeService))!;

        if (rockService.IsValidRock(str))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(ErrorMessage);
    }
}