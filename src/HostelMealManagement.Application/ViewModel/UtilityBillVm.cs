using AutoMapper;
using HostelMealManagement.Core.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace HostelMealManagement.Application.ViewModel;

[AutoMap(typeof(Core.Entities.UtilityBill), ReverseMap = true)]
public class UtilityBillVm : BaseEntity
{
    [Required(ErrorMessage = "Month is required.")]
    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
    public int Month { get; set; }

    [Required(ErrorMessage = "Date is required.")]
    public DateTimeOffset Date { get; set; }

    [Required(ErrorMessage = "Current bill amount is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Current bill amount must be positive.")]
    public decimal CurrentBill { get; set; }

    [Required(ErrorMessage = "Gas bill amount is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Gas bill amount must be positive.")]
    public decimal GasBill { get; set; }

    [Required(ErrorMessage = "Servant bill amount is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Servant bill amount must be positive.")]
    public decimal ServantBill { get; set; }
}
