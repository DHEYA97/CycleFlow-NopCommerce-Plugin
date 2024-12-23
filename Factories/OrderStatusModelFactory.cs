using Nop.Plugin.Misc.CycleFlow.Factories;
using Nop.Plugin.Misc.CycleFlow.Models.OrderStatus;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.CycleFlow.Services;
namespace Nop.Plugin.Misc.POSSystem.Areas.Admin.Factories
{
    public class OrderStatusModelFactory : IOrderStatusModelFactory
    {
        #region Fields
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly IOrderStatusService _orderStatusService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        #endregion
        #region Ctor
        public OrderStatusModelFactory(
            IHtmlFormatter htmlFormatter,
            IOrderStatusService orderStatusService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory
            )
        {
            _htmlFormatter = htmlFormatter;
            _orderStatusService = orderStatusService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
        }
        #endregion
        #region Methods
        public Task<OrderStatusSearchModel> PrepareOrderStatusSearchModelAsync(OrderStatusSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            //prepare parameters
            searchModel.SetGridPageSize();
            return Task.FromResult(searchModel);
        }
        public async Task<OrderStatusListModel> PrepareOrderStatusListModelAsync(OrderStatusSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var orderStatusInfoes = await _orderStatusService.GetAllOrderStatusAsync(showHidden: true, name: searchModel.SearchName, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
            var model = await new OrderStatusListModel().PrepareToGridAsync(searchModel, orderStatusInfoes, () =>
            {
                return orderStatusInfoes.SelectAwait(async orderStatusInfo =>
                {
                    var orderStatusModel = orderStatusInfo.ToModel<OrderStatusModel>();
                    if (!string.IsNullOrWhiteSpace(orderStatusInfo.Description))
                    {
                        orderStatusModel.Description=_htmlFormatter.StripTags(orderStatusInfo.Description);
                    }
                    return orderStatusModel;
                });
            });
            return model;


        }
        public async Task<OrderStatusModel> PrepareOrderStatusModelAsync(OrderStatusModel model, OrderStatus orderStatus, bool excludeProperties = false)
        {

            Func<OrderStatusLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (orderStatus != null)
            {
                if (model == null)
                {
                    model = orderStatus.ToModel<OrderStatusModel>();
                }

                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(orderStatus, entity => entity.Name, languageId, false, false);
                    locale.Description = await _localizationService.GetLocalizedAsync(orderStatus, entity => entity.Description, languageId, false, false);
                };
            }

            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;

        }
        #endregion
    }
}
