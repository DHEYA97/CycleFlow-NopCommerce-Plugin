using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Customers;
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
            ICycleFlowSettingService cycleFlowSettingService
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

        }
        #endregion

        #region Methods
        public virtual async Task<IPagedList<OrderStateOrderMapping>> SearchOrderStateOrderMappingAsync(
            int orderNumber = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            var query = await _orderStateOrderMappingRepository.GetAllPagedAsync(query =>
            {
                query = query.Where(x => _cycleFlowSettingService.GetCustomerAsync(x.OrderStatusId, x.PosUserId).Result.Id == customer.Id)
                             .GroupBy(x => x.OrderId)
                             .Select(g => g.OrderByDescending(x => x.InsertionDate).First());

                if (orderNumber > 0)
                {
                    query = query.Where(x => x.OrderId == orderNumber);
                }
                return query;

            }, pageIndex, pageSize, getOnlyTotalCount);
            return query;
        }
        #endregion
    }
}
