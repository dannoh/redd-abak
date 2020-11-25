namespace AbakHelper.XeroIntegration.Models
{
    public class InvoiceLineItem
    {
        public string Description { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal Quantity { get; set; }
    }
}