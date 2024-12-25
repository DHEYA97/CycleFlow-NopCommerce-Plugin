using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Misc.CycleFlow.Permission;
using Nop.Plugin.Misc.CycleFlow.Factories;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;
using Nop.Plugin.Misc.CycleFlow.Models.Return;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Plugin.Misc.CycleFlow.Constant;
namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    public class ReturnController : BaseCycleFlowController
    {
        #region Fields
        private readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly IDeportationService _deportationService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IOrderStateOrderMappingService _orderStateOrderMappingService;
        private readonly IPermissionService _permissionService;
        private readonly IPosUserService _posUserService;
        private readonly IReturnModelFactory _returnModelFactory;
        #endregion
        #region Ctor
        public ReturnController(
            ICycleFlowSettingService cycleFlowSettingService,
            IDeportationService deportationService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IOrderStateOrderMappingService orderStateOrderMappingService,
            IPermissionService permissionService,
            IPosUserService posUserService,
            IReturnModelFactory returnModelFactory
            )
        {
            _cycleFlowSettingService = cycleFlowSettingService;
            _deportationService = deportationService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _posUserService = posUserService;
            _orderStateOrderMappingService = orderStateOrderMappingService;
            _returnModelFactory = returnModelFactory;
        }
        #endregion
        #region Methods
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual async Task<IActionResult> List()
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ShowAllOrderCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;
            
            await _cycleFlowSettingService.NotificationPosUserAsync();
            var model = await _returnModelFactory.PrepareReturnSearchModelAsync(new ReturnSearchModel());
            return View(model);
        }
        [HttpPost]
        public virtual async Task<IActionResult> List(ReturnSearchModel searchModel)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ShowAllOrderCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME,true);
            if (result != null)
                return result;

            await _cycleFlowSettingService.NotificationPosUserAsync();
            var model = await _returnModelFactory.PrepareReturnModelListModelAsync(searchModel);
            return Json(model);
        }
        public virtual async Task<IActionResult> View(int customerId,int year,int month)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ShowAllOrderCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;

            var filterReturnModel = (await _deportationService.SearchReturnAsync(
                                    customerIds: new List<int>(customerId),
                                    years: new List<int>(year),
                                    months: new List<int>(month)
                                    )).FirstOrDefault();
            
            if (filterReturnModel == null)
            {
                _notificationService.ErrorNotification(_localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.filterReturnModel.NotFound").Result);
                return RedirectToAction(nameof(List));
            }
            var model = await _returnModelFactory.PrepareReturnModelAsync(null, filterReturnModel);
            return View(model);
        }
        #endregion
    }
}
