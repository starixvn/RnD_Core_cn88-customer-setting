namespace CN88.CustomerSetting.Models.Results
{
    public interface IResult
    {
        int Code { get; }

        string Message { get; }

        /// <summary>
        /// Gets the type of the MSG.
        /// </summary>
        /// <value>
        /// Message type for more details about message return after updated.
        /// </value>
        int MsgType { get; }

        bool IsSuccessful { get; }

        bool IsFailed { get; }

        bool IsOk { get; }

        bool IsNotOk { get; }
    }
}