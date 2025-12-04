using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class Member : AuditableEntity
{
    public string MemberCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string MotherName { get; set; } = string.Empty;
    public string PermanentAddress { get; set; } = string.Empty;
    public string PresentAddress { get; set; } = string.Empty;
    public DateTimeOffset JoiningDate { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }=string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmergencyContact { get; set; } = string.Empty;
    public string EmergencyName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string Religion { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public bool MaritalStatus { get; set; } 
    public string NID { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
    public bool MealStatus { get; set; }
    public decimal HouseBill { get; set; }
    public decimal UtilityBill { get; set; }
    public decimal OtherBill { get; set; }

    public ICollection<MealAttendance> MealAttendances { get; set; }
}
