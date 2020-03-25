using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infonet.CStoreCommander.BusinessLayer.Manager;

namespace Infonet.CStoreCommander.WebApi.Tests.Managers
{
    public class CarwashStub : CarwashManager
    {
        public CarwashStub(IPolicyManager policyManager) : base(policyManager)
        {
            
        }
       
        protected override bool SendCarwashRequest(string request)
        {
            if (!string.IsNullOrEmpty(request))
            {
                return true;
            }
            return false;
        }
    }
}
