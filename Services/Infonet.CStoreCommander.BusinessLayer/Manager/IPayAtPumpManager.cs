using Infonet.CStoreCommander.Entities;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface IPayAtPumpManager
    {
        /// <summary>
        /// Method to set language
        /// </summary>
        /// <param name="payPump">Pay at pump</param>
        /// <param name="language">Language</param>
        void SetLanguage(ref PayAtPump payPump, string language);

        /// <summary>
        /// Method to add a line
        /// </summary>
        /// <param name="payPump">Pay at pump</param>
        /// <param name="user">User</param>
        /// <param name="oLine">Sale line</param>
        /// <param name="adjust">Adjust or not</param>
        /// <param name="tableAdjust">Table adjust or not</param>
        /// <param name="forReprint">For reprint or not</param>
        /// <returns>True or false</returns>
        bool Add_a_Line(ref PayAtPump payPump, User user, Sale_Line oLine, 
            bool adjust = false, bool tableAdjust = true, bool forReprint = false);
    }
}
