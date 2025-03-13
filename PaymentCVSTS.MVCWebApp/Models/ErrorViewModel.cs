namespace PaymentCVSTS.MVCWebApp.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string? ErrorMessage { get; set; }
        public string? ExceptionType { get; set; }
        public string? StackTrace { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }
}