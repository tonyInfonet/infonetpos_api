namespace Infonet.CStoreCommander.Entities
{

    public class PumpControl
    {
        public int PumpId { get; set; }

        public string Status { get; set; }

        public string PumpButtonCaption { get; set; }

        public string BasketButtonCaption { get; set; }


        public int BasketButtonVisible { get; set; }


        public string BasketLabelCaption { get; set; }


        public dynamic PayPumporPrepay { get; set; }


        public string PrepayText { get; set; }


        public dynamic EnableBasketBotton { get; set; }


        public dynamic EnableStackBasketBotton { get; set; }


        public bool CanCashierAuthorize { get; set; }


        public void SetPumpStatus(byte pumpID, string statusStr)
        {
            //display different picture for different status
            switch (statusStr)
            {
                case "1": //idle, preset idle
                case "S":
                    Status = "Idle";
                    break;
                case "2": //calling, preset calling
                case "T":
                    Status = "Calling";
                    break;
                case "3": //authorized ' Binal added on 10/28/2011 for NextGen FCS
                case "L":
                    Status = "Authorized";
                    break;
                case "4": //pumping
                    Status = "Pumping";
                    break;
                case "8": //finished
                    Status = "Finished";
                    break;
                case "7": //inactive
                    Status = "Inactive";
                    break;
                case "5": //stopped
                    Status = "Stopped";
                    break;
                case "6": //Runaway
                    break;
                case "P": //paypump pumping
                    Status = "Pumping";
                    break;
                case "F": //paypump finished
                    Status = "Finished";
                    break;
                case "H": //Paypump idle
                    Status = "Idle";
                    break;
                case "C": //Paypump calling
                    Status = "Calling";
                    break;
                case "U": //Paypump authorized
                    Status = "Finished";
                    break;
                case "A": //Prepay pumping
                    Status = "Pumping";
                    break;
                case "R": //Prepay finished
                    Status = "Finished";
                    break;
                case "I": //Prepay idle
                    Status = "Idle";
                    break;
                case "O": //Prepay authorized
                    Status = "Authorized";
                    break;
            }
        }

                
    }



}
