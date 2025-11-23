using System.ComponentModel.DataAnnotations;

namespace HostelMealManagement.Application.ViewModel.Auth;

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
}
