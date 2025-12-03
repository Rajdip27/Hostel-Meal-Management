using AutoMapper;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Core.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace HostelMealManagement.Application.ViewModel;

[AutoMap(typeof(MealBazar),ReverseMap =true)]
public class MealBazarVm:BaseEntity
{
    

    [Required(ErrorMessage = "Bazar Date is required.")]
    [DataType(DataType.Date)]
    public DateTimeOffset BazarDate { get; set; }

    [Required(ErrorMessage = "Bazar Amount is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
    public decimal BazarAmount { get; set; }

    [Display(Name = "Description")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }
   


}
