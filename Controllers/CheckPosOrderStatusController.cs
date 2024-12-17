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

namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class CheckPosOrderStatusController : BasePluginController
    {
        #region Fields
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ICheckPosOrderStatusModelFactory _checkPosOrderStatusModelFactory;
        private readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly IPosUserService _posUserService;
        private readonly ILocalizationService _localizationService;
        #endregion
        #region Ctor
        public CheckPosOrderStatusController(
            IPermissionService permissionService,
            INotificationService notificationService,
            ICheckPosOrderStatusModelFactory checkPosOrderStatusModelFactory,
            ICycleFlowSettingService cycleFlowSettingService,
            IPosUserService posUserService,
            ILocalizationService localizationService
            )
        {
            _permissionService = permissionService;
            _notificationService = notificationService;
            _checkPosOrderStatusModelFactory = checkPosOrderStatusModelFactory;
            _cycleFlowSettingService = cycleFlowSettingService;
            _posUserService = posUserService;
            _localizationService = localizationService;
        }
        #endregion
        #region Methods
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return AccessDeniedView();

            var model = await _checkPosOrderStatusModelFactory.PrepareCheckPosOrderStatusSearchModelAsync(new CheckPosOrderStatusSearchModel());
            return View(model);
        }
        [HttpPost]
        public virtual async Task<IActionResult> List(CheckPosOrderStatusSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return await AccessDeniedDataTablesJson();

            var model = await _checkPosOrderStatusModelFactory.PrepareCheckPosOrderStatusListModelAsync(searchModel);
            return Json(model);
        }
        public async Task<IActionResult> View(int id)
        {
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
