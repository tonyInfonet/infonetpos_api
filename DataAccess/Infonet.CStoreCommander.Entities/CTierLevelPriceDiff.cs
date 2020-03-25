namespace Infonet.CStoreCommander.Entities
{
    public class CTierLevelPriceDiff
    {
        private object myCashDiff;
        private object myCreditDiff;

        private object _taxExemptCashDiff;
        private object _taxExemptCreditDiff;

        public dynamic CashDiff
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myCashDiff;
                return returnValue;
            }
            set
            {
                myCashDiff = value;
            }
        }


        public dynamic CreditDiff
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myCreditDiff;
                return returnValue;
            }
            set
            {
                myCreditDiff = value;
            }
        }

        public object TaxExemptCashDiff
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = _taxExemptCashDiff;
                return returnValue;
            }

            set
            {
                _taxExemptCashDiff = value;
            }
        }

        public object TaxExemptCreditDiff
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = _taxExemptCreditDiff;
                return returnValue;
            }

            set
            {
                _taxExemptCreditDiff = value;
            }
        }
    }
}
