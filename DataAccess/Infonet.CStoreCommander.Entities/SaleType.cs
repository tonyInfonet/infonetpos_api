namespace Infonet.CStoreCommander.Entities
{
    public class Sale_Type
    {

        private string mvarSaleType;
        private short mvarPRINT_COPIES;


        public string SaleType
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSaleType;
                return returnValue;
            }
            set
            {
                mvarSaleType = value;
                LoadSaleTypePolicies();
            }
        }

        

        public short PRINT_COPIES
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarPRINT_COPIES;
                return returnValue;
            }
            set
            {
                mvarPRINT_COPIES = value;
            }
        }

        private void LoadSaleTypePolicies()
        {
            //Smriti move this code to manager
            //string temp_Policy_Name = "PRINT_COPIES";
            //mvarPRINT_COPIES = System.Convert.ToInt16(modPolicy.GetPol(temp_Policy_Name, this));
        }
        
    }
}
