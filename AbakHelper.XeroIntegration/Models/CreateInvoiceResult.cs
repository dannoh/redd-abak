using AbakHelper.Integration;

namespace AbakHelper.XeroIntegration.Models
{
    public class CreateInvoiceResult : ExecutionResult
    {
        public object Invoice { get; set; }
    }
}