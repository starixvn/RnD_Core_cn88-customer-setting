namespace CN88.CustomerSetting.Client.Attributes
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Configurations;
    using Models;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    internal sealed class CustomerSettingAuth : AuthorizeAttribute
    {
        public bool IsUpdate { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            PermissionContext permissionContext = CN88CustomerSettingRegistration.GetPermissionContext();

            permissionContext.AuthorizationContext = filterContext;
            permissionContext.AllowUpdate = IsUpdate;

            var isAuthorized = true;

            if (IsUpdate)
            {
                isAuthorized = CN88CustomerSettingRegistration.CheckUpdatePermission(permissionContext);
            }
            else
            {
                isAuthorized = CN88CustomerSettingRegistration.CheckQueryPermission(permissionContext);
            }

            if (!isAuthorized)
            {
                UnAuthorizedAction(filterContext);
            }
        }

        private void UnAuthorizedAction(AuthorizationContext filterContext)
        {
            HttpResponseBase response = filterContext.HttpContext.Response;

            if (filterContext.HttpContext.Request.IsAjaxRequest()
                || filterContext.HttpContext.Request.HttpMethod.ToUpper() == "POST")
            {
                response.Clear();
                response.Write("{ \"message\": \"You are not authorized on this function!\" }");
                filterContext.Result = new HttpUnauthorizedResult();
                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                response.TrySkipIisCustomErrors = true;
                response.End();
            }
            else
            {
                filterContext.Result = new RedirectResult(CN88CustomerSettingConfiguration.Settings.AccessDeniedAction);
            }
        }
    }
}