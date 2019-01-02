namespace CN88.CustomerSetting.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Exceptions;
    using Helpers;
    using Models;

    public abstract class CN88CustomerSettingRegistration
    {
        private static CN88CustomerSettingRegistration client;
        public static string UrlRootName = "CN88CustomerSetting";

        internal static SettingContext SettingContext
        {
            get
            {
                if (client != null)
                {
                    return client.FetchSettingContext();
                }

                return new SettingContext();
            }
        }

        public static void Init()
        {
            List<Type> customerSettingClients = TypeDetection.FilterTypesInAssemblies(IsCustomerSettingClient).ToList();

            if (customerSettingClients.Count == 0)
            {
                throw new CustomerSettingException("A class extending from Customer Setting Registration is required");
            }
            else if (customerSettingClients.Count == 1)
            {
                Type clientType = customerSettingClients[0];

                client = Activator.CreateInstance(clientType) as CN88CustomerSettingRegistration;
                client.RegisterRouteConfig(RouteTable.Routes);
            }
            else
            {
                throw new CustomerSettingException("Only one class extending from Customer Setting Registration is allowed");
            }
        }

        internal static bool CheckQueryPermission(PermissionContext checkingPermissionContext)
        {
            CheckInitializationState();

            return client.VerifyQueryPermission(checkingPermissionContext);
        }

        internal static bool CheckUpdatePermission(PermissionContext checkingPermissionContext)
        {
            CheckInitializationState();

            return client.VerifyUpdatePermission(checkingPermissionContext);
        }

        public abstract bool VerifyQueryPermission(PermissionContext permissionContext);

        public abstract bool VerifyUpdatePermission(PermissionContext permissionContext);

        public abstract bool VerifyMultipleUpdatePermission(PermissionContext permissionContext);

        public abstract SettingContext FetchSettingContext();

        private void RegisterRouteConfig(RouteCollection routes)
        {
            routes.MapRoute(
                name: "CN88_Setting",
                url: UrlRootName + "/{controller}/{action}/{id}",
                defaults: new { controller = "ViewCustomerSetting", action = "Edit", id = UrlParameter.Optional },
                namespaces: new string[] { "CN88.CustomerSetting.Client.Controllers" });
        }

        private static bool IsCustomerSettingClient(Type type)
        {
            return typeof(CN88CustomerSettingRegistration).IsAssignableFrom(type) 
                && type.GetConstructor(Type.EmptyTypes) != null;
        }

        private static void CheckInitializationState()
        {
            if (client == null)
            {
                throw new CustomerSettingException("The Customer Setting Registration Init method must be called once the web application is started.");
            }
        }

        internal static PermissionContext GetPermissionContext()
        {
            SettingContext settingContext = SettingContext;
            var context = new PermissionContext();

            context.ActorId = settingContext.ActorId;
            context.ActorLevel = settingContext.ActorLevel;
            context.ActorName = settingContext.ActorName;
            context.SubAccountId = settingContext.SubAccountId;
            context.Domain = settingContext.Domain;
            context.Language = settingContext.Language;
            context.ClientIP = settingContext.IP;
            context.System = settingContext.System;

            return context;
        }
    }
}