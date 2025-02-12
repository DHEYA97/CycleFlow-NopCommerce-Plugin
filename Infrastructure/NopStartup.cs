﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.CycleFlow.Factories;
using Nop.Plugin.Misc.CycleFlow.Services;
using Nop.Plugin.Misc.POSSystem.Areas.Admin.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.CycleFlow.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => 2000;

        public void Configure(IApplicationBuilder application)
        {
            
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CycleFlowViewLocationExpander());
            });
            #region Factory
            services.AddScoped<IBaseCycleFlowModelFactory, BaseCycleFlowModelFactory>();
            services.AddScoped<ICheckPosOrderStatusModelFactory, CheckPosOrderStatusModelFactory>();
            services.AddScoped<ICycleFlowSettingModelFactory, CycleFlowSettingModelFactory>();
            services.AddScoped<IDeportationModelFactory, DeportationModelFactory>();
            services.AddScoped<IImageTypeModelFactory, ImageTypeModelFactory>();
            services.AddScoped<IOrderStatusModelFactory, OrderStatusModelFactory>();
            services.AddScoped<IReturnModelFactory, ReturnModelFactory>();
            #endregion

            #region Service
            services.AddScoped<ICycleFlowSettingService, CycleFlowSettingService>();
            services.AddScoped<IDeportationService, DeportationService>();
            services.AddScoped<IImageTypeService, ImageTypeService>();
            services.AddScoped<IOrderStatusService, OrderStatusService>();
            services.AddScoped<IOrderStateOrderMappingService, OrderStateOrderMappingService>();
            #endregion

        }
    }
}
