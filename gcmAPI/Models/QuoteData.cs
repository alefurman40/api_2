using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gcmAPI.Models.LTL
{
    public class QuoteData
    {
        public double[] densities;
        public double totalDensity, totalWeight, totalCube, totalPieces, totalUnits, extraWeight, linealFeet,calculated_freight_class;
        public bool hasDimensions, hasAccessorials, isHazmat, showDLSRates, isDUR, isAssociationID_5, isUserVanguard, isCommodity, isCommodityLkupHHG, isHHG, isUSED,
            isHHG_AndUnder500, hasFreightClass, is_AAFES_quote, is_Genera_quote, is_dura_logic, is_like_NetNet;
        public string username, api_username, origZip, destZip, origCity, destCity, origState, destState, origCountry, destCountry; //, pickupDate
        public string subdomain, mode, txtCommodityCubicFeet, txtAAFES_Quote_PO;

        public string orig_zip_Canada_no_space, dest_zip_Canada_no_space;

        public int newLogId, numOfUnitsPieces;
        //public bool
        public DateTime puDate;
        public LTLPiece[] m_lPiece, m_lPiece_actual;
        public List<LTLPiece> m_lPieceList, m_lPiece_actual_List;
        public HelperFuncs.AccessorialsObj AccessorialsObj;

        //

        public bool is_good_USF_Holland_orig_state;
    }
}