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
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Plugin.Misc.CycleFlow.Factories;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.CycleFlow.Models.CycleFlowSetting;


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
        private readonly IOrderStatusService _orderStatusService;
        #endregion
        #region Ctor
        public CycleFlowSettingController(
            IPermissionService permissionService,
            INotificationService notificationService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            ICycleFlowSettingModelFactory cycleFlowSettingFactory,
            ICycleFlowSettingService cycleFlowSettingService,
            IOrderStatusService orderStatusService
            )
        {
            _permissionService = permissionService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _cycleFlowSettingFactory = cycleFlowSettingFactory;
            _cycleFlowSettingService = cycleFlowSettingService;
            _orderStatusService = orderStatusService;
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

            var model = await _cycleFlowSettingFactory.PrepareCycleFlowSettingModelAsync(null, orderStatusSorting,currentId:id);
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
            model = await _cycleFlowSettingFactory.PrepareCycleFlowSettingModelAsync(model, orderStatusSorting, currentId: model.Id);
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
        public virtual async Task<IActionResult> GetNextStepByCurrentStep(int currentStepId, bool? addSelectStateItem, bool exclude = false)
        {
            if (currentStepId <= 0)
                throw new ArgumentNullException(nameof(currentStepId));

            var orderStateSorting = await _orderStatusService.GetOrderStatusByIdAsync(currentStepId);
            var states = orderStateSorting != null ? (await _cycleFlowSettingService.GetNextOrderStatusAsync(exclude, currentStepId)) : new List<(string,string)>();
            var result = (from s in states
                          select new { id = Convert.ToInt32(s.Item1), name = s.Item2 }).ToList();
            
                if (orderStateSorting == null)
                {
                    if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                    {
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.SelectState") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.Other") });
                    }
                }
                else
                {
                    if (!result.Any())
                    {
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.Other") });
                    }
                    else
                    {
                        if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                        {
                            result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Admin.Address.SelectState") });
                        }
                    }
            }

            return Json(result);
        }
        #endregion
    }
}