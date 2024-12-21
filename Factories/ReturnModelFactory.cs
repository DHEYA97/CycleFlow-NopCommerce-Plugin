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

            await PreparePosUsersListAsync(searchModel.AvailablePosUsers);
            await PrepareCustomerListAsync(searchModel.AvailableCustomers);
            await PrepareYearListAsync(searchModel.AvailableYears);
            await PrepareMonthListAsync(searchModel.AvailableMonths);
            
            searchModel.SearchCustomerIds
                = searchModel.SearchPosUsersIds
                = searchModel.SearchYearIds
                = searchModel.SearchMonthIds
                = new List<int>() { 0 };
            searchModel.SetGridPageSize();

            return searchModel;
        }
        public async Task<ReturnListModel> PrepareReturnModelListModelAsync(ReturnSearchModel searchModel)
        {
            var deportation = await _deportationService.SearchReturnAsync(
                posUserIds:searchModel.SearchPosUsersIds,
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

                var posUser = await _posUserService.GetPosUserByIdAsync(filterReturnModel.PosUserId);
                var customer = await _customerService.GetCustomerByIdAsync(filterReturnModel.CustomerId);

                model.PosUserName = await _customerService.GetCustomerFullNameAsync(await _posUserService.GetUserByIdAsync(posUser.Id)) ?? string.Empty;
                model.CustomerName = await _customerService.GetCustomerFullNameAsync(customer) ?? string.Empty;

                //if (!excludeProperties)
                //{
                //    var store = await _storeService.GetStoreByIdAsync(orderStateOrderMapping.NopStoreId);
                //    var customer = await _customerService.GetCustomerByIdAsync(orderStateOrderMapping.CustomerId);
                //    var orderSorting = await _orderStatusSortingTypeRepository.Table.FirstOrDefaultAsync(x => x.PosUserId == orderStateOrderMapping.PosUserId && x.OrderStatusId == orderStateOrderMapping.OrderStatusId);
                //    var imageTypes = await _OrderStatusImageTypeMapping.Table.Where(x => x.PosUserId == orderStateOrderMapping.PosUserId && x.OrderStatusId == orderStateOrderMapping.OrderStatusId).ToListAsync();
                //    var returnStatus = await _orderStatusService.GetOrderStatusByIdAsync(await _cycleFlowSettingService.GetReturnStepByCurentStepAsync(orderStateOrderMapping.OrderStatusId, orderStateOrderMapping.PosUserId) ?? 0);
                //    var posUser = await _posUserService.GetPosUserByIdAsync(orderStateOrderMapping.PosUserId);

                //    model.StoreName = store?.Name ?? string.Empty;
                //    model.PosUserName = await _customerService.GetCustomerFullNameAsync(await _posUserService.GetUserByIdAsync(posUser.Id)) ?? string.Empty;
                //    model.OrderDate = order.CreatedOnUtc;
                //    model.CustomerName = await _customerService.GetCustomerFullNameAsync(customer);
                //    model.IsEnableSendToClient = orderSorting.IsEnableSendToClient;
                //    model.ClientSmsTemplateId = orderSorting.ClientSmsTemplateId;
                //    model.IsEnableSendToUser = orderSorting.IsEnableSendToUser;
                //    model.UserSmsTemplateId = orderSorting.UserSmsTemplateId;
                //    model.IsEnableReturn = orderSorting.IsEnableReturn;
                //    model.ReturnStepId = orderSorting.ReturnStepId;
                //    model.ReturnStepName = returnStatus?.Name ?? string.Empty;
                //    model.NextOrderStatusId = nextOrderStatus?.Id ?? 0;
                //    model.ShowAllInfo = showAllInfo;
                //    model.OrderItemCount = orderItems.Sum(x => x.Quantity);
                //    model.ProductOrderItem = new List<ProductOrderItemModel>();
                //    foreach (var orderItem in orderItems)
                //    {

                //        var product = await _orderService.GetProductByOrderItemIdAsync(orderItem.Id);
                //        var orderItemAttributesXml = (await _orderService.GetOrderItemByIdAsync(orderItem.Id)).AttributesXml;
                //        var picture = await _pictureService.GetProductPictureAsync(product, orderItemAttributesXml);
                //        var pictureUrl = await _pictureService.GetPictureUrlAsync(picture.Id);
                //        model.ProductOrderItem.Add(
                //                new ProductOrderItemModel
                //                {
                //                    ProductId = product.Id,
                //                    Sku = product.Sku,
                //                    ProductName = product.Name,
                //                    PictureThumbnailUrl = pictureUrl,
                //                    Quantity = orderItem.Quantity,
                //                }
                //            );
                //    }
                //    if (imageTypes != null && imageTypes.Count() > 0)
                //    {
                //        model.ImageType = new List<ImageTypeModel>();
                //        foreach (var imageType in imageTypes)
                //        {
                //            model.ImageType!.Add(
                //                new ImageTypeModel
                //                {
                //                    ImageTypeId = imageType.ImageTypeId,
                //                    ImageTypeName = await _imageTypeService.GetImageTypeNameAsync(imageType.ImageTypeId),
                //                    ImageTypeUrl = await _orderStateOrderMappingService.GetPictureUrlByImageTypeIdAsync(imageType.ImageTypeId, orderStateOrderMapping.PosUserId, orderStateOrderMapping.OrderId, orderStateOrderMapping.OrderStatusId, orderStateOrderMapping.Id)
                //                }
                //                );
                //        }
                //    }
                //    model.AllDeportation = await _orderStateOrderMappingService.GetAllDeportationModelByIdAsync(order.Id);
                //}
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
