namespace PackageFlow.Core.Helpers
{
    public static class TrackingNumberHelper
    {
        public static string Generate()
        {
            return $"PKF{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        }
    }
}
