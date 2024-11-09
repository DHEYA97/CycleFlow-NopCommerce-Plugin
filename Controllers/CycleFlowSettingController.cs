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
using Nop.Services.Security;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Plugin.Misc.CycleFlow.Models.ImageType;
using Nop.Plugin.Misc.CycleFlow.Factories;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;
using Nop.Plugin.Misc.POSSystem.Areas.Admin.Factories;
using Nop.Plugin.Misc.POSSystem.Areas.Admin.Models.Purchase;
using Nop.Plugin.Misc.POSSystem.Domains;
using Nop.Plugin.Misc.CycleFlow.Domain;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Plugin.Misc.POSSystem.Events;
using Nop.Plugin.Misc.POSSystem.ZatcaEInvoice.Settings;
using DocumentFormat.OpenXml.EMMA;

namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class CycleFlowSettingController : BasePluginController
    {
        #region Fields
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ICycleFlowSettingModelFactory _cycleFlowSettingFactory;
        private readonly ICycleFlowSettingService _cycleFlowSettingService; 
        #endregion
        #region Ctor
        public CycleFlowSettingController(
            IPermissionService permissionService,
            INotificationService notificationService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            ICycleFlowSettingModelFactory cycleFlowSettingFactory,
            ICycleFlowSettingService cycleFlowSettingService
            )
        {
            _permissionService = permissionService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _cycleFlowSettingFactory = cycleFlowSettingFactory;
            _cycleFlowSettingService = cycleFlowSettingService;
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
           
           var model = await _cycleFlowSettingFactory.PrepareCycleFlowSettingSearchModelAsync(new CycleFlowSettingSearchModel());
            return View(model);
        }
        [HttpPost]
        public virtual async Task<IActionResult> List(CycleFlowSettingSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.ManageCycleFlowPlugin))
                return await AccessDeniedDataTablesJson();

            var model = await _cycleFlowSettingFactory.PrepareCycleFlowSettingListModelAsync(searchModel);
            return Json(model);
        }
        public async Task<IActionResult> Create()
        {
            var model = await _cycleFlowSettingFactory.PrepareCycleFlowSettingModelAsync(new CycleFlowSettingModel(), null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Create(CycleFlowSettingModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                await _cycleFlowSettingService.InsertCycleFlowSettingAsync(model);

                if (continueEditing)
                    return RedirectToAction(nameof(Edit), new { id = model.Id });

                return RedirectToAction(nameof(List));
            }

            model = await _cycleFlowSettingFactory.PrepareCycleFlowSettingModelAsync(model, null);
            return View(model);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var orderStatusSorting = await _cycleFlowSettingService.GetOrderStatusSortingByIdAsync(id);

            if (orderStatusSorting == null)
                return RedirectToAction(nameof(List));

            var model = await _cycleFlowSettingFactory.PrepareCycleFlowSettingModelAsync(null, orderStatusSorting);
            return View(model);
        }
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Edit(CycleFlowSettingModel model, bool continueEditing)
        {
            var orderStatusSorting = await _cycleFlowSettingService.GetOrderStatusSortingByIdAsync(model.Id);

            if (orderStatusSorting == null)
                return RedirectToAction(nameof(List));

            if (ModelState.IsValid)
            {
                orderStatusSorting = model.ToEntity(orderStatusSorting);
                await _cycleFlowSettingService.UpdateCycleFlowSettingAsync(model);

                if (continueEditing)
                {
                    return RedirectToAction(nameof(Edit), new { id = orderStatusSorting.Id });
                }
                return RedirectToAction(nameof(List));
            }
            model = await _cycleFlowSettingFactory.PrepareCycleFlowSettingModelAsync(model, orderStatusSorting);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var orderStatusSorting = await _cycleFlowSettingService.GetOrderStatusSortingByIdAsync(id);

            if (orderStatusSorting == null)
            {
                _notificationService.ErrorNotification("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.NotFound");
                return RedirectToAction(nameof(List));
            }

            await _cycleFlowSettingService.DeleteCycleFlowSettingAsync(orderStatusSorting);
            return RedirectToAction(nameof(List));
        }
        #endregion
    }
}