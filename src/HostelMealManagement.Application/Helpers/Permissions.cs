namespace HostelMealManagement.Application.Helpers;

public static class Permissions
{
    public const string Edit = "Admin,Manager";
    public const string Delete = "Admin";
    public const string View = "Admin,Manager,Member";
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Member = "Member";
}
