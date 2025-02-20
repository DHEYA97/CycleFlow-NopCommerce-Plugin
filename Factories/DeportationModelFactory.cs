using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models.Extensions;
using static Nop.Plugin.Misc.CycleFlow.Models.Deportation.DeportationModel;

namespace Nop.Plugin.Misc.CycleFlow.Factories
{
    public class DeportationModelFactory : IDeportationModelFactory
    {
        #region Felid
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICustomerService _customerService;
        private readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly IDeportationService _deportationService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderStatusService _orderStatusService;
        protected readonly IOrderService _orderService;
        private readonly IRepository<OrderStatusSorting> _orderStatusSortingTypeRepository;
        private readonly IOrderStateOrderMappingService _orderStateOrderMappingService;
        private readonly IPosUserService _posUserService;
        private readonly IPictureService _pictureService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IShippingService _shippingService;
        #endregion
        #region Ctor
        public DeportationModelFactory(
            IBaseAdminModelFactory baseAdminModelFactory,
            ICustomerService customerService,
            ICycleFlowSettingService cycleFlowSettingService,
            IDeportationService deportationService,
            ILocalizationService localizationService,
            IOrderStatusService orderStatusService,
            IStoreContext storeContext,
            IStoreService storeService,
            IShippingService shippingService,
            IOrderService orderService,
            IRepository<OrderStatusSorting> orderStatusSortingTypeRepository,
            IOrderStateOrderMappingService orderStateOrderMappingService,
            IPosUserService posUserService,
            IPictureService pictureService
            )
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _customerService = customerService;
            _cycleFlowSettingService = cycleFlowSettingService;
            _deportationService = deportationService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _storeService = storeService;
            _shippingService = shippingService;
            _orderStatusService = orderStatusService;
            _orderService = orderService;
            _orderStatusSortingTypeRepository = orderStatusSortingTypeRepository;
            _orderStateOrderMappingService = orderStateOrderMappingService;
            _posUserService = posUserService;
            _pictureService = pictureService;
        }
        #endregion
        #region Methods
        public Task<DeportationSearchModel> PrepareDeportationSearchModelAsync(DeportationSearchModel searchModel, bool justShowByCustomer = false, bool justLastStepOrder = false)
        {
            if(searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            // Prepare parameters
            searchModel.SetGridPageSize();
            searchModel.JustShowByCustomer = justShowByCustomer;
            searchModel.JustLastStepOrder = justLastStepOrder;
            return Task.FromResult(searchModel);
        }
       public async Task<DeportationListModel> PrepareDeportationModelListModelAsync(DeportationSearchModel searchModel)
       {
            var deportation = await _deportationService.SearchOrderStateOrderMappingAsync(
                orderNumber: searchModel.OrderNumber,
                justShowByCustomer: searchModel.JustShowByCustomer,
                justLastStepOrder: searchModel.JustLastStepOrder
            );

            return await new DeportationListModel().PrepareToGridAsync(searchModel, deportation, () =>
            {
                return deportation.SelectAwait(async deportation =>
                {
                    return await PrepareDeportationModelAsync(null, deportation,searchModel.JustShowByCustomer, true);
                });
            });
        }
        public async Task<DeportationModel> PrepareDeportationModelAsync(DeportationModel model, OrderStateOrderMapping orderStateOrderMapping, bool showAllInfo , bool excludeProperties = false, int currentId = 0 , bool skipLast = true)
        {
            if (orderStateOrderMapping != null)
            {
                model ??= orderStateOrderMapping.ToModel<DeportationModel>();

                var order = await _orderService.GetOrderByIdAsync(orderStateOrderMapping.OrderId);
                var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
                var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(orderStateOrderMapping.OrderStatusId);
                var nextOrderStatus = await _orderStatusService.GetOrderStatusByIdAsync(await _cycleFlowSettingService.GetNextStepByFirstStepAsync(orderStateOrderMapping.OrderStatusId, orderStateOrderMapping.PosUserId)??0);
                
                model.CurrentOrderStatusName = orderStatus?.Name ?? string.Empty;
                model.NextOrderStatusName = nextOrderStatus?.Name ?? string.Empty;
                if (!excludeProperties)
                {
                    var store = await _storeService.GetStoreByIdAsync(orderStateOrderMapping.NopStoreId);
                    var customer = await _customerService.GetCustomerByIdAsync(orderStateOrderMapping.CustomerId);
                    var orderSorting = await _orderStatusSortingTypeRepository.Table.FirstOrDefaultAsync(x => x.PosUserId == orderStateOrderMapping.PosUserId && x.OrderStatusId == orderStateOrderMapping.OrderStatusId);
                    var returnStatus = await _orderStatusService.GetOrderStatusByIdAsync(await _cycleFlowSettingService.GetReturnStepByCurentStepAsync(orderStateOrderMapping.OrderStatusId, orderStateOrderMapping.PosUserId) ?? 0);
                    var posUser = await _posUserService.GetPosUserByIdAsync(orderStateOrderMapping.PosUserId);

                    model.StoreName = store?.Name ?? string.Empty;
                    model.PosUserName = await _customerService.GetCustomerFullNameAsync(await _posUserService.GetUserByIdAsync(posUser.Id)) ?? string.Empty;
                    model.OrderDate = order.CreatedOnUtc;
                    model.CustomerName = await _customerService.GetCustomerFullNameAsync(customer);
                    model.IsEnableSendToClient = orderSorting.IsEnableSendToClient;
                    model.ClientSmsTemplateId = orderSorting.ClientSmsTemplateId;
                    model.IsEnableSendToUser = orderSorting.IsEnableSendToUser;
                    model.UserSmsTemplateId = orderSorting.UserSmsTemplateId;
                    model.IsEnableReturn = orderSorting.IsEnableReturn;
                    model.ReturnStepId = orderSorting.ReturnStepId;
                    model.ReturnStepName = returnStatus?.Name ?? string.Empty;
                    model.NextOrderStatusId = nextOrderStatus?.Id??0;
                    model.IsAddPictureRequired = orderSorting.IsAddPictureRequired;
                    model.ShowAllInfo = showAllInfo;
                    model.OrderItemCount = orderItems.Sum(x=>x.Quantity);
                    model.ProductOrderItem = new List<ProductOrderItemModel>();
                    foreach (var orderItem in orderItems)
                    {
                        
                        var product = await _orderService.GetProductByOrderItemIdAsync(orderItem.Id);
                        var orderItemAttributesXml = (await _orderService.GetOrderItemByIdAsync(orderItem.Id)).AttributesXml;
                        var picture = await _pictureService.GetProductPictureAsync(product, orderItemAttributesXml);
                        var pictureUrl = await _pictureService.GetPictureUrlAsync(picture.Id);
                        model.ProductOrderItem.Add(
                                new ProductOrderItemModel
                                {
                                    ProductId = product.Id,
                                    Sku = product.Sku,
                                    ProductName = product.Name,
                                    PictureThumbnailUrl = pictureUrl,
                                    Quantity = orderItem.Quantity,
                                }
                            );
                    }
                    model.AllDeportation = await _orderStateOrderMappingService.GetAllDeportationModelByIdAsync(order.Id,skipLast);
                }
            }
            return model;
        }
        public virtual async Task<OrderStatusPictureListModel> PrepareOrderStatusPictureListModelAsync(OrderStatusPictureSearchModel searchModel, OrderStateOrderMapping orderStateOrderMapping)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (orderStateOrderMapping == null)
                throw new ArgumentNullException(nameof(orderStateOrderMapping));
        
            var orderStateOrderMappingPictures = (await _orderStateOrderMappingService.GetAllOrderStateOrderImageMappingPictureOrderStatusIdAsync(searchModel.PosUserId, searchModel.OrderId, searchModel.OrderStatusId)).ToPagedList(searchModel);

            var model = await new OrderStatusPictureListModel().PrepareToGridAsync(searchModel, orderStateOrderMappingPictures, () =>
            {
                return orderStateOrderMappingPictures.SelectAwait(async orderStatePicture =>
                {
                    var OrderStatusPictureModel = new OrderStatusPictureModel();
                    
                    var picture = (await _pictureService.GetPictureByIdAsync(orderStatePicture.PictureId))
                        ?? throw new Exception("Picture cannot be loaded");

                    OrderStatusPictureModel.Id = orderStatePicture.Id;
                    OrderStatusPictureModel.PictureId = orderStatePicture.PictureId;
                    OrderStatusPictureModel.PictureUrl = (await _pictureService.GetPictureUrlAsync(picture)).Url;

                    return OrderStatusPictureModel;
                });
            });

            return model;
        }
        #endregion
    }
}