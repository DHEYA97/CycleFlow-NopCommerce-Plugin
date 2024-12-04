using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Misc.CycleFlow.Constant;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Plugin.Misc.SmsAuthentication.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public class CycleFlowSettingModelFactory : ICycleFlowSettingModelFactory
    {
        #region Felid
        private readonly ILocalizationService _localizationService;
        private readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IShippingService _shippingService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IPosStoreService _posStoreService;
        private readonly IPosUserService _posUserService;
        private readonly ICustomerService _customerService;
        private readonly IImageTypeService _imageTypeService;
        protected readonly ISmsTemplateService _smsTemplateService;
        #endregion
        #region Ctor
        public CycleFlowSettingModelFactory(ILocalizationService localizationService,
            ICycleFlowSettingService cycleFlowSettingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IShippingService shippingService,
            IOrderStatusService orderStatusService,
            IBaseAdminModelFactory baseAdminModelFactory,
            IPosStoreService posStoreService,
            IPosUserService posUserService,
            ICustomerService customerService,
            IImageTypeService imageTypeService,
            ISmsTemplateService smsTemplateService
            )
        {
            _localizationService = localizationService;
            _cycleFlowSettingService = cycleFlowSettingService;
            _storeContext = storeContext;
            _storeService = storeService;
            _shippingService = shippingService;
            _orderStatusService = orderStatusService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _posStoreService = posStoreService;
            _posUserService = posUserService;
            _customerService = customerService;
            _imageTypeService = imageTypeService;
            _smsTemplateService = smsTemplateService;
        }
        #endregion
        #region Methods
        public async Task<CycleFlowSettingSearchModel> PrepareCycleFlowSettingSearchModelAsync(CycleFlowSettingSearchModel searchModel)
        {
            searchModel ??= new CycleFlowSettingSearchModel();

            await PreparePosUsersListAsync(searchModel.AvailablePosUsers);
            await PreparePosStoresAsync(searchModel.AvailableStores);

            searchModel.SearchStoreIds
                = searchModel.SearchPosUsersIds
                = new List<int>() { 0 };
            searchModel.SetGridPageSize();

            return searchModel;
        }
        public async Task<CycleFlowSettingListModel> PrepareCycleFlowSettingListModelAsync(CycleFlowSettingSearchModel searchModel)
        {
            var cycleFlowSetting = await _cycleFlowSettingService.SearchCycleFlowSettingAsync(
                orderStatusName: searchModel.SearchName,
                posUserIds:searchModel.SearchPosUsersIds,
                storeIds: searchModel.SearchStoreIds
            );

            return await new CycleFlowSettingListModel().PrepareToGridAsync(searchModel, cycleFlowSetting, () =>
            {
                return cycleFlowSetting.SelectAwait(async cycleFlowSetting =>
                {
                    return await PrepareCycleFlowSettingModelAsync(null, cycleFlowSetting, true);
                });
            });
        }
        public async Task<CycleFlowSettingModel> PrepareCycleFlowSettingModelAsync(CycleFlowSettingModel model, OrderStatusSorting orderStatusSorting, bool excludeProperties = false,int currentId = 0)
        {
            if (orderStatusSorting != null)
            {
                model ??= orderStatusSorting.ToModel<CycleFlowSettingModel>();
                
                var store = await _storeService.GetStoreByIdAsync(orderStatusSorting.NopStoreId);
                var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(orderStatusSorting.OrderStatusId);
                var nextOrderStatus = await _orderStatusService.GetOrderStatusByIdAsync(orderStatusSorting.NextStep);
                var posUser = await _posUserService.GetPosUserByIdAsync(orderStatusSorting.PosUserId);
                var customer = await _cycleFlowSettingService.GetCustomerAsync(orderStatusSorting.PosUserId, orderStatusSorting.OrderStatusId);
                
                model.StoreName = store?.Name ?? string.Empty;
                model.CurrentOrderStatusName = orderStatus?.Name ?? string.Empty;
                model.NextOrderStatusName = nextOrderStatus?.Name ?? string.Empty;
                model.PosUserName = await _customerService.GetCustomerFullNameAsync(await _posUserService.GetUserByIdAsync(posUser.Id)) ?? string.Empty;
                model.CustomerName = await _customerService.GetCustomerFullNameAsync(customer) ?? string.Empty;
                
                model.CustomerId = customer.Id;
                model.IsFirstStep = orderStatusSorting.IsFirstStep;
                model.IsLastStep = orderStatusSorting.IsLastStep;
            }

            if (!excludeProperties)
            {
                
                await PreparePosStoresAsync(model.AvailableStores);
                await PrepareCustomerListAsync(model.AvailableCustomers);
                await PreparePosUsersListAsync(model.AvailablePosUsers);
                await PrepareImageTypeListAsync(model.AvailableImageTypes);
                await PrepareCurrentSelectedImageTypeAsync(model.SelectedImageTypeIds,model.PosUserId,model.CurrentOrderStatusId);
                await PrepareSmsTemplatesAsync(model.AvailableClientSmsTemplates);
                await PrepareSmsTemplatesAsync(model.AvailableUserSmsTemplates);
                model.EnableIsFirstStep = await _cycleFlowSettingService.EnableIsFirstStepAsync(model.PosUserId,currentId);
                model.EnableIsLastStep = await _cycleFlowSettingService.EnableIsLastStepAsync(model.PosUserId, currentId);
            }
            return model;
        }


        #endregion

        #region Utilite
        protected async Task PreparePosStoresAsync(IList<SelectListItem> items)
        {
            var availableStores = await (await _posStoreService.GetAllPosStoresAsync()).Where(ps => ps.StoreType != StoreTypes.Online).ToListAsync();
            foreach (var store in availableStores)
            {
                var nopStore = await _storeService.GetStoreByIdAsync(store.NopStoreId);
                items.Add(new SelectListItem { Value = store.Id.ToString(), Text = nopStore.Name});
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
        protected async Task PrepareCurrentSelectedImageTypeAsync(IList<int> items,int posUserId,int orderStatusId)
        {
            var currentImageType = await _cycleFlowSettingService.GetAllOrderCurrentSelectedImageTypeAsync(posUserId,orderStatusId);
            foreach (var imgId in currentImageType)
            {
                items.Add(imgId);
            }
        }
        protected async Task PreparePosUsersListAsync(IList<SelectListItem> items)
        {
            
            var availablePosUsers = await (await _posUserService.GetPosUserListAsync()).Where(ps => ps.Active).ToListAsync();
            foreach (var user in availablePosUsers)
            {
                items.Add(new SelectListItem { Value = _posUserService.GetPosUserByNopCustomerIdAsync(user.Id).Result.Id.ToString(), Text = _customerService.GetCustomerFullNameAsync(user).Result.ToString() });
            }
            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        }
        //public async Task PrepareCurrentOrderStatusListAsync(IList<SelectListItem> items,int currentId = 0)
        //{
        //    IList<(string Id, string Name)> availableOrderStatus = await _cycleFlowSettingService.GetAllOrderStatusAsync(true,currentId);
        //    foreach (var order in availableOrderStatus)
        //    {
        //        items.Add(new SelectListItem { Value = order.Id.ToString(), Text = order.Name.ToString() });
        //    }
        //    items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        //}
        //public async Task PrepareNextOrderStatusListAsync(IList<SelectListItem> items, int currentId = 0)
        //{
        //    IList<(string Id, string Name)> availableOrderStatus = await _cycleFlowSettingService.GetNextOrderStatusAsync(true, currentId);
        //    var next = await _cycleFlowSettingService.GetNextStepByFirstStep(currentId);

        //    foreach (var order in availableOrderStatus)
        //    {
        //       if(int.Parse(order.Id) == next)
        //        items.Add(new SelectListItem { Value = order.Id.ToString(), Text = order.Name.ToString(), Selected = true});
        //       else
        //        items.Add(new SelectListItem { Value = order.Id.ToString(), Text = order.Name.ToString() });
        //    }
        //    items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        //}
        protected async Task PrepareImageTypeListAsync(IList<SelectListItem> items)
        {
            var availableimageType = await (await _imageTypeService.GetAllImageTypesAsync()).Where(os => !os.Deleted).ToListAsync();
            foreach (var imageType in availableimageType)
            {
                items.Add(new SelectListItem { Value = imageType.Id.ToString(), Text = imageType.Name.ToString() });
            }
            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        }
        protected async Task PrepareSmsTemplatesAsync(IList<SelectListItem> items)
        {
            var availableSmsTemplatesType = await (await _smsTemplateService.GetAllSmsTemplatesAsync(0)).Where(os => os.Active).ToListAsync();
            foreach (var smsTemplate in availableSmsTemplatesType)
            {
                items.Add(new SelectListItem { Value = smsTemplate.Id.ToString(), Text = smsTemplate.Name.ToString() });
            }
            items.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Plugin.Misc.CycleFlow.Common.Select"), Value = "0" });
        }

        #endregion
    }
}