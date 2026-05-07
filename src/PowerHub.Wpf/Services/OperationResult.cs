namespace PowerHub.UI.Services
{
    public sealed class OperationResult
    {
        public bool Success { get; init; }
        public string? Message { get; init; }

        public static OperationResult Ok(string? message = null) =>
            new OperationResult { Success = true, Message = message };

        public static OperationResult Fail(string message) =>
            new OperationResult { Success = false, Message = message };
    }
}
