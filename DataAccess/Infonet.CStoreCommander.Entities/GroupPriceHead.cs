using System;

namespace Infonet.CStoreCommander.Entities
{
    public class GroupPriceHead
    {

        public string Department { get; set; }

        public string SubDepartment { get; set; }

        public string SubDetail { get; set; }

        public char PrType { get; set; }

        public char PrUnit { get; set; }

        public DateTime PrFrom { get; set; }

        public DateTime PrTo { get; set; }
    }
}
