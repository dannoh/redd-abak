using System.Collections.Generic;

namespace AbakHelper.XeroIntegration.Models
{
    public class ExecutionResult
    {
        public bool IsScuccess { get; set; }
        public List<string> Errors { get; set; }
    }
}
