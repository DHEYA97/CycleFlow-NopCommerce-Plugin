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
namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class ReturnController : BasePluginController
    {
        #region Fields
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IReturnModelFactory _returnModelFactory;
        private readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly IPosUserService _posUserService;
        private readonly IOrderStateOrderMappingService _orderStateOrderMappingService;
        private readonly IDeportationService _deportationService;
        private readonly ILocalizationService _localizationService;
        #endregion
        #region Ctor
        public ReturnController(
            IPermissionService permissionService,
            INotificationService notificationService,
            IReturnModelFactory returnModelFactory,
            ICycleFlowSettingService cycleFlowSettingService,
            IPosUserService posUserService,
            IOrderStateOrderMappingService orderStateOrderMappingService,
            IDeportationService deportationService,
            ILocalizationService localizationService
            )
        {
            _permissionService = permissionService;
            _notificationService = notificationService;
            _returnModelFactory = returnModelFactory;
            _cycleFlowSettingService = cycleFlowSettingService;
            _posUserService = posUserService;
            _orderStateOrderMappingService = orderStateOrderMappingService;
            _deportationService = deportationService;
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
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ShowAllOrderCycleFlowPlugin))
                return AccessDeniedView();
            await _cycleFlowSettingService.NotificationPosUserAsync();
            var model = await _returnModelFactory.PrepareReturnSearchModelAsync(new ReturnSearchModel());
            return View(model);
        }
        [HttpPost]
        public virtual async Task<IActionResult> List(ReturnSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ShowAllOrderCycleFlowPlugin))
                return await AccessDeniedDataTablesJson();
            await _cycleFlowSettingService.NotificationPosUserAsync();
            var model = await _returnModelFactory.PrepareReturnModelListModelAsync(searchModel);
            return Json(model);
        }
        public virtual async Task<IActionResult> View(int customerId,int year,int month)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ShowAllOrderCycleFlowPlugin))
                return AccessDeniedView();
            
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
