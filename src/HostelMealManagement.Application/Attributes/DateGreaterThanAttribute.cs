using System.ComponentModel.DataAnnotations;

namespace HostelMealManagement.Application.Attributes;
/// <summary>
/// Validates that the value of the decorated date property is greater than the value of a specified start date
/// property.
/// </summary>
/// <remarks>Use this attribute to enforce that one date property (such as an end date) occurs after another date
/// property (such as a start date) within the same object. If the start date property does not exist or is not of type
/// DateTimeOffset, validation will fail. This attribute is typically used in data models for input validation
/// scenarios, such as in ASP.NET Core MVC or data annotation-based validation.</remarks>
/// <param name="startDateProperty">The name of the property that contains the start date to compare against. This property must exist on the validated
/// object and be of type DateTimeOffset.</param>
public class DateGreaterThanAttribute(string startDateProperty) : ValidationAttribute
{
    private readonly string _startDateProperty = startDateProperty;
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        var endDate = (DateTimeOffset)value;
        var startDateProperty = validationContext.ObjectType.GetProperty(_startDateProperty);

        if (startDateProperty == null)
            return new ValidationResult($"Unknown property: {_startDateProperty}");

        var startDate = (DateTimeOffset)startDateProperty.GetValue(validationContext.ObjectInstance)!;

        if (endDate <= startDate)
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}
