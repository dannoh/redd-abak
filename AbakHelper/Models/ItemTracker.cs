using System.Collections.Generic;

namespace AbakHelper.Models
{
    public class ItemTracker
    {
        private readonly List<InvoiceItem> _items = new List<InvoiceItem>();

        public void AddItem(Project project, string description, double quantity, bool isHourly)
        {
            _items.Add(new InvoiceItem {Project = project, Description = description, Quantity = quantity, IsHourly = isHourly});
        }
    }

    public class InvoiceItem
    {
        public Project Project { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public bool IsHourly { get; set; }
    }
}
