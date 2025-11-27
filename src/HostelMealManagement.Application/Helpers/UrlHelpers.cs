namespace HostelMealManagement.Application.Helpers;

public static class UrlHelpers
{
    public static string GetEventImageUrl(this string imageUrl, string location = null)
    {
        string placeholder = "~/images/default.png";
        try
        {
            // Determine final path
            string finalPath = !string.IsNullOrEmpty(location) ? location.TrimEnd('/') : "";

            // Return image URL or placeholder
            return !string.IsNullOrEmpty(imageUrl) && !string.IsNullOrEmpty(finalPath)
                ? $"{finalPath}/{imageUrl}"
                : placeholder;
        }
        catch (Exception ex)
        {
            // Log exception and fallback to placeholder
            Console.WriteLine($"[GetEventImageUrl] Error: {ex.Message}");
            return placeholder;
        }
    }
}
