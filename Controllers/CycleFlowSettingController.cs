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
using System.Diagnostics.Metrics;


namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    [Area(AreaNames.Admin)]
    [AuthorizeAdmin]
    [AutoValidateAntiforgeryToken]
    public class CycleFlowSettingController : BasePluginController
    {
        #region Fields
        private readonly ICycleFlowSettingModelFactory _cycleFlowSettingFactory;
        private readonly ICycleFlowSettingService _cycleFlowSettingService; 
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IOrderStatusService _orderStatusService;
        #endregion
        #region Ctor
        public CycleFlowSettingController(
            ICycleFlowSettingModelFactory cycleFlowSettingFactory,
            ICycleFlowSettingService cycleFlowSettingService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IOrderStatusService orderStatusService
            )
        {
            _cycleFlowSettingFactory = cycleFlowSettingFactory;
            _cycleFlowSettingService = cycleFlowSettingService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
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
                _notificationService.ErrorNotification(_localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.NotFound").Result);
                return RedirectToAction(nameof(List));
            }

            await _cycleFlowSettingService.DeleteCycleFlowSettingAsync(orderStatusSorting);
            return RedirectToAction(nameof(List));
        }
        public virtual async Task<IActionResult> GetFirstStepByCurrentStep(int posUserId, bool? addSelectStateItem, bool exclude = false)
        {
            if (posUserId <= 0)
                throw new ArgumentNullException(nameof(posUserId));

            var firstList = posUserId > 0 ? (await _cycleFlowSettingService.GetFirstOrderStatusAsync(posUserId, exclude:exclude))
                                        : new List<(string, string)>();
            var result = (from s in firstList
                          select new { id = Convert.ToInt32(s.Item1), name = s.Item2 }).ToList();

            if (result == null)
            {
                if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                {
                    result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.SelectOrderStatus") });
                }
                else
                {
                    result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.Other") });
                }
            }
            else
            {
                if (!result.Any())
                {
                    result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.Other") });
                }
                else
                {
                    if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                    {
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.SelectOrderStatus") });
                    }
                }
            }

            return Json(result);
        }
        public virtual async Task<IActionResult> GetNextStepByCurrentStep(int posUserId,int currentStepId, bool? addSelectStateItem, bool exclude = false)
        {
            
            if (posUserId <= 0)
                throw new ArgumentNullException(nameof(posUserId));

            if (currentStepId <= 0)
                throw new ArgumentNullException(nameof(currentStepId));

            var orderStateSorting = await _orderStatusService.GetOrderStatusByIdAsync(currentStepId);
            var nextList = orderStateSorting != null ? (await _cycleFlowSettingService.GetNextOrderStatusAsync(posUserId, currentStepId, exclude)) : new List<(string,string,int)>();
            var result = (from s in nextList
                          select new { id = Convert.ToInt32(s.Item1), name = s.Item2 , next = Convert.ToInt32(s.Item3) }).ToList();
            
                if (orderStateSorting == null)
                {
                    if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                    {
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.SelectNextOrderStatus"), next = 0 });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.Other") ,next = 0 });
                    }
                }
                else
                {
                    if (!result.Any())
                    {
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.Other"), next = 0 });
                    }
                    else
                    {
                        if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                        {
                            result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.SelectNextOrderStatus"), next = 0 });
                        }
                    }
            }

            return Json(result);
        }
        public virtual async Task<IActionResult> GetReturnByCurrentStep(int posUserId, int currentStepId, bool? addSelectStateItem, bool exclude = false)
        {

            if (posUserId <= 0)
                throw new ArgumentNullException(nameof(posUserId));

            if (currentStepId <= 0)
                throw new ArgumentNullException(nameof(currentStepId));

            var orderStateSorting = await _orderStatusService.GetOrderStatusByIdAsync(currentStepId);
            var returnList = orderStateSorting != null ? (await _cycleFlowSettingService.GetReturnOrderStatusAsync(posUserId, currentStepId, exclude)) : new List<(string, string,int)>();
            var result = (from s in returnList
                          select new { id = Convert.ToInt32(s.Item1), name = s.Item2,retern = s.Item3}).ToList();

            if (orderStateSorting == null)
            {
                if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                {
                    result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.SelectNextOrderStatus") , retern = 0 });
                }
                else
                {
                    result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.Other"), retern = 0 });
                }
            }
            else
            {
                if (!result.Any())
                {
                    result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.Other"), retern = 0 });
                }
                else
                {
                    if (addSelectStateItem.HasValue && addSelectStateItem.Value)
                    {
                        result.Insert(0, new { id = 0, name = await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.OrderStatusSorting.SelectNextOrderStatus"), retern = 0 });
                    }
                }
            }

            return Json(result);
        }

        #endregion
    }
}