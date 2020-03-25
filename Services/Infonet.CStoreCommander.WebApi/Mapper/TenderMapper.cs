using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.WebApi.Models.Tender;

namespace Infonet.CStoreCommander.WebApi.Mapper
{
    /// <summary>
    /// Tender summary mapper
    /// </summary>
    public static class TenderMapper
    {
        /// <summary>
        /// Tender summary mapper
        /// </summary>
        /// <param name="updatedTenders"></param>
        /// <param name="issueStoreCreditMessage"></param>
        /// <param name="selectedTenders"></param>
        /// <returns></returns>
        public static TenderSummaryModel GetTenderSummaryModel(Tenders updatedTenders, string issueStoreCreditMessage,
           IEnumerable<Tender> selectedTenders)
        {
            var tenderSummary = new TenderSummaryModel
            {
                Summary1 = updatedTenders.Summary1,
                Summary2 = updatedTenders.Summary2,
                OutstandingAmount = Math.Round(Convert.ToDouble(updatedTenders.Tend_Totals.Change), 2) > 0.0D
                    ? updatedTenders.Tend_Totals.Change.ToString(Constants.ChangeFormat)
                    : 0.ToString(Constants.ChangeFormat),
                EnableCompletePayment = updatedTenders.EnableCompletePayment,
                DisplayNoReceiptButton = updatedTenders.DisplayNoReceiptButton,
                IssueStoreCreditMessage = issueStoreCreditMessage,
                EnableRunAway = updatedTenders.EnableRunAway,
                EnablePumpTest = updatedTenders.EnablePumpTest,
                Tenders = (from tender in selectedTenders
                           select new TenderResponseModel
                           {
                               TenderCode = tender.Tender_Code,
                               TenderName = tender.Tender_Name,
                               TenderClass = tender.Tender_Class,
                               AmountEntered = tender.Amount_Entered == 0 ? "" : tender.Amount_Entered.ToString(Constants.CurrencyFormat),
                               AmountValue = tender.Amount_Used == 0 ? "" : tender.Amount_Used.ToString(Constants.CurrencyFormat),
                               IsEnabled = string.IsNullOrEmpty(tender.Credit_Card.Cardnumber),
                               MaximumValue = tender.MaxAmount,
                               MinimumValue = tender.MinAmount,
                               Image = tender.Image
                           }).ToList(),
                CustomerDisplay = updatedTenders.CustomerDisplay
            };
            return tenderSummary;
        }
    }
}