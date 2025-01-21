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
    public interface IOrderStateOrderMappingService
    {
        Task<OrderStateOrderImageMapping> GetOrderStateOrderImageMappingByIdAsync(int orderStateOrderImageMappingId);
        Task InsertOrderStateOrderImageMappingAsync(OrderStateOrderImageMapping orderStateOrderImageMapping);
        Task UpdateOrderStateOrderImageMappingAsync(OrderStateOrderImageMapping orderStateOrderImageMapping);
        Task DeleteOrderStateOrderImageMappingAsync(OrderStateOrderImageMapping orderStateOrderImageMapping);
        Task<OrderStateOrderMapping> GetOrderStateOrderMappingByIdAsync(int id);
        Task<IList<AllDeportationModel>> GetAllDeportationModelByIdAsync(int posOrderId, bool skipLast = true);
        Task<IList<OrderStateOrderMapping>> GeAllOrderStateOrderMappingAsync();
        Task InsertStepAsync(DeportationModel model, Deportation deportationType);
        Task<OrderStateOrderMapping> GeOrderStateOrderMappingAsync(OrderStatusPictureSearchModel searchModel);
        Task<IList<OrderStateOrderImageMapping>> GetAllOrderStateOrderImageMappingPictureOrderStatusIdAsync(int posUserId, int orderId, int orderStatusId);
    }
}
