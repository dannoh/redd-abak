using System;

namespace AbakHelper.XeroIntegration.Models
{
    public class ProjectTask
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DisplayDesc { get; set; }
        public bool IsBillable { get; set; }
        public bool IsHourly { get; set; }
        public double Quantity { get; set; }
        public DateTime Date { get; set; }
        public Project Project { get; set; }
        public ProjectTaskDefinition TaskDefinition { get; set; }
    }
}
