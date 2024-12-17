using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Controllers;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
namespace Nop.Plugin.Misc.CycleFlow.Event
{
    public class PosOrderEventHandler : IConsumer<EntityInsertedEvent<PosOrder>>
    {
        public async Task HandleEventAsync(EntityInsertedEvent<PosOrder> eventMessage)
        {
            var posOrder = eventMessage.Entity;
            if (posOrder != null)
            {
                var cycleFlowSettingService = EngineContext.Current.Resolve<ICycleFlowSettingService>();
                var orderStateOrderMappingRepo = EngineContext.Current.Resolve<IRepository<OrderStateOrderMapping>>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                var notificationService = EngineContext.Current.Resolve<INotificationService>();
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

                var firstSortingStep = await cycleFlowSettingService.GetFirstStepInPosUserAsync(posOrder.PosUserId);
               
                if (firstSortingStep == null)
                {
                    notificationService.ErrorNotification(localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.First.NotFound").Result, false);
                    return;
                }
                var customer = await cycleFlowSettingService.GetCustomerByOrderStatusIdAsync(firstSortingStep.PosUserId,firstSortingStep.OrderStatusId);

                OrderStateOrderMapping orderStateOrderMapping = new OrderStateOrderMapping
                {
                    OrderId = posOrder.NopOrderId,
                    NopStoreId  = posOrder.NopStoreId,
                    PosUserId = posOrder.PosUserId,
                    OrderStatusId = firstSortingStep.OrderStatusId,
                    CustomerId = customer.Id,
                };
                await orderStateOrderMapping.SetBaseInfoAsync<OrderStateOrderImageMapping>(workContext);
                await orderStateOrderMappingRepo.InsertAsync(orderStateOrderMapping);
            }
        }
    }
}
