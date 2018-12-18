namespace CN88.CustomerSetting.Client.Models
{
    using System.Web.Mvc;
    using Requests;

    public class PermissionContext : SettingRequest
    {
        public bool AllowUpdate { get; set; }

        public AuthorizationContext AuthorizationContext { get; set; }
    }
}