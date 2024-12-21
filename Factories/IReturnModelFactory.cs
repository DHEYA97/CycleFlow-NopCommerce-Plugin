using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using Nop.Plugin.Misc.CycleFlow.Models.Return;


namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public interface IReturnModelFactory
    {
        Task<ReturnSearchModel> PrepareReturnSearchModelAsync(ReturnSearchModel searchModel);
        Task<ReturnListModel> PrepareReturnModelListModelAsync(ReturnSearchModel searchModel);
        Task<ReturnModel> PrepareReturnModelAsync(ReturnModel model, FilterReturnModel FilterReturnModel, bool excludeProperties = false);
    }
}
