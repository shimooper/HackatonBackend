namespace AiCalendarBackend
{
    public class Utils
    {
        public static bool InvalidField(string? field)
        {
            return field == null || string.IsNullOrWhiteSpace(field) || field.ToLower() == "NaN".ToLower() ||
                   string.IsNullOrEmpty(field);
        }
    }
}
