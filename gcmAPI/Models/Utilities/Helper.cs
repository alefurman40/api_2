using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gcmAPI.Models.Utilities
{
    public class Helper
    {

        #region Get_business_days_between_2_dates

        public int Get_business_days_between_2_dates(DateTime pu_date, DateTime del_date)
        {
            int days = 0;

            if (pu_date.Date > del_date.Date)
            {
                // It's a later date
                //return 0;
            }
            else if (pu_date.Date < del_date.Date)
            {
                // It's an earlier date
                //return 0;
                for (byte i = 0; i < 20; i++)
                {
                    if (pu_date == del_date)
                        break;

                    pu_date = pu_date.AddDays(1);
                    if (pu_date.DayOfWeek == DayOfWeek.Saturday || pu_date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        // Do nothing
                    }
                    else
                    {
                        days++;
                    }
                }
            }
            else
            {
                //It's an earlier or equal date
            }

            return days;
        }

        #endregion
    }
}