using HostelMealManagement.Infrastructure.Helper.Acls;

namespace HostelMealManagement.Application.Helpers;

public static class HtmlHelperExtensions
{
    /// <summary>
    /// Returns readonly attribute based on SignInHelper roles and a condition.
    /// </summary>
    /// <param name="signInHelper">Injected SignInHelper</param>
    /// <param name="isConditionTrue">Condition that makes the field readonly (e.g., entity.IsUserAssigned)</param>
    /// <param name="editableRoles">Roles allowed to edit</param>
    /// <returns>readonly string if not editable</returns>
    public static string ReadOnlyByRole(this ISignInHelper signInHelper, params string[] editableRoles)
    {
        bool hasRole = signInHelper.Roles.Any(r => editableRoles.Contains(r));

        return hasRole ? "" : "readonly title='You do not have permission to edit this field'";
    }
}
