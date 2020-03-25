namespace Infonet.CStoreCommander.Entities
{
    public class GCPayment
    {

        private int mvarSale_Num;
        private decimal mvarAmount;
        private GCTenders mvarGC_Lines;
        private byte mvarTill_Num;
        private object mvarDB;
        //TODO remove this DB to create connection
        //public ADODB.Connection DB
        //{
        //    set
        //    {
        //        mvarDB = value;
        //    }
        //}

        public object DB { get; set; }

        public GCTenders GC_Lines
        {
            get
            {
                GCTenders returnValue = default(GCTenders);
                if (mvarGC_Lines == null)
                {
                    mvarGC_Lines = new GCTenders();
                }
                returnValue = mvarGC_Lines;
                return returnValue;
            }
            set
            {
                mvarGC_Lines = value;
            }
        }


        public decimal Amount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarAmount;
                return returnValue;
            }
            set
            {
                mvarAmount = value;
            }
        }


        public int Sale_Num
        {
            get
            {
                int returnValue = 0;
                returnValue = mvarSale_Num;
                return returnValue;
            }
            set
            {
                mvarSale_Num = value;
            }
        }


        public byte Till_Num
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarTill_Num;
                return returnValue;
            }
            set
            {
                mvarTill_Num = value;
            }
        }


        public void Add_a_Line(GCTender oGCLine)
        {
            mvarGC_Lines.AddLine(oGCLine, "");
            mvarAmount = mvarAmount + oGCLine.SaleAmount;
        }

        private void Class_Initialize_Renamed()
        {
            GC_Initialize();
        }
        public GCPayment()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            mvarGC_Lines = null;
        }
        ~GCPayment()
        {
            Class_Terminate_Renamed();
            //base.Finalize();
        }     

        public void GC_Initialize()
        {
            mvarSale_Num = 0;
            mvarTill_Num = (byte)0;
            mvarAmount = 0;
            mvarGC_Lines = new GCTenders();
        }  

        private string GetFileName(bool SameSale)
        {
            string returnValue = "";
            //Smriti move this code to manager
            //    string strFileName = "";
            //    short i = 0;

            //    
            //    
            //    if (!SameSale)
            //    {
            //        
            //        
            //        strFileName = FileSystem.Dir((new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\MillipleinReceipt*.txt");
            //        while (!string.IsNullOrEmpty(strFileName))
            //        {
            //            Variables.DeleteFile((new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\" + strFileName);
            //            strFileName = FileSystem.Dir((new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\MillipleinReceipt*.txt");
            //        }
            //        strFileName = "MillipleinReceipt.txt";
            //    }
            //    else
            //    {
            //        
            //        
            //        strFileName = FileSystem.Dir((new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\MillipleinReceipt*.txt");
            //        if (string.IsNullOrEmpty(strFileName))
            //        {
            //            strFileName = "MillipleinReceipt.txt";
            //        }
            //        else
            //        {
            //            i = (short)0;
            //            while (!string.IsNullOrEmpty(strFileName))
            //            {
            //                i++;
            //                strFileName = FileSystem.Dir((new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\MillipleinReceipt" + (i).ToString() + "*.txt");
            //            }
            //            strFileName = "MillipleinReceipt" + (i).ToString() + ".txt";
            //        }
            //    }

            //    returnValue = strFileName;
            return returnValue;
        }
        
    }
}
