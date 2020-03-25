namespace Infonet.CStoreCommander.Entities
{
    public class CGradePriceIncrement
    {
        private object myCashPriceIncre;
        private object myCreditPriceIncre;

        private object _taxExemptCashPriceIncre;
        private object _taxExemptCreditPriceIncre;


        public dynamic CashPriceIncre
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myCashPriceIncre;
                return returnValue;
            }
            set
            {
                myCashPriceIncre = value;
            }
        }


        public dynamic CreditPriceIncre
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = myCreditPriceIncre;
                return returnValue;
            }
            set
            {
                myCreditPriceIncre = value;
            }
        }

        public object TaxExemptCashPriceIncre
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = _taxExemptCashPriceIncre;
                return returnValue;
            }

            set
            {
                _taxExemptCashPriceIncre = value;
            }
        }

        public object TaxExemptCreditPriceIncre
        {
            get
            {
                dynamic returnValue = default(dynamic);
                returnValue = _taxExemptCreditPriceIncre;
                return returnValue;
            }

            set
            {
                _taxExemptCreditPriceIncre = value;
            }
        }
    }
}
