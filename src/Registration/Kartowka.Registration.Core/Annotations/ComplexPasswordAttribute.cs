using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Kartowka.Registration.Core.Annotations;

public class ComplexPasswordAttribute : ValidationAttribute
{
    private static readonly Regex PasswordValidationRegex = new(
        @"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\W]).{6,50})", 
        RegexOptions.Compiled
    );

    public override bool IsValid(object? value)
    {
        var password = value?.ToString();
        if (password == null)
        {
            // Should be handled by the RequiredAttribute
            return true;
        }

        return PasswordValidationRegex.IsMatch(password);
    }
}
