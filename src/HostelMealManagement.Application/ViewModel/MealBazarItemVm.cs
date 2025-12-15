using AutoMapper;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Core.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace HostelMealManagement.Application.ViewModel;

[AutoMap(typeof(MealBazarItem), ReverseMap = true)]
public class MealBazarItemVm : BaseEntity
{
    [Required(ErrorMessage = "Bazar reference is required.")]
    public long MealBazarId { get; set; }

    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(150, ErrorMessage = "Product name must be less than 150 characters.")]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public decimal Quantity { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }
}
