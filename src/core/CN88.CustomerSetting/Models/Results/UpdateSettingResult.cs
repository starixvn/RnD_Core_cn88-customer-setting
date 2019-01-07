namespace CN88.CustomerSetting.Models.Results
{
    using System.Collections.Generic;
    using System.Linq;

    public class UpdateSettingResult : Result
    {
        public UpdateSettingResult(IEnumerable<SettingResult> subResults)
            : base(GetRepresentativeCode(subResults))
        {
            SubResults = subResults;
        }

        public UpdateSettingResult(IEnumerable<IResult> results)
            : base(GetRepresentativeCode(results))
        {
            if (results != null)
            {
                var subResults = new List<SettingResult>();

                foreach (IResult result in results)
                {
                    var updateSettingResult = result as UpdateSettingResult;

                    if (updateSettingResult != null)
                    {
                        subResults.AddRange(updateSettingResult.SubResults);
                    }
                    else
                    {
                        var settingResult = result as SettingResult;

                        if (settingResult != null)
                        {
                            subResults.Add(settingResult);
                        }
                        else
                        {
                            subResults.Add(new SettingResult(result));
                        }
                    }
                }

                SubResults = subResults;
            }
        }

        public UpdateSettingResult(IResult result)
            : base(result.Code, result.Message)
        {
            SubResults = new List<SettingResult> { new SettingResult(result) };
        }

        public IEnumerable<SettingResult> SubResults { get; private set; }

        public override bool IsSuccessful
            => SubResults != null && SubResults.Any()
                ? SubResults.FirstOrDefault(x => !x.IsSuccessful) == null
                : base.IsSuccessful;

        private static int GetRepresentativeCode(IEnumerable<IResult> results)
            => results.Any(result => result.IsFailed)
                ? results.FirstOrDefault(result => result.IsFailed).Code
                : 0;
    }
}