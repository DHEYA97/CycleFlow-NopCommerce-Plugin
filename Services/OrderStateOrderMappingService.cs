using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
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
    public class OrderStateOrderMappingService : IOrderStateOrderMappingService
    {
        #region Fields
        private readonly IRepository<OrderStateOrderImageMapping> _orderStateOrderImageMapping;
        private readonly IRepository<OrderStateOrderMapping> _orderStateOrderMapping;
        private readonly IPictureService _pictureService;
        private readonly IPosOrderService _posOrderService;
        
        #endregion

        #region Ctor
        public OrderStateOrderMappingService(
            IRepository<OrderStateOrderImageMapping> orderStateOrderImageMapping,
            IRepository<OrderStateOrderMapping> orderStateOrderMapping,
            IPictureService pictureService,
            IPosOrderService posOrderService)
        {
            _orderStateOrderImageMapping = orderStateOrderImageMapping;
            _orderStateOrderMapping = orderStateOrderMapping;
            _pictureService = pictureService;
            _posOrderService = posOrderService;
        }
        #endregion
        #region Method
        public async Task<string?> GetPictureUrlByImageTypeIdAsync(int imgTypeId,int posUserId,int orderId,int orderStatusId)
        {
            var pictureId = await _orderStateOrderImageMapping.Table.Where(x=>x.PosUserId == posUserId && x.OrderId == orderId && x.OrderStatusId == orderStatusId && x.ImageTypeId == imgTypeId).Select(x=>x.PictureId).FirstOrDefaultAsync();
            var pictureUrl = await _pictureService.GetPictureUrlAsync(pictureId);
            return pictureUrl;
        }
        public async Task<OrderStateOrderMapping> GetOrderStateOrderMappingByIdAsync(int id)
        {
            return await _orderStateOrderMapping.GetByIdAsync(id);
        }

        public async Task GetAllDeportationModelById(int posOrderId)
        {
            var order = _posOrderService.GetPosOrderByIdAsync(posOrderId);
            if (order == null)
            {
                throw new ArgumentNullException(nameof(CycleFlowSettingModel), "PosOrderId not Found");
            }
        }
        #endregion
    }
}
