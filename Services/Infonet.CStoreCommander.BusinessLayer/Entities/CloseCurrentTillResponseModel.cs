using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class CloseCurrentTillResponseModel
    {

        public CloseCurrentTillResponseModel()
        {
            ShowBillCoins = true;
            ShowDifferenceField = true;
            ShowSystemField = true;
            ShowEnteredField = true;
            BillCoins = new List<BillCoin>();
            Tenders = new List<TillCloseTender>();
            Total = "0.00";
        }

        public bool ShowBillCoins { get; set; }

        public List<BillCoin> BillCoins { get; set; }

        public bool ShowEnteredField { get; set; }

        public bool ShowSystemField { get; set; }

        public bool ShowDifferenceField { get; set; }

        public List<TillCloseTender> Tenders { get; set; }

        public string Total { get; set; }

        public CustomerDisplay CustomerDisplay { get; set; }
    }

    public class TillCloseTender
    {
        public string Tender { get; set; }

        public string Count { get; set; }

        public string Entered { get; set; }

        public string System { get; set; }

        public string Difference { get; set; }
    }
}
