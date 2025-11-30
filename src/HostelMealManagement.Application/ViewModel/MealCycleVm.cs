using AutoMapper;
using HostelMealManagement.Application.Attributes;
using HostelMealManagement.Core.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace HostelMealManagement.Application.ViewModel;

[AutoMap(typeof(Core.Entities.MealCycle), ReverseMap = true)]
public class MealCycleVm:BaseEntity
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name must be less than 100 characters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Start date is required.")]
    public DateTimeOffset StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    [DateGreaterThan("StartDate", ErrorMessage = "End Date must be greater than Start Date.")]
    public DateTimeOffset EndDate { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Total days must be at least 1.")]
    public int TotalDays { get; set; }
}
