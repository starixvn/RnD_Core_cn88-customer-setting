namespace CN88.CustomerSetting.Models
{
    using System.Collections.Generic;
    using CN88.CustomerSetting.Models.Enumerations;
    using CN88.CustomerSetting.Models.Results;

    public interface ISetting
    {
        int CustomerId { get; set; }

        Level CustomerLevel { get; }

        byte ProductId { get; }

        string ProductName { get; }

        bool IsCreateNew { get; }

        IEnumerable<SettingType> SettingTypes { get; }

        IResult WarningMessage { get; set; }
    }
}
