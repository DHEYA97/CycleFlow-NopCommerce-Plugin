using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Constant.Enum;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Customers;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public class DeportationService : IDeportationService
    {
        #region Fields
        private readonly IRepository<Domain.OrderStatus> _orderStatusRepository;
        private readonly IRepository<OrderStatusSorting> _orderStatusSortingTypeRepository;
        private readonly IRepository<OrderStatusPermissionMapping> _orderStatusPermissionMappingRepository;
        private readonly IRepository<OrderStatusImageTypeMapping> _orderStatusImageTypeMappingRepository;
        private readonly IRepository<OrderStateOrderMapping> _orderStateOrderMappingRepository;
        private readonly INotificationService _notificationService;
        private readonly IImageTypeService _imageTypeService;
        private readonly IStoreService _storeService;
        private readonly IShippingService _shippingService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IPosUserService _posUserService;
        private readonly ICustomerService _customerService;
        protected readonly IWorkContext _workContext;
        protected readonly ICycleFlowSettingService _cycleFlowSettingService;
        protected readonly IOrderService _orderService;
        protected readonly IOrderStateOrderMappingService _orderStateOrderMappingService;
        #endregion

        #region Ctor
        public DeportationService(
            IRepository<Domain.OrderStatus> orderStatusRepository,
            IRepository<OrderStatusSorting> orderStatusSortingTypeRepository,
            IRepository<OrderStatusPermissionMapping> orderStatusPermissionMappingRepository,
            IRepository<OrderStateOrderMapping> orderStateOrderMappingRepository,
            IRepository<OrderStatusImageTypeMapping> orderStatusImageTypeMappingRepository,
            INotificationService notificationService,
            IImageTypeService imageTypeService,
            IStoreService storeService,
            IShippingService shippingService,
            IOrderStatusService orderStatusService,
            IPosUserService posUserService,
            ICustomerService customerService,
            IWorkContext workContext,
            ICycleFlowSettingService cycleFlowSettingService,
            IOrderService orderService,
            IOrderStateOrderMappingService orderStateOrderMappingService
            )
        {
            _orderStatusRepository = orderStatusRepository;
            _orderStatusSortingTypeRepository = orderStatusSortingTypeRepository;
            _orderStatusPermissionMappingRepository = orderStatusPermissionMappingRepository;
            _orderStateOrderMappingRepository = orderStateOrderMappingRepository;
            _orderStatusImageTypeMappingRepository = orderStatusImageTypeMappingRepository;
            _notificationService = notificationService;
            _imageTypeService = imageTypeService;
            _storeService = storeService;
            _shippingService = shippingService;
            _orderStatusService = orderStatusService;
            _posUserService = posUserService;
            _customerService = customerService;
            _workContext = workContext;
            _cycleFlowSettingService = cycleFlowSettingService;
            _orderService = orderService;
            _orderStateOrderMappingService = orderStateOrderMappingService;
        }
        #endregion

        #region Methods
        public virtual async Task<IPagedList<OrderStateOrderMapping>> SearchOrderStateOrderMappingAsync(
                int orderNumber = 0,bool justShowByCustomer = false ,bool justLastStepOrder = false,
                int pageIndex = 0,int pageSize = int.MaxValue,bool getOnlyTotalCount = false)
            {
            
            var filteredRecords = new List<OrderStateOrderMapping>();
            var allRecords = await _orderStateOrderMappingRepository.Table.ToListAsync();
            var groupedRecords = allRecords
                .GroupBy(x => x.OrderId)
                .Select(g => g.OrderByDescending(x => x.InsertionDate).FirstOrDefault());
            
            
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (groupedRecords.Any())
            {
                foreach (var record in groupedRecords)
                {
                    if (justShowByCustomer)
                    {
                        var customerSetting = await _cycleFlowSettingService.GetCustomerByOrderStatusIdAsync(record!.PosUserId, record.OrderStatusId);
                        if (customerSetting.Id == customer.Id)
                        {
                            filteredRecords.Add(record);
                        }
                        continue;
                    }
                    if (justLastStepOrder)
                    {
                        var IsLastStep = await _cycleFlowSettingService.IsLastStepInSortingByStatusIdAsync(record!.OrderStatusId, record.PosUserId);
                        if (IsLastStep)
                        {
                            filteredRecords.Add(record);
                        }
                        continue;
                    }
                    filteredRecords.Add(record);

                }
            }
            if (orderNumber > 0)
            {
                filteredRecords = filteredRecords.Where(x => x.OrderId == orderNumber).ToList();
            }
            var pagedList = new PagedList<OrderStateOrderMapping>(filteredRecords, pageIndex, pageSize);
            return pagedList;
        }
        public async Task DeportationAsync(DeportationModel model, Deportation deportationType)
        {
            var order = await _orderService.GetOrderByIdAsync(model.OrderId);
            if (order == null)
                throw new ArgumentNullException(nameof(CycleFlowSettingModel), $"PosOrderId  {model.OrderId} not Found");
            var currentOrderStatusId = await _orderStatusService.GetOrderStatusByIdAsync(model.OrderStatusId);
            if(currentOrderStatusId == null)
                throw new Exception("OrderStatusId Not Found");
            var isIsLastStep = await _cycleFlowSettingService.IsLastStepInSortingByStatusIdAsync(model.OrderStatusId, model.PosUserId);
            if(isIsLastStep)
                throw new Exception("OrderStatusId LastStep in Sorting");
            var nextOrderStatusId = await _orderStatusService.GetOrderStatusByIdAsync(model.NextOrderStatusId);
            if (nextOrderStatusId == null)
                throw new Exception("NextOrderStatusId Not Found");

            if (deportationType == Deportation.Return)
            {
                if (!model.IsEnableReturn)
                    throw new Exception("Return Not Allowed");
                if (!model.ReturnStepId.HasValue || model.ReturnStepId <= 0)
                    throw new Exception("Return Step not Found");
                var returnOrderStatusId = await _orderStatusService.GetOrderStatusByIdAsync((int)model.ReturnStepId!);
                if (returnOrderStatusId == null)
                    throw new Exception("Return Step not Found");
            }
            await _orderStateOrderMappingService.InsertStepAsync(model, deportationType);
        }
        #endregion
    }
}
