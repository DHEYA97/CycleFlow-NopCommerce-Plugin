using Nop.Plugin.Misc.CycleFlow.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public interface IOrderStateOrderMappingService
    {
        Task<string?> GetPictureUrlByImageTypeIdAsync(int imgTypeId, int posUserId, int orderId, int orderStatusId);
        Task<OrderStateOrderMapping> GetOrderStateOrderMappingByIdAsync(int id);
    }
}
