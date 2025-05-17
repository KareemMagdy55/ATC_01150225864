using System.ComponentModel.DataAnnotations;

namespace Nadwa.Validators;


public class DateValidator :  ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
        var date = (DateTime)value;
        if (date.Day < DateTime.UtcNow.Day && date.Month < DateTime.UtcNow.Month &&
            date.Year < DateTime.UtcNow.Year)
            
        {
            return new ValidationResult("The date must be in the future.");
        }

        return ValidationResult.Success;
    }
    
}
