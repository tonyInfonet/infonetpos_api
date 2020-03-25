using Infonet.CStoreCommander.Entities;
using System.Collections.Generic;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.BusinessLayer.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ITaxManager
    {
        /// <summary>
        /// Get Taxes
        /// </summary>
        /// <returns></returns>
        List<string> GetTaxes();

        /// <summary>
        /// Method to load taxes for payout
        /// </summary>
        /// <param name="payoutTaxes"></param>
        void Load_Taxes(ref Payout_Taxes payoutTaxes);

        /// <summary>
        /// Remove Tax
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="treatyNumber">Treaty number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="intcaptureMethod">Capture method</param>
        /// <param name="documentNumber">Document number</param>
        /// <param name="error">Error</param>
        /// <returns>Treaty number response</returns>
        TreatyNumberResponse RemoveTax(int saleNumber, int tillNumber, string treatyNumber, byte registerNumber,
            string userCode, short intcaptureMethod, string documentNumber, out ErrorMessage error);


        /// <summary>
        /// Validate treaty number response
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="treatyNumber">Treaty number</param>
        /// <param name="treatyName">Treaty name</param>
        /// <param name="documentNumber">Document number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="intcaptureMethod">Capture method</param>
        /// <param name="isEnterPressed">Enter pressed or not</param>
        /// <param name="error">Error</param>
        /// <returns>Treaty number response</returns>
        TreatyNumberResponse Validate(int saleNumber, int tillNumber, string treatyNumber,
            string treatyName, string documentNumber, byte registerNumber, string userCode
            , short intcaptureMethod, bool isEnterPressed, out ErrorMessage error);

        /// <summary>
        /// VerifyTax Exempt
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>Verify tax exempt</returns>
        VerifyTaxExempt VerifyTaxExampt(int saleNumber, int tillNumber, int registerNumber, string userCode, out ErrorMessage error);


        /// <summary>
        /// Validate AITE Card
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="cardNumber">Card number</param>
        /// <param name="barCode">Bar code</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="checkMode">Check mode</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>AITE response</returns>
        AiteCardResponse ValidateAiteCard(int saleNumber, int tillNumber, int shiftNumber, string cardNumber, string barCode, byte registerNumber, byte checkMode, string userCode, out ErrorMessage error);

        /// <summary>
        /// Affix Bar Code
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="barCode">Bar code</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        bool AffixBarCode(string cardNumber, string barCode, out ErrorMessage error);

        /// <summary>
        /// Exempt AITE GST/PST
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="treatyNumber">Treaty number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>AITE card response</returns>
        AiteCardResponse AiteGstPstExempt(int saleNumber, int tillNumber, int shiftNumber, string treatyNumber,
            byte registerNumber, string userCode, out ErrorMessage error);

        /// <summary>
        /// Validate QITE Band Member
        /// </summary>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="bandMember">Band member</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error</param>
        /// <returns>QITE response</returns>
        QiteCardResponse ValidateQiteBandMember(int saleNumber, int tillNumber, int shiftNumber, byte registerNumber, string bandMember, string userCode, out ErrorMessage error);

        /// <summary>
        /// Get Sale Summary
        /// </summary>
        /// <param name="input">Sale summary</param>
        /// <param name="error">Error</param>
        /// <returns>Sale summary response</returns>
        SaleSummaryResponse GetSaleSummary(SaleSummaryInput input, out ErrorMessage error);

        /// <summary>
        /// Method to get treaty name
        /// </summary>
        /// <param name="treatyNumber">Treaty number</param>
        /// <param name="intcaptureMethod">Capture method</param>
        /// <param name="userCode">Userb code</param>
        /// <param name="error">Error</param>
        /// <returns>Treaty name</returns>
        string GetTreatyName(string treatyNumber, short intcaptureMethod, string userCode, out ErrorMessage error);


        /// <summary>
        /// Process FNGTR Sale
        /// </summary>
        /// <param name="tillNumber"></param>
        /// <param name="saleNumber"></param>
        /// <param name="registerNumber"></param>
        /// <param name="stfdNumber"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        TreatyNumberResponse ProcessFngtrSale(int tillNumber, int saleNumber, byte registerNumber, string stfdNumber, out ErrorMessage error);
    }
}
