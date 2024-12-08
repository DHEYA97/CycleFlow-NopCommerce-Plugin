using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public class OrderStateOrderImageMappingService : IOrderStateOrderImageMappingService
    {
        #region Fields
        private readonly IRepository<OrderStateOrderImageMapping> _orderStateOrderImageMapping;
        private readonly IPictureService _pictureService;
        
        #endregion

        #region Ctor
        public OrderStateOrderImageMappingService(
            IRepository<OrderStateOrderImageMapping> orderStateOrderImageMapping,
            IPictureService pictureService)
        {
            _orderStateOrderImageMapping = orderStateOrderImageMapping;
            _pictureService = pictureService;
        }
        #endregion
        #region Method
        public async Task<string?> GetPictureUrlByImageTypeIdAsync(int imgTypeId,int posUserId,int orderId,int orderStatusId)
        {
            var pictureId = await _orderStateOrderImageMapping.Table.Where(x=>x.PosUserId == posUserId && x.OrderId == orderId && x.OrderStatusId == orderStatusId && x.ImageTypeId == imgTypeId).Select(x=>x.PictureId).FirstOrDefaultAsync();
            var pictureUrl = await _pictureService.GetPictureUrlAsync(pictureId);
            return pictureUrl;
        }
        #endregion
    }
}
