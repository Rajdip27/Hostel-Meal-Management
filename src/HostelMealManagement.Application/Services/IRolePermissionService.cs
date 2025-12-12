namespace HostelMealManagement.Application.Services;

public interface IRolePermissionService
{
    bool CanCreate(List<string> roles);
    bool CanEdit(List<string> roles);
    bool CanDelete(List<string> roles);
    bool CanView(List<string> roles);
}

public class RolePermissionService : IRolePermissionService
{
    private readonly Dictionary<string, (bool Create, bool Edit, bool Delete, bool View)> _rules =
        new()
        {
            { "Admin",   (true, true, true, true) },
            { "Manager", (true, true, false, true) },
            { "User",    (false, false, false, true) }
        };

    private bool HasPermission(List<string> roles, Func<(bool Create, bool Edit, bool Delete, bool View), bool> selector)
    {
        foreach (var role in roles)
        {
            if (_rules.ContainsKey(role))
            {
                if (selector(_rules[role]))
                    return true;
            }
        }
        return false;
    }

    public bool CanCreate(List<string> roles) =>
        HasPermission(roles, perm => perm.Create);

    public bool CanEdit(List<string> roles) =>
        HasPermission(roles, perm => perm.Edit);

    public bool CanDelete(List<string> roles) =>
        HasPermission(roles, perm => perm.Delete);

    public bool CanView(List<string> roles) =>
        HasPermission(roles, perm => perm.View);
}

