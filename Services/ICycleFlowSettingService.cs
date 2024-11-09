using Nop.Core;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public interface ICycleFlowSettingService
    {
        Task<IPagedList<OrderStatusSorting>> SearchCycleFlowSettingAsync(
            string orderStatusName = null,
            int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
        Task InsertCycleFlowSettingAsync(CycleFlowSettingModel model);
        Task<OrderStatusSorting> GetOrderStatusSortingByIdAsync(int orderStatusSortingId);
        Task UpdateCycleFlowSettingAsync(CycleFlowSettingModel model);
        Task DeleteCycleFlowSettingAsync(OrderStatusSorting model);
    }
}
