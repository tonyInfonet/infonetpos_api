using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ICreditCardManager
    {
        /// <summary>
        /// Method to authorize a card
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        void Authorize_Card(ref Credit_Card creditCard);

        /// <summary>
        /// Method to validate whether to call the bank or not
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>True or false</returns>
        bool Call_The_Bank(ref Credit_Card creditCard);

        /// <summary>
        /// Method to chek if card is negative card
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>True or false</returns>
        bool CardInNcf(ref Credit_Card creditCard);

        /// <summary>
        /// Method to check if card is positive card
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>True or false</returns>
        bool CardInPcf(ref Credit_Card creditCard);

        /// <summary>
        /// Method to check if card is valid or not
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>True or false</returns>
        bool CardIsValid(ref Credit_Card creditCard);

        /// <summary>
        /// Method to check if card is expired or not
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>True or false</returns>
        bool Card_Is_Expired(ref Credit_Card creditCard);

        /// <summary>
        /// Method to find tender code
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Tender code</returns>
        string Find_TenderCode(ref Sale sale, ref Credit_Card creditCard);

        /// <summary>
        /// Method to get card product codes
        /// </summary>
        /// <param name="cdSale">Sale</param>
        /// <param name="creditCard">Credit card</param>
        void GetCardProductCodes(ref Sale cdSale, Credit_Card creditCard);

        /// <summary>
        /// Method to get valid products for card
        /// </summary>
        /// <param name="cdSale">Sale</param>
        /// <param name="creditCard">Credit card</param>
        /// <param name="strLineNum">Line number</param>
        /// <returns>Valid amount</returns>
        float GetValidProductForCard(Sale cdSale, Credit_Card creditCard,
            ref string strLineNum);

        /// <summary>
        /// Method to insert data
        /// </summary>
        /// <param name="cdSale">Sale</param>
        /// <param name="cc">Credit card</param>
        /// <param name="fuelType">Fuel type</param>
        /// <returns>Amount</returns>
        decimal InsertData(Sale cdSale, Credit_Card cc, string fuelType);

        /// <summary>
        /// Method to set invalid credit card reason
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Reason</returns>
        string Invalid_Reason(ref Credit_Card creditCard);

        /// <summary>
        /// Method to set credit card language
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Language</returns>
        string Language(ref Credit_Card creditCard);

        /// <summary>
        /// Method to set product codes for a till
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Product codes</returns>
        string ProductCodes(int tillNumber);

        /// <summary>
        /// Method to get receipt total text
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Receipt total</returns>
        string ReceiptTotalText(ref Credit_Card creditCard);

        /// <summary>
        /// Method to return card codes
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Card code</returns>
        string Return_CardCode(ref Credit_Card creditCard);

        /// <summary>
        /// Method to return tender class
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Tender class</returns>
        string Return_TendClass(Credit_Card creditCard);

        /// <summary>
        /// Method to return tender description
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>Tender description</returns>
        string Return_TendDesc(Credit_Card creditCard);

        /// <summary>
        /// Method to set card number
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="cardNumber">Card number</param>
        void SetCardnumber(ref Credit_Card creditCard, string cardNumber);

        /// <summary>
        /// Method to set driver number
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="driverNumber">Driver number</param>
        void SetDriverNumber(ref Credit_Card creditCard, string driverNumber);

        /// <summary>
        /// Method to set Id number
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="idNumber">Id number</param>
        void SetIdNumber(ref Credit_Card creditCard, string idNumber);

        /// <summary>
        /// Method to set odometer number 
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="odoMeter">Odometer number</param>
        void SetOdoMeter(ref Credit_Card creditCard, string odoMeter);

        /// <summary>
        /// Method to set track 1
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="track1">Track 1</param>
        void SetTrack1(ref Credit_Card creditCard, string track1);

        /// <summary>
        /// Method to set track 2
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="track2">Track 2</param>
        void SetTrack2(ref Credit_Card creditCard, string track2);

        /// <summary>
        /// Method to set usage code
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="usageCode">Usage code</param>
        void SetUsageCode(ref Credit_Card creditCard, string usageCode);

        /// <summary>
        /// Method to set vehicle number
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="vehicleNumber">Vehicle number</param>
        void SetVehicleNumber(ref Credit_Card creditCard, string vehicleNumber);

        /// <summary>
        /// Method to check if valid customer card or not
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <returns>True or false</returns>
        bool ValidCustomerCard(ref Credit_Card creditCard);

        /// <summary>
        /// Method to set the swiped string
        /// </summary>
        /// <param name="creditCard">Credit card</param>
        /// <param name="swipeData">Swiped text</param>
        void SetSwipeString(ref Credit_Card creditCard, string swipeData);

        /// <summary>
        /// Method to load card types
        /// </summary>
        /// <returns>Card types</returns>
        CardTypes Load_CardTypes();

        /// <summary>
        /// Method to get tender card using tender code
        /// </summary>
        /// <param name="tenderCode">Tender code</param>
        /// <returns>Tender card</returns>
        TenderCard GetTendCard(string tenderCode);
    }
}