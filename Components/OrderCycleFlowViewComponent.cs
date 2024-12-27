using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.CycleFlow.Constant;
using Nop.Plugin.Misc.CycleFlow.Constant.Enum;
using Nop.Plugin.Misc.CycleFlow.Factories;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using Nop.Plugin.Misc.CycleFlow.Permission;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Components;


namespace Nop.Plugin.Misc.CycleFlow.Components
{
    [ViewComponent(Name = SystemDefaults.ORDER_CYCLE_FLOW)]
    public class OrderCycleFlowViewComponent : NopViewComponent
    {

        #region Fields
        protected readonly IDeportationModelFactory _deportationModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public OrderCycleFlowViewComponent(IDeportationModelFactory deportationModelFactory,
         IPermissionService permissionService,
         IWorkContext workContext)
        {
            _deportationModelFactory = deportationModelFactory;
            _permissionService = permissionService;
            _workContext = workContext;
        }
        #endregion

        #region Method

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return Content("");

            var isHavePermission = await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.DeportationCycleFlowPlugin) || await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ShowAllOrderCycleFlowPlugin);

            if (!isHavePermission)
                return Content("");

            if (additionalData is OrderModel orderModel)
            {
                var deportationSearch = new DeportationSearchModel();
                deportationSearch.OrderNumber = orderModel.Id;
                var deportationListModel = await _deportationModelFactory.PrepareDeportationModelListModelAsync(deportationSearch);
                var model = deportationListModel.Data.FirstOrDefault().Id;
                return View($"~/Plugins/{SystemDefaults.PluginOutputDir}/Views/Sheared/Components/{SystemDefaults.ORDER_CYCLE_FLOW}/Default.cshtml",model);
            }

            return Content("");
        }
        #endregion
    }
}
