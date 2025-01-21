using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.CycleFlow.Models.OrderStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public interface ICycleFlowSettingModelFactory
    {
        Task<CycleFlowSettingSearchModel> PrepareCycleFlowSettingSearchModelAsync(CycleFlowSettingSearchModel searchModel);
        Task<CycleFlowSettingListModel> PrepareCycleFlowSettingListModelAsync(CycleFlowSettingSearchModel searchModel);
        Task<CycleFlowSettingModel> PrepareCycleFlowSettingModelAsync(CycleFlowSettingModel model, OrderStatusSorting orderStatusSorting, bool excludeProperties = false, int currentId = 0);

    }
}
