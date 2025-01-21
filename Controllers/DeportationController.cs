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
using Nop.Plugin.Misc.CycleFlow.Domain;
using DocumentFormat.OpenXml.Office2010.Excel;
using Nop.Plugin.Misc.CycleFlow.Constant.Enum;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Plugin.Misc.CycleFlow.Constant;
using Nop.Core.Domain.Catalog;
using Nop.Core;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc;
using Nop.Services.Media;
using DocumentFormat.OpenXml.EMMA;
using Nop.Core.Infrastructure;
using Microsoft.Extensions.FileProviders;
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
        private readonly IPictureService _pictureService;
        private readonly INopFileProvider _fileProvider;
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
            IPosUserService posUserService,
            IPictureService pictureService,
            INopFileProvider fileProvider
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
            _pictureService = pictureService;
            _fileProvider = fileProvider;
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
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Nop.Plugin.Misc.CycleFlow.Deportation.NotFound"));
                return RedirectToAction(nameof(List));
            }
            await _cycleFlowSettingService.NotificationPosUserAsync();
            model = await _deportationModelFactory.PrepareDeportationModelAsync(model, orderStateOrderMapping,true);
            return View(nameof(View),model);
        }

       
        [HttpPost]
        public virtual async Task<IActionResult> OrderStatusPictureList(OrderStatusPictureSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.DeportationCycleFlowPlugin))
                return await AccessDeniedDataTablesJson();

            var orderStateOrderMapping = await _orderStateOrderMappingService.GeOrderStateOrderMappingAsync(searchModel)
                ?? throw new ArgumentException("No Order State Order Mapping found with the specified id");

            var model = await _deportationModelFactory.PrepareOrderStatusPictureListModelAsync(searchModel, orderStateOrderMapping);

            return Json(model);
        }
        [HttpPost]
        public virtual async Task<IActionResult> OrderStatusPictureAdd(int pictureId, int posUserId ,int orderId ,int orderStatusId,int storeId , int customerId, string? ImageData = null)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.DeportationCycleFlowPlugin))
                return AccessDeniedView();

            if (pictureId == 0 && string.IsNullOrEmpty(ImageData))
                throw new ArgumentException();

            var searchModel = new OrderStatusPictureSearchModel {
                OrderId = orderId,
                OrderStatusId = orderStatusId,
                PosUserId = posUserId,
            };
            var orderStateOrderMapping = await _orderStateOrderMappingService.GeOrderStateOrderMappingAsync(searchModel)
                ?? throw new ArgumentException("No Order State Order Mapping found with the specified id");

            
            if ((await _orderStateOrderMappingService.GetAllOrderStateOrderImageMappingPictureOrderStatusIdAsync(searchModel.PosUserId,searchModel.OrderId,searchModel.OrderStatusId)).Any(p => p.PictureId == pictureId))
                return Json(new { Result = false });

            var uploadsFolder = _fileProvider.Combine("wwwroot", "images", "order", $"{orderId}");
            _fileProvider.CreateDirectory(uploadsFolder);

            if (pictureId == 0 && !string.IsNullOrEmpty(ImageData))
            {
                try
                {
                    var imageBytes = Convert.FromBase64String(ImageData);
                    var uploadPicture = await _pictureService.InsertPictureAsync(
                           imageBytes, MimeTypes.ImagePng,string.Empty
                        );
                    pictureId = uploadPicture.Id;
                }
                catch (FormatException)
                {
                    _fileProvider.DeleteDirectory(uploadsFolder);
                    return BadRequest("Invalid Base64 string");
                }
                catch (Exception ex)
                {
                    _fileProvider.DeleteDirectory(uploadsFolder);
                    return StatusCode(500, "Internal server error");
                }
            }
            var picture = await _pictureService.GetPictureByIdAsync(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            var pictureBinary = await _pictureService.LoadPictureBinaryAsync(picture);
            await _pictureService.UpdatePictureAsync(picture.Id,
                pictureBinary,
                picture.MimeType,
                picture.SeoFilename);

            await _orderStateOrderMappingService.InsertOrderStateOrderImageMappingAsync(
                new OrderStateOrderImageMapping
                {
                    PictureId = pictureId,
                    OrderId = orderId,
                    OrderStatusId = orderStatusId,
                    PosUserId = posUserId,
                    CustomerId = customerId,
                    NopStoreId  = storeId,
                    OrderStateOrderMappingId = null
                }
            );
            
            var uniqueFileName = $"{pictureId}.png";
            var filePath = _fileProvider.Combine(uploadsFolder, uniqueFileName);
            await _fileProvider.WriteAllBytesAsync(filePath, pictureBinary);

            return Json(new { Result = true });
        }
        [HttpPost]
        public virtual async Task<IActionResult> OrderStatusPictureDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(CycleFlowPluginPermissionProvider.DeportationCycleFlowPlugin))
                return AccessDeniedView();

            var orderStateImage = await _orderStateOrderMappingService.GetOrderStateOrderImageMappingByIdAsync(id)
                ?? throw new ArgumentException("No Order State Order Mapping picture found with the specified id");



            var pictureId = orderStateImage.PictureId;
            await _orderStateOrderMappingService.DeleteOrderStateOrderImageMappingAsync(orderStateImage);

            var picture = await _pictureService.GetPictureByIdAsync(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            await _pictureService.DeletePictureAsync(picture);
            var orderId = orderStateImage.OrderId;

            var uploadsFolder = _fileProvider.Combine("wwwroot", "images", "order", $"{orderId}");
            var filePath = _fileProvider.Combine(uploadsFolder, $"{picture.Id}.png");

            if (_fileProvider.FileExists(filePath))
            {
                try
                {
                    _fileProvider.DeleteFile(filePath);
                }
                catch (Exception ex)
                {
                    _notificationService.ErrorNotification($"Can not Delete Picture {pictureId}");
                }
            }

            return new NullJsonResult();
        }
        #endregion
    }
}
