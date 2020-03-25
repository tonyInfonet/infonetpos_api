using System;
namespace Infonet.CStoreCommander.Entities
{
    public class GCTender
    {

        private short mvarLine_No;
        private string mvarCertificateNum;
        private string mvarRefNum;
        private string mvarCertType;
        private string mvarExpDate;
        private decimal mvarSaleAmount;
        private decimal mvarBalance;
        
        private DateTime mvarTransactionTime;
        private string mvarTermID; 
        private string mvarSequence; 
                                     
        private string mvarMessage;


        public string Message
        {
            get
            {
                string returnValue = "";
                returnValue = mvarMessage;
                return returnValue;
            }
            set
            {
                mvarMessage = value;
            }
        }
        


        public string CertificateNum
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCertificateNum;
                return returnValue;
            }
            set
            {
                mvarCertificateNum = value;
            }
        }
        //

        public string RefNum
        {
            get
            {
                string returnValue = "";
                returnValue = mvarRefNum;
                return returnValue;
            }
            set
            {
                mvarRefNum = value;
            }
        }
        //

        public string ExpDate
        {
            get
            {
                string returnValue = "";
                returnValue = mvarExpDate;
                return returnValue;
            }
            set
            {
                mvarExpDate = value;
            }
        }


        public string CertType
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCertType;
                return returnValue;
            }
            set
            {
                mvarCertType = value;
            }
        }

        

        public string TermID
        {
            get
            {
                string returnValue = "";
                returnValue = mvarTermID;
                return returnValue;
            }
            set
            {
                mvarTermID = value;
            }
        }


        public string Sequence
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSequence;
                return returnValue;
            }
            set
            {
                mvarSequence = value;
            }
        }
        


        public short Line_No
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarLine_No;
                return returnValue;
            }
            set
            {
                mvarLine_No = value;
            }
        }


        public decimal SaleAmount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarSaleAmount;
                return returnValue;
            }
            set
            {
                mvarSaleAmount = value;
            }
        }
        //

        public decimal Balance
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarBalance;
                return returnValue;
            }
            set
            {
                mvarBalance = value;
            }
        }

        
        
        
        
        
        
        


        public DateTime TransactionTime
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarTransactionTime;
                return returnValue;
            }
            set
            {
                mvarTransactionTime = value;
            }
        }
    }
}
