using HostelMealManagement.Core.Entities.BaseEntities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelMealManagement.Application.ViewModel;

public class MemberVm:BaseEntity
{
    [Required(ErrorMessage = "Member Code is required.")]
    [StringLength(20, ErrorMessage = "Member Code cannot exceed 20 characters.")]
    [Display(Name = "Member Code")]
    public string MemberCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Father's Name is required.")]
    [StringLength(100)]
    [Display(Name = "Father's Name")]
    public string FatherName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mother's Name is required.")]
    [StringLength(100)]
    [Display(Name = "Mother's Name")]
    public string MotherName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Permanent Address is required.")]
    [StringLength(200)]
    [Display(Name = "Permanent Address")]
    public string PermanentAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Present Address is required.")]
    [StringLength(200)]
    [Display(Name = "Present Address")]
    public string PresentAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Joining Date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Joining Date")]
    public DateTimeOffset JoiningDate { get; set; }

    [Required(ErrorMessage = "Date of Birth is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTimeOffset DateOfBirth { get; set; }

    [Required(ErrorMessage = "Phone Number is required.")]
    [Phone(ErrorMessage = "Invalid phone number.")]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Emergency Contact is required.")]
    [Phone(ErrorMessage = "Invalid emergency contact number.")]
    [Display(Name = "Emergency Contact")]
    public string EmergencyContact { get; set; } = string.Empty;

    [Required(ErrorMessage = "Emergency Name is required.")]
    [StringLength(100)]
    [Display(Name = "Emergency Name")]
    public string EmergencyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Relationship is required.")]
    [StringLength(50)]
    public string Relationship { get; set; } = string.Empty;

    [Required(ErrorMessage = "Religion is required.")]
    [StringLength(50)]
    public string Religion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required.")]
    [StringLength(10)]
    public string Gender { get; set; } = string.Empty;

    [Display(Name = "Marital Status")]
    public bool MaritalStatus { get; set; }

    [Required(ErrorMessage = "NID is required.")]
    [StringLength(20)]
    public string NID { get; set; } = string.Empty;

    [Display(Name = "Picture")]
    public string Picture { get; set; } = string.Empty;

    [Display(Name = "Meal Status")]
    public bool MealStatus { get; set; }

    [Required(ErrorMessage = "House Bill is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "House Bill must be a positive number.")]
    [DataType(DataType.Currency)]
    [Display(Name = "House Bill")]
    public decimal HouseBill { get; set; }

    [Required(ErrorMessage = "Utility Bill is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Utility Bill must be a positive number.")]
    [DataType(DataType.Currency)]
    [Display(Name = "Utility Bill")]
    public decimal UtilityBill { get; set; }

    [Required(ErrorMessage = "Other Bill is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Other Bill must be a positive number.")]
    [DataType(DataType.Currency)]
    [Display(Name = "Other Bill")]
    public decimal OtherBill { get; set; }
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    [NotMapped]
    public string? Password { get; set; } = string.Empty;    

}
