using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Misc.CycleFlow.Models.CheckPosOrderStatus;
using Nop.Plugin.Misc.CycleFlow.Permission;
using Nop.Plugin.Misc.CycleFlow.Factories;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.POSSystem.Services;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Plugin.Misc.CycleFlow.Models.Deportation;

namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class DeportationController : BasePluginController
    {
        #region Fields
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IDeportationModelFactory _deportationModelFactory;
        private readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly IPosUserService _posUserService;
        #endregion
        #region Ctor
        public DeportationController(
            IPermissionService permissionService,
            INotificationService notificationService,
            IDeportationModelFactory deportationModelFactory,
            ICycleFlowSettingService cycleFlowSettingService,
            IPosUserService posUserService
            )
        {
            _permissionService = permissionService;
            _notificationService = notificationService;
            _deportationModelFactory = deportationModelFactory;
            _cycleFlowSettingService = cycleFlowSettingService;
            _posUserService = posUserService;
        }
        #endregion
        #region Methods
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.DeportationCycleFlowPlugin))
                return AccessDeniedView();
            await _cycleFlowSettingService.NotificationPosUserAsync();
            var model = await _deportationModelFactory.PrepareDeportationSearchModelAsync(new DeportationSearchModel());
            return View(model);
        }
        [HttpPost]
        public virtual async Task<IActionResult> List(DeportationSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.DeportationCycleFlowPlugin))
                return await AccessDeniedDataTablesJson();
            await _cycleFlowSettingService.NotificationPosUserAsync();
            var model = await _deportationModelFactory.PrepareDeportationModelListModelAsync(searchModel);
            return Json(model);
        }
        #endregion
    }
}
