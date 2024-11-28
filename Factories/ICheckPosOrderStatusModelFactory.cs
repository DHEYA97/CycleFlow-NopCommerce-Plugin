using Nop.Plugin.Misc.CycleFlow.Models.CheckPosOrderStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public interface ICheckPosOrderStatusModelFactory
    {
        Task<CheckPosOrderStatusSearchModel> PrepareCheckPosOrderStatusSearchModelAsync(CheckPosOrderStatusSearchModel searchModel);
        Task<CheckPosOrderStatusListModel> PrepareCheckPosOrderStatusListModelAsync(CheckPosOrderStatusSearchModel searchModel);
    }
}
