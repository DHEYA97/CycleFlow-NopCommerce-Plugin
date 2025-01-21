using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;


namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public interface IDeportationModelFactory
    {
        Task<DeportationSearchModel> PrepareDeportationSearchModelAsync(DeportationSearchModel searchModel, bool justShowByCustomer = false, bool justLastStepOrder = false);
        Task<DeportationListModel> PrepareDeportationModelListModelAsync(DeportationSearchModel searchModel);
        Task<DeportationModel> PrepareDeportationModelAsync(DeportationModel model, OrderStateOrderMapping orderStateOrderMapping, bool showAllInfo, bool excludeProperties = false, int currentId = 0, bool skipLast = true);
        Task<OrderStatusPictureListModel> PrepareOrderStatusPictureListModelAsync(OrderStatusPictureSearchModel searchModel, OrderStateOrderMapping orderStateOrderMapping);
    }
}
