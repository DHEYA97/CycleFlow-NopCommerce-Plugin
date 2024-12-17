using Nop.Core;
using Nop.Plugin.Misc.CycleFlow.Constant.Enum;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public interface IDeportationService
    {
        Task<IPagedList<OrderStateOrderMapping>> SearchOrderStateOrderMappingAsync(
            int orderNumber = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
        Task DeportationAsync(DeportationModel model, Deportation deportationType);
    }
}
