namespace CN88.CustomerSetting.Models.Results
{
    public class CreateNewResult : Result
    {
        public CreateNewResult(int code, string message, int custId) 
            : base(code, message)
        {
            CustId = custId;
        }

        public int CustId { get; }
    }
}
