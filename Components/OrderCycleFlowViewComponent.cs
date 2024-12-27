using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.CycleFlow.Constant;
using Nop.Plugin.Misc.CycleFlow.Constant.Enum;
using Nop.Plugin.Misc.CycleFlow.Factories;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Components;


namespace Nop.Plugin.Misc.CycleFlow.Components
{
    [ViewComponent(Name = SystemDefaults.ORDER_CYCLE_FLOW)]
    public class OrderCycleFlowViewComponent : NopViewComponent
    {

        #region Fields
        protected readonly IDeportationModelFactory _deportationModelFactory;

        #endregion

        #region Ctor
        public OrderCycleFlowViewComponent(IDeportationModelFactory deportationModelFactory)
        {
            _deportationModelFactory = deportationModelFactory;
        }
        #endregion

        #region Method

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
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
