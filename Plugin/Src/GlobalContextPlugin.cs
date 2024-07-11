using Azure.Core;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Globalization;

namespace LongreachAi.Semantic.Plugins;

public class GlobalContextPlugin
{

    [KernelFunction("get_Current_DateTime_LocalTimezone")]
    [Description("Get the current date and time in local timezone")]
    public string GetCurrentDateTime()
    {
        var timeNow = DateTime.Now;
        return $@"date: {timeNow.ToLongDateString()}, time: {timeNow.ToLongTimeString()}, weekDay: {timeNow.DayOfWeek}, 
            dayofYear: {timeNow.DayOfYear}, localTimeZoneName: {TimeZoneInfo.Local.DisplayName}, utcTimeOffset: {TimeZoneInfo.Local.BaseUtcOffset}";
    }

    [KernelFunction("get_Current_DateTime_UTC")]
    [Description("Get the current date and time in UTC timezone")]
    public static string GetCurrentDateTimeUTC()
    {
        var timeNow = DateTime.UtcNow;
        return $@"date: {timeNow.ToLongDateString()}, time: {timeNow.ToLongTimeString()}, weekDay: {timeNow.DayOfWeek}, 
            dayofYear: {timeNow.DayOfYear}, timeZoneName: {TimeZoneInfo.Utc.DisplayName}, utcTimeOffset: {TimeZoneInfo.Utc.BaseUtcOffset}";
    }

    [KernelFunction("get_TimeZone_List")]
    [Description("Get the list of timezones")]
    public static string GetTimeZoneList()
    {
        var timeZones = TimeZoneInfo.GetSystemTimeZones();
        return string.Join("; ", timeZones.Select(tz => $@"utcTimeOffset: {tz.BaseUtcOffset},
            timeZoneName: {tz.DisplayName}"));
    }

    [KernelFunction("get_Current_DateTime_SpecifiedTimezone")]
    [Description("Given a utc Time Offset and timezone name, gets the current date and time in the specified timezone")]
    public string GetCurrentDateTime(string utcTimeOffset, string timeZoneName)
    {
        var timeNow = DateTime.UtcNow;
        var timeZone = TimeZoneInfo.CreateCustomTimeZone(timeZoneName, TimeSpan.Parse(utcTimeOffset), timeZoneName, timeZoneName);
        var customTime = TimeZoneInfo.ConvertTimeFromUtc(timeNow, timeZone);
        return $@"date: {customTime.ToLongDateString()}, time: {customTime.ToLongTimeString()}, weekDay: {customTime.DayOfWeek}, 
            dayofYear: {customTime.DayOfYear}, timeZoneName: {timeZone.DisplayName}, utcOffset: {timeZone.BaseUtcOffset}";
    }

    [KernelFunction("get_days_Time_Difference")]
    [Description("Get the days and time difference between two datetimes in the same timezone")]
    public static string GetTimeDifference([Description("The first datetime in 'yyyy-MM-dd HH:mm:ss' format")] string date1,
    [Description("The second datetime in 'yyyy-MM-dd HH:mm:ss' format")] string date2)
    {
        TimeSpan timeDifference = DateTime.Parse(date2).Subtract(DateTime.Parse(date1));
        return $"days: {timeDifference.Days}, hours: {timeDifference.Hours}, minutes: {timeDifference.Minutes}, seconds: {timeDifference.Seconds}";
    }


}
