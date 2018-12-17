namespace CN88.CustomerSetting.Models.Messages
{
    using System.Xml.Serialization;

    [XmlRoot("Root")]
    public class SimpleMessage
    {
        [XmlAttribute("MsgID")]
        public int MessageId { get; set; }

        [XmlAttribute("Msg")]
        public string Message { get; set; }
    }
}