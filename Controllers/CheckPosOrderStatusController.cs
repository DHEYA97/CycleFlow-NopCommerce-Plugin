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
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Plugin.Misc.CycleFlow.Models.CheckPosOrderStatus;
using Nop.Core;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Plugin.Misc.CycleFlow.Constant;

namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    public class CheckPosOrderStatusController : BaseCycleFlowController
    {
        #region Fields
        private readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly ICheckPosOrderStatusModelFactory _checkPosOrderStatusModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPosUserService _posUserService;
        private readonly IWorkContext _workContext;
        #endregion
        #region Ctor
        public CheckPosOrderStatusController(
            ICheckPosOrderStatusModelFactory checkPosOrderStatusModelFactory,
            ICycleFlowSettingService cycleFlowSettingService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPosUserService posUserService,
            IWorkContext workContext
            )
        {
            _checkPosOrderStatusModelFactory = checkPosOrderStatusModelFactory;
            _cycleFlowSettingService = cycleFlowSettingService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _posUserService = posUserService;
            _workContext = workContext;
        }
        #endregion
        #region Methods
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual async Task<IActionResult> List()
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;

            var model = await _checkPosOrderStatusModelFactory.PrepareCheckPosOrderStatusSearchModelAsync(new CheckPosOrderStatusSearchModel());
            return View(model);
        }
        [HttpPost]
        public virtual async Task<IActionResult> List(CheckPosOrderStatusSearchModel searchModel)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME,true);
            if (result != null)
                return result;

            var model = await _checkPosOrderStatusModelFactory.PrepareCheckPosOrderStatusListModelAsync(searchModel);
            return Json(model);
        }
        public async Task<IActionResult> View(int id)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;

            var posUser = await _posUserService.GetPosUserByIdAsync(id);
            if (posUser == null)
            {
                _notificationService.ErrorNotification(_localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.NotFound").Result);
                return RedirectToAction(nameof(List));
            }
            var (sequenceHtml,status) = await _cycleFlowSettingService.CheckOrderStatusSequenceAsync(id);
            return View(model: sequenceHtml);
        }
        #endregion
    }
}
