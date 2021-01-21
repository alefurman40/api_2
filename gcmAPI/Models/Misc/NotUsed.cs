using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gcmAPI.Models.Misc
{
    public class NotUsed
    {
        #region Start_join_LTL_threads_if_density_not_low

        //private void Start_join_LTL_threads_if_density_not_low(string mode)
        //{
        //    if (quoteData.linealFeet >= 20.0)
        //    {
        //        if (quoteData.totalWeight > 8800)
        //            if (mode == "start")
        //                Start_threads();
        //            else
        //                Join_threads();
        //    }
        //    else if (quoteData.linealFeet >= 19.0)
        //    {
        //        if (quoteData.totalWeight > 8200)
        //            if (mode == "start")
        //                Start_threads();
        //            else
        //                Join_threads();
        //    }
        //    else if (quoteData.linealFeet >= 18.0)
        //    {
        //        if (quoteData.totalWeight > 7700)
        //            if (mode == "start")
        //                Start_threads();
        //            else
        //                Join_threads();
        //    }
        //    else if (quoteData.linealFeet >= 17.0)
        //    {
        //        if (quoteData.totalWeight > 7200)
        //            if (mode == "start")
        //                Start_threads();
        //            else
        //                Join_threads();
        //    }
        //    else if (quoteData.linealFeet >= 16.0)
        //    {
        //        if (quoteData.totalWeight > 6600)
        //            if (mode == "start")
        //                Start_threads();
        //            else
        //                Join_threads();
        //    }
        //    else if (quoteData.linealFeet >= 15.0)
        //    {
        //        if (quoteData.totalWeight > 6100)
        //            if (mode == "start")
        //                Start_threads();
        //            else
        //                Join_threads();
        //    }
        //    else if (quoteData.linealFeet >= 14.0)
        //    {
        //        if (quoteData.totalWeight > 5600)
        //            if (mode == "start")
        //                Start_threads();
        //            else
        //                Join_threads();
        //    }
        //    else if (quoteData.linealFeet >= 13.0)
        //    {
        //        if (quoteData.totalWeight > 5100)
        //            if (mode == "start")
        //                Start_threads();
        //            else
        //                Join_threads();
        //    }
        //    else if (quoteData.linealFeet >= 12.0)
        //    {
        //        if (quoteData.totalWeight > 4700)
        //            if (mode == "start")
        //                Start_threads();
        //            else
        //                Join_threads();
        //    }
        //    else
        //    {
        //        if (mode == "start")
        //            Start_threads();
        //        else
        //            Join_threads();
        //    }
        //}

        #endregion


        #region UPS_PackageStartThreads

        //public void UPS_PackageStartThreads()
        //{
        //    oThreadUPSPackage_Ground.Start();
        //    oThreadUPSPackage_NextDayAir.Start();
        //    oThreadUPSPackage_SecondDayAir.Start();
        //    oThreadUPSPackage_3DaySelect.Start();
        //    oThreadUPSPackage_NextDayAirSaver.Start();
        //    oThreadUPSPackage_NextDayAirEarlyAM.Start();
        //    oThreadUPSPackage_2ndDayAirAM.Start();
        //}

        #endregion

        #region UPS_PackageJoinThreads

        //public void UPS_PackageJoinThreads()
        //{
        //    if (!oThreadUPSPackage_Ground.Join(TimeSpan.FromSeconds(12)))
        //    {
        //        oThreadUPSPackage_Ground.Abort();
        //    }
        //    if (!oThreadUPSPackage_NextDayAir.Join(TimeSpan.FromSeconds(12)))
        //    {
        //        oThreadUPSPackage_NextDayAir.Abort();
        //    }
        //    if (!oThreadUPSPackage_SecondDayAir.Join(TimeSpan.FromSeconds(12)))
        //    {
        //        oThreadUPSPackage_SecondDayAir.Abort();
        //    }
        //    if (!oThreadUPSPackage_3DaySelect.Join(TimeSpan.FromSeconds(12)))
        //    {
        //        oThreadUPSPackage_3DaySelect.Abort();
        //    }
        //    if (!oThreadUPSPackage_NextDayAirSaver.Join(TimeSpan.FromSeconds(12)))
        //    {
        //        oThreadUPSPackage_NextDayAirSaver.Abort();
        //    }
        //    if (!oThreadUPSPackage_NextDayAirEarlyAM.Join(TimeSpan.FromSeconds(12)))
        //    {
        //        oThreadUPSPackage_NextDayAirEarlyAM.Abort();
        //    }
        //    if (!oThreadUPSPackage_2ndDayAirAM.Join(TimeSpan.FromSeconds(12)))
        //    {
        //        oThreadUPSPackage_2ndDayAirAM.Abort();
        //    }
        //}

        #endregion


        //if (quoteData.isHazmat == false && quoteData.hasDimensions.Equals(true))
        //{

        //    //UPS_PackageStartThreads();

        //    #region Not used
        //    //oThreadUPSPackage_Ground.Start();
        //    //oThreadUPSPackage_NextDayAir.Start();
        //    //oThreadUPSPackage_SecondDayAir.Start();
        //    //oThreadUPSPackage_3DaySelect.Start();
        //    //oThreadUPSPackage_NextDayAirSaver.Start();
        //    //oThreadUPSPackage_NextDayAirEarlyAM.Start();
        //    //oThreadUPSPackage_2ndDayAirAM.Start();
        //    #endregion
        //}

        //if (isPuertoRicoDest == false) // Gives a wrong rate for PR
        //{
        //    oThreadNEMF.Start();
        //    ltl_threads.Add(oThreadNEMF);
        //}
    }
}
