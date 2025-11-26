using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class Member : AuditableEntity
{
    
    public string Name { get; set; }
    public string FatherName { get; set; }
    public string MotherName { get; set; }
    public string PermanentAddress { get; set; }
    public string PresentAddress { get; set; }
    public DateTimeOffset Date { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Religion { get; set; }
    public string NID { get; set; }
    public bool MealStatus { get; set; }
    public string Picture { get; set; }
}
