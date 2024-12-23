using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Constant;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using Nop.Plugin.Misc.CycleFlow.Models.Return;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Plugin.Misc.SmsAuthentication.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using static Nop.Plugin.Misc.CycleFlow.Models.Return.ReturnModel;

namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public class ReturnModelFactory : IReturnModelFactory
    {
        #region Felid
        private readonly ILocalizationService _localizationService;
        private readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly IDeportationService _deportationService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IShippingService _shippingService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IPosUserService _posUserService;
        private readonly ICustomerService _customerService;
        private readonly IImageTypeService _imageTypeService;
        protected readonly IOrderService _orderService;
        private readonly IRepository<OrderStatusSorting> _orderStatusSortingTypeRepository;
        private readonly IRepository<OrderStatusImageTypeMapping> _OrderStatusImageTypeMapping;
        private readonly IPictureService _pictureService;
        private readonly IOrderStateOrderMappingService _orderStateOrderMappingService;
        
        #endregion
        #region Ctor
        public ReturnModelFactory(ILocalizationService localizationService,
            ICycleFlowSettingService cycleFlowSettingService,
            IDeportationService deportationService,
            IStoreContext storeContext,
            IStoreService storeService,
            IShippingService shippingService,
            IOrderStatusService orderStatusService,
            IBaseAdminModelFactory baseAdminModelFactory,
            IPosUserService posUserService,
            ICustomerService customerService,
            IImageTypeService imageTypeService,
            IOrderService orderService,
            IRepository<OrderStatusSorting> orderStatusSortingTypeRepository,
            IPictureService pictureService,
            IRepository<OrderStatusImageTypeMapping> OrderStatusImageTypeMapping,
            IOrderStateOrderMappingService orderStateOrderMappingService
            )
        {
            _localizationService = localizationService;
            _cycleFlowSettingService = cycleFlowSettingService;
            _deportationService = deportationService;
            _storeContext = storeContext;
            _storeService = storeService;
            _shippingService = shippingService;
            _orderStatusService = orderStatusService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _posUserService = posUserService;
            _customerService = customerService;
            _imageTypeService = imageTypeService;
            _orderService = orderService;
            _orderStatusSortingTypeRepository = orderStatusSortingTypeRepository;
            _pictureService = pictureService;
            _OrderStatusImageTypeMapping = OrderStatusImageTypeMapping;
            _orderStateOrderMappingService = orderStateOrderMappingService;
        }
        #endregion
        #region Methods
        public async Task<ReturnSearchModel> PrepareReturnSearchModelAsync(ReturnSearchModel searchModel)
        {
            searchModel ??= new ReturnSearchModel();

            await PrepareCustomerListAsync(searchModel.AvailableCustomers);
            await PrepareYearListAsync(searchModel.AvailableYears);
            await PrepareMonthListAsync(searchModel.AvailableMonths);
            
            searchModel.SearchCustomerIds
                = searchModel.SearchYearIds
                = searchModel.SearchMonthIds
                = new List<int>() { 0 };
            searchModel.SetGridPageSize();

            return searchModel;
        }
        public async Task<ReturnListModel> PrepareReturnModelListModelAsync(ReturnSearchModel searchModel)
        {
            var deportation = await _deportationService.SearchReturnAsync(
                customerIds:searchModel.SearchCustomerIds,
                years:searchModel.SearchYearIds,
                months:searchModel.SearchMonthIds
            );

            return await new ReturnListModel().PrepareToGridAsync(searchModel, deportation, () =>
            {
                return deportation.SelectAwait(async FilterReturn =>
                {
                    return await PrepareReturnModelAsync(null, FilterReturn,true);
                });
            });
        }
        public async Task<ReturnModel> PrepareReturnModelAsync(ReturnModel model, FilterReturnModel filterReturnModel, bool excludeProperties = false)
        {
            if (filterReturnModel != null)
            {
                model ??= filterReturnModel.ToModel<ReturnModel>();
                var customer = await _customerService.GetCustomerByIdAsync(filterReturnModel.CustomerId);

                model.CustomerName = await _customerService.GetCustomerFullNameAsync(customer) ?? string.Empty;

                if (!excludeProperties)
                {
                    var AllReturnList = new List<AllReturnModel>();
                    if (filterReturnModel?.OrderDetail != null && filterReturnModel?.OrderDetail.Count > 0)
                    {
                        foreach(var orderMapp in filterReturnModel.OrderDetail)
                        {
                            var OrderStateOrderMapping = await _orderStateOrderMappingService.GetOrderStateOrderMappingByIdAsync(orderMapp.OrderStateOrderMappingId);
                            var posUser = await _posUserService.GetPosUserByIdAsync(OrderStateOrderMapping.PosUserId);
                            var store = await _storeService.GetStoreByIdAsync(OrderStateOrderMapping.NopStoreId);
                            var ImgTypeIds = await _orderStateOrderMappingService.GetImageTypeIdsByOrderStatusIdAsync(OrderStateOrderMapping.PosUserId, OrderStateOrderMapping.OrderStatusId);
                            var CustomerReturnFromName = await _cycleFlowSettingService.GetCustomerByOrderStatusIdAsync(OrderStateOrderMapping.PosUserId, OrderStateOrderMapping.ReturnOrderStatusId ?? 0);
                            var ImageTypeList = new List<(string, string)?>();
                            if (ImgTypeIds != null && ImgTypeIds?.Count > 0)
                            {
                                foreach (var imgType in ImgTypeIds)
                                {
                                    ImageTypeList.Add(
                                        (
                                            await _imageTypeService.GetImageTypeNameAsync(imgType.ImageTypeId),
                                            await _orderStateOrderMappingService.GetPictureUrlByImageTypeIdAsync(imgType.ImageTypeId, OrderStateOrderMapping.PosUserId, OrderStateOrderMapping.OrderId, OrderStateOrderMapping.OrderStatusId, OrderStateOrderMapping.Id)
                                        )
                                     );
                                }
                            }
                            AllReturnList.Add(
                                new AllReturnModel
                                {
                                    CustomerReturnName = model.CustomerName,
                                    Note = OrderStateOrderMapping.Note??string.Empty,
                                    OrderId = OrderStateOrderMapping.OrderId,
                                    OrderStateOrderMappingId = OrderStateOrderMapping.Id,
                                    PosUserName = _customerService.GetCustomerFullNameAsync(_posUserService.GetUserByIdAsync(posUser.Id).Result).Result ?? string.Empty,
                                    PosStoreName = store?.Name ?? string.Empty,
                                    ReturnDate = OrderStateOrderMapping.InsertionDate!.Value,
                                    ReturnStatusName = _orderStatusService.GetOrderStatusNameAsync(OrderStateOrderMapping.OrderStatusId).Result,
                                    ReturnFromStatusName = _orderStatusService.GetOrderStatusNameAsync(OrderStateOrderMapping.ReturnOrderStatusId??0).Result,
                                    CustomerReturnFromName = _customerService.GetCustomerFullNameAsync(CustomerReturnFromName).Result ?? string.Empty,
                                    ImageType = ImageTypeList
                                }
                            );
                        }
                        model.AllReturn = AllReturnList;
                    }
                }
            }
            return model;
        }
        #endregion
        #region Utilite
        protected async Task PreparePosUsersListAsync(IList<SelectListItem> items)
        {

            var availablePosUsers = await (await _posUserService.GetPosUserListAsync()).Where(ps => ps.Active).ToListAsync();
            foreach (var user in availablePosUsers)
            {
                items.Add(new SelectListItem { Value = _posUserService.GetPosUserByNopCustomerIdAsync(user.Id).Result.Id.ToString(), Text = _customerService.GetCustomerFullNameAsync(user).Result.ToString() });
            }
            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        }
        protected async Task PrepareCustomerListAsync(IList<SelectListItem> items)
        {
            var cycleFowRole = await _customerService.GetCustomerRoleBySystemNameAsync(SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            var availableCustomer = await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] { cycleFowRole.Id }).Result.ToListAsync();
            foreach (var customer in availableCustomer)
            {
                items.Add(new SelectListItem { Value = customer.Id.ToString(), Text = await _customerService.GetCustomerFullNameAsync(customer) });
            }
            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        }
        protected async Task PrepareYearListAsync(IList<SelectListItem> items)
        {
            var orderStateOrderMappingList = await _orderStateOrderMappingService.GeAllOrderStateOrderMappingAsync();
            var yearList =  orderStateOrderMappingList.GroupBy(x => x.InsertionDate.Value.Year);
            foreach (var year in yearList)
            {
                items.Add(new SelectListItem { Value = year.Key.ToString(), Text = year.Key.ToString() });
            }
            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        }
        protected async Task PrepareMonthListAsync(IList<SelectListItem> items)
        {
            var orderStateOrderMappingList = await _orderStateOrderMappingService.GeAllOrderStateOrderMappingAsync();
            var monthList = orderStateOrderMappingList.GroupBy(x => x.InsertionDate.Value.Month);
            foreach (var month in monthList)
            {
                items.Add(new SelectListItem { Value = month.Key.ToString(), Text = month.Key.ToString() });
            }
            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        }
        #endregion
    }
}
