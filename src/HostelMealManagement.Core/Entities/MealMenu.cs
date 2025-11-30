using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities
{
    public class MealMenu : AuditableEntity
    {
        public DateTimeOffset Date { get; set; }

        public string BreakfastIteam { get; set; }
        public string LunchIteam { get; set; }
        public string DinnerIteam { get; set; }

        // Who updated the menu (username or ID)
        public string UpdateBy { get; set; }

        // Additional recommended fields:

        // If you want to know which manager created the menu:
        public int? ManagerId { get; set; }

        // Text note about the menu
        public string? Notes { get; set; }

        // For enabling/disabling a menu
        public bool IsActive { get; set; } = true;

        // Optional: Track if menu is finalized and locked
        public bool IsLocked { get; set; } = false;
    }
}



