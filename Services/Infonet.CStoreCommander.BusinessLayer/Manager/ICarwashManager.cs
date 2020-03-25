using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public interface ICarwashManager
    {
        bool GetCarwashCode();

        bool ValidateCarwash(string carwashCode);

        void RefundCarwash();

        bool GetCarwashServerStatus();
        
    }
}
