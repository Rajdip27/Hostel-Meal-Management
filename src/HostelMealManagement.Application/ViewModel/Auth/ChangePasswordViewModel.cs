using System.ComponentModel.DataAnnotations;

namespace HostelMealManagement.Application.ViewModel.Auth
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Password not matched")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
