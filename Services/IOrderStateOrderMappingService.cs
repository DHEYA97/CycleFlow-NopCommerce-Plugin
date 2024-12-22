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
        Task<string?> GetPictureUrlByImageTypeIdAsync(int imgTypeId, int posUserId, int orderId, int orderStatusId, int orderStateOrderMappingId);
        Task<OrderStateOrderMapping> GetOrderStateOrderMappingByIdAsync(int id);
        Task<List<AllDeportationModel>> GetAllDeportationModelByIdAsync(int posOrderId);
        Task<List<OrderStateOrderMapping>> GeAllOrderStateOrderMappingAsync();
        Task InsertStepAsync(DeportationModel model, Deportation deportationType);
        Task<List<OrderStatusImageTypeMapping>> GetImageTypeIdsByOrderStatusIdAsync(int posUserId, int orderStatusId);
    }
}
