﻿using Microsoft.AspNetCore.Mvc;
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
using Nop.Plugin.Misc.CycleFlow.Domain;
using DocumentFormat.OpenXml.Office2010.Excel;
using Nop.Plugin.Misc.CycleFlow.Constant.Enum;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Plugin.Misc.CycleFlow.Constant;
namespace Nop.Plugin.Misc.CycleFlow.Controllers
{
    public class DeportationController : BaseCycleFlowController
    {
        #region Fields
        private readonly ICycleFlowSettingService _cycleFlowSettingService;
        private readonly IDeportationModelFactory _deportationModelFactory;
        private readonly IDeportationService _deportationService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IOrderStateOrderMappingService _orderStateOrderMappingService;
        private readonly IPermissionService _permissionService;
        private readonly IPosUserService _posUserService;
        #endregion
        #region Ctor
        public DeportationController(
            ICycleFlowSettingService cycleFlowSettingService,
            IDeportationService deportationService,
            IDeportationModelFactory deportationModelFactory,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IOrderStateOrderMappingService orderStateOrderMappingService,
            IPermissionService permissionService,
            IPosUserService posUserService
            )
        {
            _cycleFlowSettingService = cycleFlowSettingService;
            _deportationModelFactory = deportationModelFactory;
            _deportationService = deportationService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _orderStateOrderMappingService = orderStateOrderMappingService;
            _permissionService = permissionService;
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
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.DeportationCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;

            
            await _cycleFlowSettingService.NotificationPosUserAsync();
            var model = await _deportationModelFactory.PrepareDeportationSearchModelAsync(new DeportationSearchModel(), true, false);
            return View(model);
        }
        [HttpPost]
        public virtual async Task<IActionResult> List(DeportationSearchModel searchModel)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.DeportationCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME,true);
            if (result != null)
                return result;

            await _cycleFlowSettingService.NotificationPosUserAsync();
            var model = await _deportationModelFactory.PrepareDeportationModelListModelAsync(searchModel);
            return Json(model);
        }
        public virtual async Task<IActionResult> AllOrder()
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ShowAllOrderCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;

            await _cycleFlowSettingService.NotificationPosUserAsync();
            var model = await _deportationModelFactory.PrepareDeportationSearchModelAsync(new DeportationSearchModel(), false);
            return View("List", model);
        }
        public virtual async Task<IActionResult> LastStepOrder()
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ShowAllOrderCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
                return result;

            await _cycleFlowSettingService.NotificationPosUserAsync();
            var model = await _deportationModelFactory.PrepareDeportationSearchModelAsync(new DeportationSearchModel(), justLastStepOrder: true);
            return View("List", model);
        }
        public virtual async Task<IActionResult> View(int id,bool showAllInfo = false)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.DeportationCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
            {
                var result2 = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ShowAllOrderCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
                if (result2 != null)
                { 
                    return result2;
                }
            }

            var orderStateOrderMapping = await _orderStateOrderMappingService.GetOrderStateOrderMappingByIdAsync(id);
            if(orderStateOrderMapping == null)
            {
                _notificationService.ErrorNotification(_localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.NotFound").Result);
                return RedirectToAction(nameof(List));
            }
            await _cycleFlowSettingService.NotificationPosUserAsync();
            var model = await _deportationModelFactory.PrepareDeportationModelAsync(null, orderStateOrderMapping,showAllInfo);
            return View(model);
        }
        public virtual async Task<IActionResult> Chart(int id)
        {
            var result = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.DeportationCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
            if (result != null)
            {
                var result2 = await CheckPermissionAndRoleAsync(CycleFlowPluginPermissionProvider.ShowAllOrderCycleFlowPlugin, SystemDefaults.CYCLE_FLOW_USER_ROLE_SYSTEM_NAME);
                if (result2 != null)
                {
                    return result2;
                }
            }
            var orderStateOrderMapping = await _orderStateOrderMappingService.GetOrderStateOrderMappingByIdAsync(id);
            if (orderStateOrderMapping == null)
            {
                _notificationService.ErrorNotification(_localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.NotFound").Result);
                return RedirectToAction(nameof(List));
            }
            await _cycleFlowSettingService.NotificationPosUserAsync();
            var model = await _deportationModelFactory.PrepareDeportationModelAsync(null, orderStateOrderMapping,false,skipLast:false);
            return View(model);
        }
        [HttpPost]
        public virtual async Task<IActionResult> DeportationProcess(DeportationModel model, Deportation deportationType)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.DeportationCycleFlowPlugin))
                return await AccessDeniedDataTablesJson();
            await _cycleFlowSettingService.NotificationPosUserAsync();
            if(ModelState.IsValid)
            {
                await _deportationService.DeportationAsync(model, deportationType);
                return RedirectToAction(nameof(List));
            }
            var orderStateOrderMapping = await _orderStateOrderMappingService.GetOrderStateOrderMappingByIdAsync(model.Id);
            if (orderStateOrderMapping == null)
            {
                _notificationService.ErrorNotification(_localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.NotFound").Result);
                return RedirectToAction(nameof(List));
            }
            await _cycleFlowSettingService.NotificationPosUserAsync();
            model = await _deportationModelFactory.PrepareDeportationModelAsync(model, orderStateOrderMapping,true);
            return View(nameof(View),model);
        }
        #endregion
    }
}
