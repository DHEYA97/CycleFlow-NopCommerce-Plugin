using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public interface IOrderStateOrderImageMappingService
    {
        Task<string?> GetPictureUrlByImageTypeIdAsync(int imgTypeId, int posUserId, int orderId, int orderStatusId);
    }
}
