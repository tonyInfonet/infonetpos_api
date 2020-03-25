using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ITaxExemptSaleLineManager
    {
        /// <summary>
        /// Method to make default tax exempt sale line
        /// </summary>
        /// <param name="saleLine">Sale line</param>
        /// <param name="theSystem">Tax exempt system</param>
        /// <returns>True or false</returns>
        bool MakeTaxExemptLine(ref TaxExemptSaleLine saleLine, ref teSystem theSystem);
    }
}