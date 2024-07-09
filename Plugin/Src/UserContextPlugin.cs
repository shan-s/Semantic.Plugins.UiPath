using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace LongreachAi.Semantic.Plugins;
[Description("Gets the details about the environment in which the user lives.")]
public class UserContextPlugin
{

    [KernelFunction("get_Current_DateTime")]
    [Description("Get the current date and time")]
    public string GetCurrentDateTime()
    {
        var timeNow = DateTime.Now;
        return $@"date: {timeNow.ToLongDateString()}, time: {timeNow.ToLongTimeString()}, weekDay: {timeNow.DayOfWeek}, 
            dayofYear: {timeNow.DayOfYear}, timeZoneName: {TimeZoneInfo.Local.DisplayName}";
    }

    [KernelFunction("get_Time_Difference")]
    [Description("Get the time difference between two dates")]
    public string GetTimeDifference([Description("The first date in yyyy-MM-dd HH:mm:ss format")] string date1,
    [Description("The second date n yyyy-MM-dd HH:mm:ss format")] string date2)
    {
        TimeSpan timeDifference = DateTime.Parse(date2) - DateTime.Parse(date1);
        return $"days: {timeDifference.Days}, hours: {timeDifference.Hours}, minutes: {timeDifference.Minutes}, seconds: {timeDifference.Seconds}";
    }


}
