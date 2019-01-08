namespace CN88.CustomerSetting.Models
{
    public interface ISettingContext
    {
        User Actor { get; set; }

        string Language { get; }

        string IP { get; }

        byte System { get; set; }

        string Domain { get; }
    }
}