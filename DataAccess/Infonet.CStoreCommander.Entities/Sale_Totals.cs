using System;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Sale_Totals : IDisposable
    {

        private decimal mvarGross;
        private decimal mvarNet;
        private decimal mvarPayment;
        private decimal mvarPayOut;
        private decimal mvarCharge;
        private decimal mvarInvoice_Discount;
        private string mvarInvoice_Discount_Type;
        private float mvarDiscount_Percent; //  
        private Sale_Taxes mvarSale_Taxes;

        // TODO: UI Controls commented - Ipsit_1
        //Smriti: 5 removed UI properties
        private decimal mvarTotalLabel;
        //private System.Windows.Forms.Label mvarSummaryLabel;
        //private System.Windows.Forms.Label mvarTopLeft;
        //private System.Windows.Forms.Label mvarTopRight;


        private decimal mvarPenny_Adj; //  

        public decimal Total
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarTotalLabel;
                return returnValue;
            }
            set
            {
                mvarTotalLabel = value;
            }
        }

        public string SummaryLabel { get; set; }

        public string TotalLabel { get; set; }

        public string NetRefundLabel { get; set; }

        public string TaxesLabel { get; set; }

        public string ChargesLabel { get; set; }

        public string NetSaleLabel { get; set; }

        public int SaleNumber { get; set; }

        //public System.Windows.Forms.Label TopLeft
        //{
        //    get
        //    {
        //        System.Windows.Forms.Label returnValue = default(System.Windows.Forms.Label);
        //        returnValue = mvarTopLeft;
        //        return returnValue;
        //    }
        //    set
        //    {
        //        mvarTopLeft = value;
        //        mvarTopLeft.Text = "";
        //        if (!(this.TopRight == null))
        //        {
        //            Top_Box();
        //        }
        //    }
        //}


        //public System.Windows.Forms.Label TopRight
        //{
        //    get
        //    {
        //        System.Windows.Forms.Label returnValue = default(System.Windows.Forms.Label);
        //        returnValue = mvarTopRight;
        //        return returnValue;
        //    }
        //    set
        //    {
        //        mvarTopRight = value;
        //        mvarTopRight.Text = "";
        //        if (!(this.TopLeft == null))
        //        {
        //            Top_Box();
        //        }

        //    }
        //}


        //public System.Windows.Forms.Label SummaryLabel
        //{
        //    get
        //    {
        //        System.Windows.Forms.Label returnValue = default(System.Windows.Forms.Label);
        //        returnValue = mvarSummaryLabel;
        //        return returnValue;
        //    }
        //    set
        //    {
        //        mvarSummaryLabel = value;
        //        mvarSummaryLabel.Text = "";
        //    }
        //}

        public Sale_Taxes Sale_Taxes
        {
            get
            {
                Sale_Taxes returnValue = default(Sale_Taxes);
                if (mvarSale_Taxes == null)
                {
                    mvarSale_Taxes = new Sale_Taxes();
                }
                returnValue = mvarSale_Taxes;
                return returnValue;
            }
            set
            {
                mvarSale_Taxes = value;
            }
        }




        public string Invoice_Discount_Type
        {
            get
            {
                string returnValue = "";
                returnValue = mvarInvoice_Discount_Type;
                return returnValue;
            }
            set
            {
                mvarInvoice_Discount_Type = value;
            }
        }



        public decimal Invoice_Discount
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarInvoice_Discount;
                return returnValue;
            }
            set
            {
                mvarInvoice_Discount = value;
            }
        }


        // end changes

        public decimal PayOut
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPayOut;
                return returnValue;
            }
            set
            {
                mvarPayOut = value;
            }
        }
        //  

        public float Discount_Percent
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarDiscount_Percent;
                return returnValue;
            }
            set
            {
                mvarDiscount_Percent = value;
            }
        }




        public decimal Payment
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPayment;
                return returnValue;
            }
            set
            {
                mvarPayment = value;
            }
        }




        public double Charge
        {
            get
            {
                double returnValue = 0;
                returnValue = (double)mvarCharge;
                return returnValue;
            }
            set
            {
                mvarCharge = (decimal)value;
            }
        }



        public decimal Net
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarNet;
                return returnValue;
            }
            set
            {
                mvarNet = value;
                this.Gross = value;
            }
        }


        public decimal Gross
        {
            get; set;
            //{
                //Smriti move this code to manager
                //Sale_Tax STX = default(Sale_Tax);
                //decimal OldGross = new decimal();
                //decimal Tax_Rebate = new decimal();
                //decimal Tax_Exemption = new decimal();
                //OldGross = mvarGross;
                //mvarGross = value;
                //Tax_Rebate = 0;
                //Tax_Exemption = 0;

                //foreach (Sale_Tax tempLoopVar_STX in this.Sale_Taxes)
                //{
                //    STX = tempLoopVar_STX;
                //    // agencies
                //    if (policyManager.TAX_EXEMPT_GA)
                //    {
                //        mvarGross = mvarGross + STX.Tax_Added_Amount - STX.Tax_Rebate - STX.Tax_Exemption_GA_Incl;
                //        Tax_Exemption = Tax_Exemption + STX.Tax_Exemption_GA_Incl + STX.Tax_Exemption_GA_Added;
                //    }
                //    else
                //    {
                //        //   end
                //        mvarGross = mvarGross + STX.Tax_Added_Amount - STX.Tax_Rebate; //   added Tax_Rebate deduction
                //    }
                //    Tax_Rebate = Tax_Rebate + STX.Tax_Rebate; //  
                //}
                //mvarGross = mvarGross + (decimal)this.Charge;

                //STX = null;

                // TODO: Sets the text and font of the sale totals - Ipsit_4 

                //if (!(this.TotalLabel == null))
                //{
                //    if (OldGross != mvarGross || this.TotalLabel.Text.Length == 0)
                //    {

                //if (Microsoft.VisualBasic.Compatibility.VB6.Support.Format(mvarGross, "###,##0.00").Length > 8)
                //{
                //    this.TotalLabel.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeSize(this.TotalLabel.Font, 36);
                //}
                //else
                //{
                //    this.TotalLabel.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeSize(this.TotalLabel.Font, 40);
                //}
                //this.TotalLabel.Text = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(mvarGross, "###,##0.00");


                //    }
                //}

                // END: Sets the text and font of the sale totals - Ipsit_4 

                // TODO: Setting the Summary text including Taxes, Discounts & Associated charges - Ipsit_5

                // Summarize Taxes, Discounts & Associated charges.
                //if (!(mvarSummaryLabel == null))
                //{
                //    mvarSummaryLabel.Text = "";
                //    foreach (Sale_Tax tempLoopVar_STX in this.Sale_Taxes)
                //    {
                //        STX = tempLoopVar_STX;
                //        if (STX.Tax_Added_Amount != 0)
                //        {
                //            mvarSummaryLabel.Text = mvarSummaryLabel.Text + STX.Tax_Name + "     $" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(STX.Tax_Added_Amount, "###,##0.00") + "    ";
                //        }
                //        if (STX.Tax_Included_Amount != 0)
                //        {
                //            mvarSummaryLabel.Text = mvarSummaryLabel.Text + STX.Tax_Name + "(I)  $" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(STX.Tax_Included_Amount, "###,##0.00") + "    ";
                //        }
                //    }
                //    if (this.Charge != 0)
                //    {
                //        mvarSummaryLabel.Text = mvarSummaryLabel.Text + Chaps_Main.GetResString((short)264) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(this.Charge, "###,##0.00") + "    "; //"Chg $"
                //    }
                //    if (this.Invoice_Discount != 0)
                //    {
                //        mvarSummaryLabel.Text = mvarSummaryLabel.Text + Chaps_Main.GetResString((short)265) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(this.Invoice_Discount, "###,##0.00") + "    "; //"Discount $"
                //    }
                //    //   don't check the policy, if is false, Tax_Rebate amount should be 0
                //    if (Tax_Rebate != 0)
                //    {
                //        mvarSummaryLabel.Text = mvarSummaryLabel.Text + Chaps_Main.GetResString((short)474) + "  " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Tax_Rebate, "$###,##0.00") + "    ";
                //    }
                //    //   end
                //    //  
                //    if (Tax_Exemption != 0)
                //    {
                //        mvarSummaryLabel.Text = mvarSummaryLabel.Text + Chaps_Main.GetResString((short)1712) + "  " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Tax_Exemption, "$###,##0.00") + "    ";
                //    }
                //    //   end

                //}

                // END: Setting the Summary text including Taxes, Discounts & Associated charges - Ipsit_5
           // }
        }


        public decimal Penny_Adj
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPenny_Adj;
                return returnValue;
            }
            set
            {
                mvarPenny_Adj = value;
            }
        }

        private void Class_Initialize_Renamed()
        {
            mvarSale_Taxes = new Sale_Taxes();

            // TODO: Setting of UI Controls to null - Ipsit_2

            //mvarTotalLabel = null;
            //mvarTopLeft = null;
            //mvarTopRight = null;

            // END: Setting of UI Controls to null - Ipsit_2
            mvarGross = 0;
            mvarNet = 0;
            mvarCharge = 0;
        }

        public Sale_Totals()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            // TODO: Setting of UI Controls to null - Ipsit_3

            //mvarSale_Taxes = null;
            //mvarTotalLabel = null;
            //mvarTopLeft = null;
            //mvarTopRight = null;

            // END: Setting of UI Controls to null - Ipsit_3
        }

        private void Top_Box()
        {
            Sale_Tax Sale_Tax_Renamed = default(Sale_Tax);
            decimal curTotalTaxes = new decimal();
            curTotalTaxes = 0;
            //Smriti move this code to manager
            var topLeftText = string.Empty;
            var topRightText = string.Empty;

            //if (this.Gross < 0)
            //{
            //    topLeftText = Chaps_Main.GetResString((short)266) + "\r\n"; //"Net Refund "
            //    topRightText = this.Net.ToString("###,##0.00") + "\r\n";

            //    foreach (Sale_Tax tempLoopVar_Sale_Tax_Renamed in this.Sale_Taxes)
            //    {
            //        Sale_Tax_Renamed = tempLoopVar_Sale_Tax_Renamed;
            //        if ((double)System.Math.Abs(Sale_Tax_Renamed.Tax_Added_Amount) > 0.005)
            //        {
            //            //                mvarTopLeft.Caption = mvarTopLeft.Caption & Sale_Tax.Tax_Name & vbCrLf
            //            //                mvarTopRight.Caption = mvarTopRight.Caption & Format(Sale_Tax.Tax_Added_Amount, "###,##0.00") & vbCrLf
            //            curTotalTaxes = curTotalTaxes + Sale_Tax_Renamed.Tax_Added_Amount;
            //        }
            //    }
            //    topLeftText = topLeftText + Chaps_Main.GetResString((short)137) + "\r\n";
            //    topRightText = topRightText + curTotalTaxes.ToString("###,##0.00") + "\r\n";

            //    if (this.Charge != 0)
            //    {
            //        topLeftText = topLeftText + Chaps_Main.GetResString((short)138) + "\r\n"; //"Charges "
            //        topRightText = topRightText + this.Charge.ToString("###,##0.00") + "\r\n";
            //    }
            //    topLeftText = topLeftText + "\r\n";
            //    topRightText = topRightText + "________" + "\r\n";

            //    topLeftText = topLeftText + Chaps_Main.GetResString((short)210); //"Total"
            //    topRightText = topRightText + this.Gross.ToString("###,##0.00");
            //}
            //else
            //{
            //    topLeftText = Chaps_Main.GetResString((short)267) + " : " + "\r\n"; //Net Sale
            //    topRightText = this.Net.ToString("###,##0.00") + "\r\n";

            //    foreach (Sale_Tax tempLoopVar_Sale_Tax_Renamed in this.Sale_Taxes)
            //    {
            //        Sale_Tax_Renamed = tempLoopVar_Sale_Tax_Renamed;
            //        if ((double)System.Math.Abs(Sale_Tax_Renamed.Tax_Added_Amount) > 0.005)
            //        {
            //            //                mvarTopLeft.Caption = mvarTopLeft.Caption & Sale_Tax.Tax_Name & vbCrLf
            //            //                mvarTopRight.Caption = mvarTopRight.Caption & Format(Sale_Tax.Tax_Added_Amount, "###,##0.00") & vbCrLf
            //            curTotalTaxes = curTotalTaxes + Sale_Tax_Renamed.Tax_Added_Amount;
            //        }
            //    }
            //    topLeftText = topLeftText + Chaps_Main.GetResString((short)137) + "\r\n";
            //    topRightText = topRightText + curTotalTaxes.ToString("###,##0.00") + "\r\n";

            //    if (this.Charge != 0)
            //    {
            //        topLeftText = topLeftText + Chaps_Main.GetResString((short)138) + "\r\n"; //"Charges "
            //        topRightText = topRightText + this.Charge.ToString("###,##0.00") + "\r\n";
            //    }
            //    topLeftText = topLeftText + "\r\n";
            //    topRightText = topRightText + "________" + "\r\n";

            //    topLeftText = topLeftText + Chaps_Main.GetResString((short)210); // "Total"
            //    topRightText = topRightText + this.Gross.ToString("###,##0.00");
            //}

            // TODO: Updates the text of UI Controls - Ipsit_7

            //mvarTopLeft.Text = topLeftText;
            //mvarTopRight.Text = topRightText;

            // END: Updates the text of UI Controls - Ipsit_7

        }

        public void Dispose()
        {

        }
        ~Sale_Totals()
        {
            Class_Terminate_Renamed();
        }
    }
}
