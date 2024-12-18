using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
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
using static Nop.Plugin.Misc.CycleFlow.Models.Deportation.DeportationModel;

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
        public DeportationModelFactory(ILocalizationService localizationService,
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
        public Task<DeportationSearchModel> PrepareDeportationSearchModelAsync(DeportationSearchModel searchModel, bool justShowByCustomer = false, bool justLastStepOrder = false)
       {
            if (searchModel == null)
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
        public async Task<DeportationModel> PrepareDeportationModelAsync(DeportationModel model, OrderStateOrderMapping orderStateOrderMapping, bool showAllInfo , bool excludeProperties = false, int currentId = 0)
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
                    var imageTypes = await _OrderStatusImageTypeMapping.Table.Where(x => x.PosUserId == orderStateOrderMapping.PosUserId && x.OrderStatusId == orderStateOrderMapping.OrderStatusId).ToListAsync();
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
                    if(imageTypes != null && imageTypes.Count() > 0)
                    {
                        model.ImageType = new List<ImageTypeModel>();
                        foreach (var imageType in imageTypes)
                        {
                            model.ImageType!.Add(
                                new ImageTypeModel
                                    {
                                        ImageTypeId = imageType.ImageTypeId,
                                        ImageTypeName = await _imageTypeService.GetImageTypeNameAsync(imageType.ImageTypeId),
                                        ImageTypeUrl = await _orderStateOrderMappingService.GetPictureUrlByImageTypeIdAsync(imageType.ImageTypeId, orderStateOrderMapping.PosUserId, orderStateOrderMapping.OrderId, orderStateOrderMapping.OrderStatusId)
                                    }
                                );
                        }
                    }
                    model.AllDeportation = await _orderStateOrderMappingService.GetAllDeportationModelByIdAsync(order.Id);
                }
            }
            return model;
        }

        #endregion
    }
}
