using AutoMapper;
using HostelMealManagement.Application.Attributes;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Core.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace HostelMealManagement.Application.ViewModel;

[AutoMap(typeof(MealMenu), ReverseMap = true)]
public class MealMenuVm : BaseEntity
{
    [Required(ErrorMessage = "Day of week is required.")]
    public DayOfWeek DayOfWeek { get; set; }

    [Required(ErrorMessage = "Meal name is required.")]
    [StringLength(100, ErrorMessage = "Meal name must be less than 100 characters.")]
    public string MealName { get; set; } = null!;

    [Required(ErrorMessage = "Menu items are required.")]
    [StringLength(500, ErrorMessage = "Menu items must be less than 500 characters.")]
    public string MenuItems { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}
