using Infonet.CStoreCommander.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.ADOData
{
    public interface IKickBackService
    {
        void Set_Customer_KickBack_Data(string InputValue,bool boolIsPhoneNumber, string PointCardNumber,
            ref Sale sale, bool isSwipeMethod = false);
        DataTable GaskingKickback(Sale sale);

        void InsertKickbackQueue(string source);

        DataTable GetKickbackQueue();

       void  DeleteKickbackQueue();


    }
    
}
