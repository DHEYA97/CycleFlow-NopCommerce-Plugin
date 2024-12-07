using Nop.Core;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Plugin.Misc.SmsAuthentication.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public class DeportationModelFactory : IDeportationModelFactory
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
        private readonly IPosStoreService _posStoreService;
        private readonly IPosUserService _posUserService;
        private readonly ICustomerService _customerService;
        private readonly IImageTypeService _imageTypeService;
        protected readonly ISmsTemplateService _smsTemplateService;
        protected readonly IOrderService _orderService;
        #endregion
        #region Ctor
        public DeportationModelFactory(ILocalizationService localizationService,
            ICycleFlowSettingService cycleFlowSettingService,
            IDeportationService deportationService,
            IStoreContext storeContext,
            IStoreService storeService,
            IShippingService shippingService,
            IOrderStatusService orderStatusService,
            IBaseAdminModelFactory baseAdminModelFactory,
            IPosStoreService posStoreService,
            IPosUserService posUserService,
            ICustomerService customerService,
            IImageTypeService imageTypeService,
            ISmsTemplateService smsTemplateService,
            IOrderService orderService
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
            _posStoreService = posStoreService;
            _posUserService = posUserService;
            _customerService = customerService;
            _imageTypeService = imageTypeService;
            _smsTemplateService = smsTemplateService;
            _orderService = orderService;
        }
        #endregion
        #region Methods
        public Task<DeportationSearchModel> PrepareDeportationSearchModelAsync(DeportationSearchModel searchModel)
       {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            // Prepare parameters
            searchModel.SetGridPageSize();
            return Task.FromResult(searchModel);
       }
       public async Task<DeportationListModel> PrepareDeportationModelListModelAsync(DeportationSearchModel searchModel)
       {
            var deportation = await _deportationService.SearchOrderStateOrderMappingAsync(
                orderNumber: searchModel.OrderNumber
            );

            return await new DeportationListModel().PrepareToGridAsync(searchModel, deportation, () =>
            {
                return deportation.SelectAwait(async deportation =>
                {
                    return await PrepareDeportationModelAsync(null, deportation, true);
                });
            });
        }
        public async Task<DeportationModel> PrepareDeportationModelAsync(DeportationModel model, OrderStateOrderMapping orderStateOrderMapping, bool excludeProperties = false, int currentId = 0)
        {
            if (orderStateOrderMapping != null)
            {
                model ??= orderStateOrderMapping.ToModel<DeportationModel>();

                var order = await _orderService.GetOrderByIdAsync(orderStateOrderMapping.OrderId);
                var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(orderStateOrderMapping.OrderStatusId);
                var nextOrderStatus = await _orderStatusService.GetOrderStatusByIdAsync(await _cycleFlowSettingService.GetNextStepByFirstStep(orderStateOrderMapping.OrderStatusId, orderStateOrderMapping.PosUserId));
                
                model.CurrentOrderStatusName = orderStatus?.Name ?? string.Empty;
                model.NextOrderStatusName = nextOrderStatus?.Name ?? string.Empty;
                

            }

            if (!excludeProperties)
            {

                //await PreparePosStoresAsync(model.AvailableStores);
                //await PrepareCustomerListAsync(model.AvailableCustomers);
                //await PreparePosUsersListAsync(model.AvailablePosUsers);
                //await PrepareImageTypeListAsync(model.AvailableImageTypes);
                //await PrepareCurrentSelectedImageTypeAsync(model.SelectedImageTypeIds, model.PosUserId, model.CurrentOrderStatusId);
                //await PrepareSmsTemplatesAsync(model.AvailableClientSmsTemplates);
                //await PrepareSmsTemplatesAsync(model.AvailableUserSmsTemplates);
                //model.EnableIsFirstStep = await _cycleFlowSettingService.EnableIsFirstStepAsync(model.PosUserId, currentId);
                //model.EnableIsLastStep = await _cycleFlowSettingService.EnableIsLastStepAsync(model.PosUserId, currentId);
            }
            return model;
        }

        #endregion
    }
}
