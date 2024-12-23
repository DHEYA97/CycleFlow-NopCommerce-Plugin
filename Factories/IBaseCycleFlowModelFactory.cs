using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public interface IBaseCycleFlowModelFactory
    {
        Task PreparePosUsersListAsync(IList<SelectListItem> items);
        Task PreparePosStoresAsync(IList<SelectListItem> items);
        Task PrepareCustomerListAsync(IList<SelectListItem> items);
    }
}
