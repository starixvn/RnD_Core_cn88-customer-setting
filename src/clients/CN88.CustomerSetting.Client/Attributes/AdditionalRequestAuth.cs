namespace CN88.CustomerSetting.Client.Attributes
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Configurations;
    using Models;
    using Models.Constants;

    using static System.Net.WebRequestMethods;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    internal sealed class AdditionalRequestAuth : AuthorizeAttribute
    {
        private const string ActionTypeName = "actionType";

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            base.OnAuthorization(filterContext);

            var isAuthorized = false;
            var actionType = filterContext.Controller.ValueProvider.GetValue(ActionTypeName);

            if (actionType != null)
            {
                byte actualActionType = Convert.ToByte(actionType.AttemptedValue);
                PermissionContext permissionContext = CN88CustomerSettingRegistration.GetPermissionContext();

                permissionContext.AuthorizationContext = filterContext;
                permissionContext.AllowUpdate = actualActionType == ActionTypes.Update;

                switch (actualActionType)
                {
                    case ActionTypes.Query:
                        isAuthorized = CN88CustomerSettingRegistration.CheckQueryPermission(permissionContext);
                        break;

                    case ActionTypes.Update:
                        isAuthorized = CN88CustomerSettingRegistration.CheckUpdatePermission(permissionContext);
                        break;
                }
            }

            if (!isAuthorized)
            {
                UnAuthorizedAction(filterContext);
            }
        }

        private void UnAuthorizedAction(AuthorizationContext filterContext)
        {
            HttpResponseBase response = filterContext.HttpContext.Response;

            if (filterContext.HttpContext.Request.IsAjaxRequest() || Http.Post == filterContext.HttpContext.Request.HttpMethod)
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