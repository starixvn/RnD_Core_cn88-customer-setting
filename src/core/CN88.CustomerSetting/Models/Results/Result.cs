namespace CN88.CustomerSetting.Models.Results
{
    using AppResources;
    using Constants;

    public class Result : IResult
    {
        public static readonly Result Success = new Result(ResultValues.Success);
        public static readonly Result AccessDenied = new Result(ResultValues.AccessDenied, Resources.AccessDeniedMsg, MessageTypes.Error);
        public static readonly Result NoChange = new Result(ResultValues.NoChange, Resources.NoChangeToUpdate, MessageTypes.Warning);
        public static readonly Result SystemError = new Result(ResultValues.Error, Resources.SystemError, MessageTypes.Error);
        public static readonly Result DataError = new Result(ResultValues.DataIsNotValid, Resources.DataIsNotValid, MessageTypes.Error);

        public Result(int code)
            : this(code, string.Empty)
        {
        }

        public Result(int code, string message)
        {
            Code = code;
            Message = message;

            if(code == ResultValues.Success)
            {
                MsgType = ResultValues.MsgTypeSuccess;
            }
        }

        public Result(int code, string message, int msgType)
            : this(code, message)
        {
            MsgType = msgType;
        }

        public int Code { get; private set; }

        public string Message { get; private set; }

        public int MsgType { get; private set; }

        public virtual bool IsSuccessful => Code == ResultValues.Success;

        public virtual bool IsFailed => !IsSuccessful;

        public bool IsOk => IsSuccessful;

        public bool IsNotOk => !IsOk;

        public override bool Equals(object obj)
        {
            var otherObj = obj as Result;

            if (ReferenceEquals(this, otherObj))
            {
                return true;
            }

            if (otherObj == null)
            {
                return false;
            }

            return Code == otherObj.Code && Message == otherObj.Message;
        }

        public override int GetHashCode()
            => Code.GetHashCode();

        public static Result CreateErrorResult(string message) 
            => new Result(ResultValues.Error, message, MessageTypes.Error);

        public static Result CreateSuccessResult(string message)
            => new Result(ResultValues.Success, message);
    }
}