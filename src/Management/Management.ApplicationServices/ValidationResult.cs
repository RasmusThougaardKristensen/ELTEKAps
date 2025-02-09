namespace ELTEKAps.Management.ApplicationServices
{
    public record ValidationResult(bool Success, string? Message)
    {
        public static ValidationResult Ok() => new(true, null);
        public static ValidationResult Fail(string message) => new(false, message);
    }
}
