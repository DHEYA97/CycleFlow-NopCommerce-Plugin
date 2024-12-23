using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Constant.Enum;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using Nop.Plugin.Misc.CycleFlow.Models.Return;
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
using static Nop.Plugin.Misc.CycleFlow.Models.Return.FilterReturnModel;

namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public class DeportationService : IDeportationService
    {
        #region Fields
        protected readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly ICustomerService _customerService;
        private readonly IImageTypeService _imageTypeService;
        private readonly INotificationService _notificationService;
        private readonly IRepository<Domain.OrderStatus> _orderStatusRepository;
        private readonly IRepository<OrderStatusSorting> _orderStatusSortingTypeRepository;
        private readonly IRepository<OrderStatusPermissionMapping> _orderStatusPermissionMappingRepository;
        private readonly IRepository<OrderStatusImageTypeMapping> _orderStatusImageTypeMappingRepository;
        private readonly IRepository<OrderStateOrderMapping> _orderStateOrderMappingRepository;
        private readonly IOrderStatusService _orderStatusService;
        protected readonly IOrderStateOrderMappingService _orderStateOrderMappingService;
        protected readonly IOrderService _orderService;
        private readonly IPosUserService _posUserService;
        private readonly IStoreService _storeService;
        private readonly IShippingService _shippingService;
        protected readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public DeportationService(
            ICustomerService customerService,
            ICycleFlowSettingService cycleFlowSettingService,
            IImageTypeService imageTypeService,
            INotificationService notificationService,
            IRepository<Domain.OrderStatus> orderStatusRepository,
            IRepository<OrderStatusSorting> orderStatusSortingTypeRepository,
            IRepository<OrderStatusPermissionMapping> orderStatusPermissionMappingRepository,
            IRepository<OrderStateOrderMapping> orderStateOrderMappingRepository,
            IRepository<OrderStatusImageTypeMapping> orderStatusImageTypeMappingRepository,
            IOrderStateOrderMappingService orderStateOrderMappingService,
            IOrderService orderService,
            IOrderStatusService orderStatusService,
            IPosUserService posUserService,
            IStoreService storeService,
            IShippingService shippingService,
            IWorkContext workContext
            )
        {
            _customerService = customerService;
            _cycleFlowSettingService = cycleFlowSettingService;
            _imageTypeService = imageTypeService;
            _notificationService = notificationService;
            _orderStatusRepository = orderStatusRepository;
            _orderStatusSortingTypeRepository = orderStatusSortingTypeRepository;
            _orderStatusPermissionMappingRepository = orderStatusPermissionMappingRepository;
            _orderStateOrderMappingRepository = orderStateOrderMappingRepository;
            _orderStatusImageTypeMappingRepository = orderStatusImageTypeMappingRepository;
            _orderStatusService = orderStatusService;
            _orderService = orderService;
            _orderStateOrderMappingService = orderStateOrderMappingService;
            _posUserService = posUserService;
            _storeService = storeService;
            _shippingService = shippingService;
            _workContext = workContext;
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
                    var IsLastStep = await _cycleFlowSettingService.IsLastStepInSortingByStatusIdAsync(record!.OrderStatusId, record.PosUserId);
                    if (justShowByCustomer)
                    {
                        var customerSetting = await _cycleFlowSettingService.GetCustomerByOrderStatusIdAsync(record!.PosUserId, record.OrderStatusId);
                        if (customerSetting.Id == customer.Id)
                        {
                            if (IsLastStep)
                                continue;
                            filteredRecords.Add(record);
                        }
                        continue;
                    }
                    if (justLastStepOrder)
                    {
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
        public virtual async Task<IPagedList<FilterReturnModel>> SearchReturnAsync(
               IList<int> posUserIds = null,
               IList<int> customerIds = null,
               IList<int> years = null,
               IList<int> months = null,
               int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {

            var filteredRecords = new List<FilterReturnModel>();
            var allRecords = await _orderStateOrderMappingRepository.Table.Where(x=>x.IsReturn.HasValue && x.IsReturn == true).ToListAsync();

            var groupedRecords = allRecords
                .GroupBy(o => new
                {
                    o.CustomerId,
                    Year = o.InsertionDate.HasValue ? o.InsertionDate.Value.Year : 0,
                    Month = o.InsertionDate.HasValue ? o.InsertionDate.Value.Month : 0
                                })
                    .Select(g => new
                    {
                        CustomerId = g.Key.CustomerId,
                        CustomerName = string.Empty,
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        ReturnCount = g.Count(),
                        Orders = g.Select(o => new
                        {
                            o.Id
                        }).ToList()
                    }).ToList();

            if (groupedRecords.Any())
            {
                foreach (var record in groupedRecords)
                {
                    filteredRecords.Add(
                    new FilterReturnModel
                    {
                        CustomerId = record.CustomerId,
                        CustomerName = record.CustomerName,
                        Year = record.Year,
                        Month = record.Month,
                        ReturnCount = record.ReturnCount,
                        OrderDetail = record.Orders.Select(x=>new OrderDetailModel
                        {
                            OrderStateOrderMappingId = x.Id,
                        }).ToList()
                    });
                }
            }

            if (customerIds?.Any(x => x != 0) ?? false)
                filteredRecords = filteredRecords.Where(p => customerIds.Contains(p.CustomerId)).ToList();
            
            if (years?.Any(x => x != 0) ?? false)
                filteredRecords = filteredRecords.Where(p => years.Contains(p.Year)).ToList();
            
            if (months?.Any(x => x != 0) ?? false)
                filteredRecords = filteredRecords.Where(p => months.Contains(p.Month)).ToList();

            var pagedList = new PagedList<FilterReturnModel>(filteredRecords, pageIndex, pageSize);
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
