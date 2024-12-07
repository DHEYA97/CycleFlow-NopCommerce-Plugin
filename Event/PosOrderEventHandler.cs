using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Controllers;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Services.Events;
namespace Nop.Plugin.Misc.CycleFlow.Event
{
    public class PosOrderEventHandler : IConsumer<PosOrderEvent>
    {
        public async Task HandleEventAsync(PosOrderEvent eventMessage)
        {
            if (eventMessage?.PosOrder != null)
            {
                PosOrder posOrder = eventMessage.PosOrder;
                var cycleFlowSettingService = EngineContext.Current.Resolve<CycleFlowSettingService>();
                var orderStateOrderMappingRepo = EngineContext.Current.Resolve<IRepository<OrderStateOrderMapping>>();

                var (sequenceHtml, status) = await cycleFlowSettingService.CheckOrderStatusSequence(posOrder.PosUserId);
                
                OrderStateOrderMapping orderStateOrderMapping = new OrderStateOrderMapping
                {
                    OrderId = posOrder.Id,
                    NopStoreId  = posOrder.NopStoreId,
                    PosUserId = posOrder.PosUserId,
                    CustomerId
                };
            }
        }
    }
}
