using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
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
        private readonly IRepository<OrderStatusImageTypeMapping> _orderStatusImageTypeMapping;
        private readonly IPictureService _pictureService;
        private readonly IPosOrderService _posOrderService;
        private readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IImageTypeService _imageTypeService;

        #endregion

        #region Ctor
        public OrderStateOrderMappingService(
            IRepository<OrderStateOrderImageMapping> orderStateOrderImageMapping,
            IRepository<OrderStateOrderMapping> orderStateOrderMapping,
            IRepository<OrderStatusImageTypeMapping> orderStatusImageTypeMapping,
            IPictureService pictureService,
            IPosOrderService posOrderService,
            ICycleFlowSettingService cycleFlowSettingService,
            IOrderStatusService orderStatusService,
            IImageTypeService imageTypeService)
        {
            _orderStateOrderImageMapping = orderStateOrderImageMapping;
            _orderStateOrderMapping = orderStateOrderMapping;
            _orderStatusImageTypeMapping = orderStatusImageTypeMapping;
            _pictureService = pictureService;
            _posOrderService = posOrderService;
            _cycleFlowSettingService = cycleFlowSettingService;
            _orderStatusService = orderStatusService;
            _imageTypeService = imageTypeService;
        }
        #endregion
        #region Method
        public async Task<string?> GetPictureUrlByImageTypeIdAsync(int imgTypeId,int posUserId,int orderId,int orderStatusId)
        {
            var pictureId = await _orderStateOrderImageMapping.Table.Where(x=>x.PosUserId == posUserId && x.OrderId == orderId && x.OrderStatusId == orderStatusId && x.ImageTypeId == imgTypeId).Select(x=>x.PictureId).FirstOrDefaultAsync();
            var pictureUrl = await _pictureService.GetPictureUrlAsync(pictureId);
            return pictureUrl;
        }
        public async Task<List<OrderStatusImageTypeMapping>> GetImageTypeIdsByOrderStatusIdAsync(int posUserId, int orderStatusId)
        {
            return await _orderStatusImageTypeMapping.Table.Where(x=>x.PosUserId == posUserId && x.OrderStatusId == orderStatusId).ToListAsync();
        }
        public async Task<OrderStateOrderMapping> GetOrderStateOrderMappingByIdAsync(int id)
        {
            return await _orderStateOrderMapping.GetByIdAsync(id);
        }
        public async Task<List<OrderStateOrderMapping>> GeAllOrderStateOrderMappingByOrderIdAsync(int orderId)
        {
            return await _orderStateOrderMapping.Table.Where(x => x.OrderId == orderId).OrderBy(x=>x.InsertionDate).ToListAsync();
        }
        public async Task<List<AllDeportationModel>> GetAllDeportationModelByIdAsync(int posOrderId)
        {
            var order = _posOrderService.GetPosOrderByIdAsync(posOrderId);
            if (order == null)
            {
                throw new ArgumentNullException(nameof(CycleFlowSettingModel), $"PosOrderId  {posOrderId} not Found");
            }
            var orderMapping = await GeAllOrderStateOrderMappingByOrderIdAsync(posOrderId);
            if (orderMapping == null)
            {
                throw new ArgumentNullException(nameof(CycleFlowSettingModel), $"Thar is No Deportation to order : {posOrderId}");
            }
            List<AllDeportationModel> AllDeportationModelList = new List<AllDeportationModel>();
            foreach (var item in orderMapping)
            {
                var imgTypes = await GetImageTypeIdsByOrderStatusIdAsync(item.PosUserId, item.OrderStatusId);
                var imgTypeList = new List<(string, string)?>();
                foreach (var img in imgTypes)
                {
                    imgTypeList.Add(
                        new( await _imageTypeService.GetImageTypeNameAsync(img.ImageTypeId), await GetPictureUrlByImageTypeIdAsync(img.ImageTypeId,img.PosUserId,posOrderId,img.OrderStatusId)??string.Empty)
                        );
                }
                AllDeportationModelList.Add(
                    new AllDeportationModel
                    {
                        DeportationDate = item.InsertionDate,
                        StatusName = await _orderStatusService.GetOrderStatusNameAsync(item.OrderStatusId),
                        NextStatusName = await _orderStatusService.GetOrderStatusNameAsync((await _cycleFlowSettingService.GetNextStepByFirstStepAsync(item.OrderStatusId, item.PosUserId))),
                        ImageType = imgTypeList
                    });
            }
            return AllDeportationModelList;
        }
        #endregion
    }
}
