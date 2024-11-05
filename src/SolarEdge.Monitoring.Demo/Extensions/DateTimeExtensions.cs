using System;
using System.Collections.Generic;
using System.Globalization;

namespace SolarEdge.Monitoring.Demo.Extensions;

public static class DateTimeExtensions
{
  public static IEnumerable<DateTime> EachDay(this DateTime self, DateTime thru)
  {
    for (var day = self.Date; day.Date <= thru.Date; day = day.AddDays(1))
    {
      yield return day;
    }
  }

  public static DateTime StartOfThisMonth(this DateTime self)
  {
    return new DateTime(self.Year, self.Month, 1, 0, 0, 0, DateTimeKind.Utc);
  }

  public static DateTime StartOfThisWeek(this DateTime self)
  {
    var returnDateTime = self.AddDays(-(self.DayOfWeek - CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek));
    return returnDateTime;
  }

  public static DateTime StartOfDay(this DateTime self)
  {
    var returnDateTime = new DateTime(self.Year, self.Month, self.Day, 0, 0, 0, DateTimeKind.Utc);
    return returnDateTime;
  }

  public static DateTime EndOfDay(this DateTime self)
  {
    var returnDateTime = new DateTime(self.Year, self.Month, self.Day, 23, 59, 59, DateTimeKind.Utc);
    return returnDateTime;
  }

  public static DateTime StartOfThisYear(this DateTime self)
  {
    return new DateTime(self.Year, 1, 1,0, 0, 0, DateTimeKind.Utc);
  }

  public static string ToSqlDateTime(this DateTime self)
  {
    return self.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
  }
}