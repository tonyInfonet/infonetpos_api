using System;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class CustomerDisplay
    {
        public CustomerDisplay()
        {
            NonOposTexts = new List<string>();
        }

        public string OposText1 { get; set; }

        public string OposText2 { get; set; }

        public List<string> NonOposTexts { get; set; }
    }
}
