using System;

namespace WpfApp1
{
    internal enum DateTimeCategory
    {
        Today,
        Yesterday,
        ThisWeek,
        LastWeek,
        ThisMonth,
        LastMonth,
        MorePast,
    }

    internal static class DateTimeCategoryExtensions
    {
        public static DateTimeCategory CategorizeDateTime(this DateTime lastLogin)
        {
            var lastLoginDate = lastLogin.Date;
            var todayDate = DateTime.Now.Date;
            switch ((todayDate - lastLoginDate).Days)
            {
                case 0:
                    return DateTimeCategory.Today;
                case 1:
                    return DateTimeCategory.Yesterday;
            };
            switch ((todayDate.AddDays(-(int)todayDate.DayOfWeek) -
                    lastLoginDate.AddDays(-(int)lastLoginDate.DayOfWeek)).Days)
            {
                case 0:
                    return DateTimeCategory.ThisWeek;
                case 7:
                    return DateTimeCategory.LastWeek;
            }
            if (lastLoginDate.Year == todayDate.Year)
            {
                switch (todayDate.Month - lastLogin.Month)
                {
                    case 0:
                        return DateTimeCategory.ThisMonth;
                    case 1:
                        return DateTimeCategory.LastMonth;
                    default:
                        return DateTimeCategory.MorePast;
                }
            }
            return DateTimeCategory.MorePast;
        }
    }
}
