using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
   public interface IKickBackManager
    {
        double VerifyKickBack(string pointCardNumber, string phoneNumber, int tillNumber, 
            int saleNumber, byte registerNumber, string userCode, out ErrorMessage errorMessage,
            out Sale sale, ref bool useCardNumber);
        double CheckKickbackResponse(bool response, int tillNumber, int saleNumber, string userCode, out ErrorMessage errorMessage,ref Sale sale);
        double CheckBalance(string pointCardNum, int saleNumber, int tillNumber, string userCode, out ErrorMessage errorMessage);
        bool ProcessKickBack(short command_Renamed, string userCode, ref Sale sale, out ErrorMessage errorMessage);

        GaskingKickback ValidateGasKing(int tillNumber, int saleNumber, byte registerNumber, string userCode, out ErrorMessage errorMessage, bool isCardSwipedInTenderScreen = false);

        void InsertTo_KickBackQueue(string SourceString);
    }
}
