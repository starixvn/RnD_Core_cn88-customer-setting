namespace CN88.CustomerSetting.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Exceptions;
    using Helpers;
    using Models;

    public abstract class CN88CustomerSettingRegistration
    {
        private static CN88CustomerSettingRegistration client;
        internal const string UrlRootName = "CN88CustomerSetting";
        private const string EncryptionKeySessionName = "CN88.CustomerSetting.Client.EncryptionKey";

        internal static SettingContext SettingContext
        {
            get
            {
                var settingContext = client != null 
                    ? client.FetchSettingContext()
                    : new SettingContext();

                settingContext.HashKey = GetEncryptionKey();

                return settingContext;
            }
        }

        private static string GetEncryptionKey()
        {
            var encryptionKey = HttpContext.Current.Session?[EncryptionKeySessionName];

            if(string.IsNullOrWhiteSpace(encryptionKey as string))
            {
                encryptionKey = RandomHelper.RandomString(4);
                HttpContext.Current.Session[EncryptionKeySessionName] = encryptionKey;
            }

            return encryptionKey.ToString();
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

        protected virtual void RegisterRouteConfig(RouteCollection routes)
        {
            routes.MapRoute(
                name: "CN88_Setting_View",
#pragma warning disable S1075 // URIs should not be hardcoded
                url: UrlRootName + "/ViewCN88CustomerSetting/{action}/{id}",
#pragma warning restore S1075 // URIs should not be hardcoded
                defaults: new { controller = "ViewCN88CustomerSetting", action = "Edit", id = UrlParameter.Optional },
                namespaces: new string[] { "CN88.CustomerSetting.Client.Controllers" });

            routes.MapRoute(
                name: "CN88_Setting_Create",
#pragma warning disable S1075 // URIs should not be hardcoded
                url: UrlRootName + "/CreateCN88Customer/{action}/{id}",
#pragma warning restore S1075 // URIs should not be hardcoded
                defaults: new { controller = "CreateCN88Customer", action = "Create", id = UrlParameter.Optional },
                namespaces: new string[] { "CN88.CustomerSetting.Client.Controllers" });

            routes.MapRoute(
                name: "CN88_Setting_Update",
#pragma warning disable S1075 // URIs should not be hardcoded
                url: UrlRootName + "/UpdateCN88CustomerSetting/{action}/{id}",
#pragma warning restore S1075 // URIs should not be hardcoded
                defaults: new { controller = "UpdateCN88CustomerSetting", action = "Update", id = UrlParameter.Optional },
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