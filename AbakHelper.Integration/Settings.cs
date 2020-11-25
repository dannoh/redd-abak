using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AbakHelper.Integration
{
    
    public class Settings
    {
        public Dictionary<string, object> ComponentSettings { get; private set; }
        
        public string AbakUrl { get; set; }

        public Settings()
        {
            ComponentSettings = new Dictionary<string, object>();
        }

    }

}
