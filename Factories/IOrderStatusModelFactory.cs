using Nop.Plugin.Misc.CycleFlow.Models.OrderStatus;
using Nop.Plugin.Misc.CycleFlow.Domain;

namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public interface IOrderStatusModelFactory
    {
        Task<OrderStatusSearchModel> PrepareOrderStatusSearchModelAsync(OrderStatusSearchModel searchModel);

        Task<OrderStatusListModel> PrepareOrderStatusListModelAsync(OrderStatusSearchModel searchModel);

        Task<OrderStatusModel> PrepareOrderStatusModelAsync(OrderStatusModel model, OrderStatus orderStatus, bool excludeProperties = false);
    }
}
