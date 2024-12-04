using Nop.Plugin.Misc.CycleFlow.Models.Deportation;


namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public interface IDeportationModelFactory
    {
        Task<DeportationSearchModel> PrepareDeportationSearchModelAsync(DeportationSearchModel searchModel);
        Task<DeportationListModel> PrepareDeportationModelListModelAsync(DeportationSearchModel searchModel);
    }
}
