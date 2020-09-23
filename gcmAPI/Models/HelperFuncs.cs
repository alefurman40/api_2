#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Linq;
using System.Xml;
using gcmAPI.Models.LTL;
using gcmAPI.Models.AAFES;
using gcmAPI.Models.Utilities;

#endregion

/// <summary>
/// Summary description for HelperFuncs Live Alex2015
/// </summary>
public static class HelperFuncs
{
    #region Variables

    public enum Subdomains { spc, mwi, rs, atlas, tyson, allmodes, mbg, clipper };

    public enum Modes { ws, aes, live };

    public struct AccessorialsObj
    {
        // Residential Pickup, Residential Delivery, Construnction Pickup, Construction Delivery, Inside Delivery,
        // Appointment Pickup, Appointment Delivery, Tradeshow Pickup, Tradeshow Delivery, Liftgate Pickup, 
        // Liftgate Delivery
        public bool RESPU, RESDEL, CONPU, CONDEL, INSDEL, APTPU, APTDEL, TRADEPU, TRADEDEL, LGPU, LGDEL,
            MILIPU,MILIDEL,GOVPU,GOVDEL;
    }

    public struct weightClass
    {
        public List<Double> weights;
        public List<string> fClasses;
    }

    //public struct QuoteData
    //{
    //    public double[] densities;
    //    public double totalDensity, totalWeight, totalCube, totalPieces, extraWeight;
    //    public bool hasDimensions, hasAccessorials, isHazmat, showDLSRates, isDUR, isAssociationID_5, isUserVanguard, isCommodity, isCommodityLkupHHG, isHHG, isUSED, 
    //        isHHG_AndUnder500, hasFreightClass;
    //    public string username, origZip, destZip, origCity, destCity, origState, destState, origCountry, destCountry; //, pickupDate
    //    public string subdomain, mode;
    //    public int newLogId, numOfUnitsPieces;
    //    //public bool
    //    public DateTime puDate;
    //    public LTLPiece[] m_lPiece;
    //    public List<LTLPiece> m_lPieceList;
    //    public HelperFuncs.AccessorialsObj AccessorialsObj;
    //}

    public struct DispatchInfo
    {
        public string username, ShipmentReadyDate, deliveryDay, txtDesc1, txtDesc2, txtDesc3, txtDesc4, txtPONumber,
            rateType, carrier, txtDAddress1, txtDAddress2, txtDName, txtDCompany, txtPAddress1, txtPName, txtPCompany;
        public string ddlRHour, ddlRMinute, ddlRAMPM, ddlCHour, ddlCMinute, ddlCAMPM;
        public DateTime delDate, puDate;
        public int ShipmentID;
        public bool isHazmat;
    }

    public struct SHIPMENT
    {
        #region Not used
        /*
      
      ,[CardAuth]
      ,[CopyGroup]    
      ,[LOASent]
      ,[MissedPU]
      ,[RegDate]
      ,[RequestDeliv]     
      ,[TimeStamp]
    
         */
        #endregion

        public int ShipmentID, CompID_CUST, AESSell;
        public string Initials, ItemNotes, ShipStatus;

    }

    public struct SEGMENT
    {
        #region Not used
        /*
       
      ,[ArrivePortID]
      ,[DepartPortID]
      ,[CompID_AGT]
      ,[CompID_BKR]
      ,[CompID_NOTIFY] 
      ,[MBLID]
      ,[]
      ,[]
      ,[CarrierALL]
      ,[]
      ,[]
      ,[]
      ,[Container]
      ,[ContainerNbr]
      ,[ContainerSize]
      ,[]
      ,[LastFreeDay]
      ,[]
      ,[DI]
      ,[]
      ,[]
      ,[]
      ,[DeclaredVal]
      ,[ImportEntryNbr]
      ,[ImportFreeTimeExp]
      ,[ImportHWBNbr]
      ,[Incoterms]
      ,[Instr1]
      ,[Instr2]
      ,[InsuredAmt]
      ,[ITNbr]
      ,[ITNNbr]
      ,[LFUsed]
      ,[]
      ,[SealNbr]
      ,[]
      ,[]
      ,[]
      ,[Service]
      ,[TransMode]
      ,[VesselVoyage]
      ,[TimeStamp]
      ,[]
      ,[]
      ,[MarksTxt]
      ,[DescTxt]
      ,[FinalPortID]
      ,[]
      ,[DO_text]
      ,[CompID_Freight_Location]
      ,[CompID_Delivery_Port]
      ,[MBLno]
      ,[HBLno]
      ,[placeofreceipt]
      ,[RPUDate]
      ,[]
      ,[]
         */
        #endregion

        public int ShipmentID, CompID_CAR, CompID_DELIV, CompID_PU, billing;
        public DateTime ATA, PUDate, DelivDate, DispatchDate, DispatchTime, ReadyTime, CloseTime;
        public string BookingNbr, CarrierQuoteNum, Comments, DispatchName, ProNbr, SegRmk1, SegRmk2, SegRmk3, SegStatus;
        public double CarrierQuoteAmt;
        public byte SegmentNbr;
    }

    public struct CollectionLocationDoc
    {
        public CookieCollection coll;
        public string doc;
        public string location;
    }
    ////---------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------
    //For UPS Package
    public struct upsPackageItem
    {
        public string weight, uom, packageType, length, width, height;
    }

    public struct upsPackageShipInfo
    {
        public string user, pass, accessLicenseNum, accountNum, shipperNum, serviceCode, shipDescription, shipChargeType,
            shipperAddressLine1, shipperCity, shipperState, shipperPostalCode, shipperCountryCode,
            shipperName, shipperAttentionName, shipperPhone,
            fromAddressLine1, fromCity, fromState, fromPostalCode, fromCountryCode, fromName, fromAttentionName,
            toAddressLine1, toCity, toState, toPostalCode, toCountryCode, toName, toAttentionName, toPhone;

        public upsPackageItem[] packageItems;
    }

    //public struct upsPackageResRow
    //{
    //    public string service;
    //    public double cost;
    //    public int days;
    //    public DateTime latestPickupDateTime, scheduleBy, deliveredBy;
    //}

    //--

    //For DLS
    public struct dlsItem
    {
        public string weight, fClass, length, width, height;
    }

    public struct dlsShipInfo
    {
        public string user, pass, accessLicenseNum, accountNum, shipperNum, serviceCode, shipDescription, shipChargeType,
            shipperAddressLine1, shipperCity, shipperState, shipperPostalCode, shipperCountryCode,
            shipperName, shipperAttentionName, shipperPhone,
            fromAddressLine1, fromCity, fromState, fromPostalCode, fromCountryCode, fromName, fromAttentionName, fromPhone,
            toAddressLine1, toCity, toState, toPostalCode, toCountryCode, toName, toAttentionName, toPhone;

        public dlsItem[] Items;
    }

    //--

    public struct ModalX_Result
    {
        public bool success;
        public DateTime pickupDate, deliveryDate;
        public decimal cost;
    }

    public struct PoSystemShipInfo
    {

        public string fCharges, spZip, ccZip, VendorNotes, ShipperNotes;
        public string spContactName, spContactPhone, spContactEmail, spName, spAddress1, spCity, spState, spCountry, ccName, ccAddress1, ccAddress3, ccCity,
            ccState, ccCountry, spAddress2, spAddress3, ccAddress2, ccContactPhone, shipByDate, shipAfterDate;

        public DateTime requestedDelDateStart, requestedDelDateEnd, requestedPuDateStart, requestedPuDateEnd, requestedDelTimeStart, requestedDelTimeEnd, requestedPuTimeStart, requestedPuTimeEnd;
        public List<string> customerPO, shipperRefNumbers;

        public decimal totalCube, totalWeight;
        public bool accept, hazMat, AcceptOrderAES, wasBooked;
        public string status;
        public int Pcs, PO_ID, QuoteID, ShipmentID, SegmentID, spGCMCompID, ccGCMCompID, weight;
        public byte IndexOfCarrierInQuote;
        public double freightClass;

        public string Ponumber, Routing, LatestMessage, DispatchedBy;

    }

    public struct CompInfo
    {
        public string Contact, Phone, Addr1, Addr2, CompName, City, State, Zip, EMail, Fax;
    }

    public struct upsPackageScraperInfo
    {
        public string origCity, origState, origZip, destCity, destState, destZip;
        public string user, pass, puDate;
        public string[] length, width, height, weight, packageType;
    }

    #endregion

    public static string connStringRater2009 = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aesrater_dataConnectionStringSS"].ConnectionString;

    #region SQL Helpers

    #region ExecuteNonQuery

    public static void ExecuteNonQuery(string ConnectionString, ref string sql, string sender)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {              
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception e)
        {
            writeToSiteErrors(string.Concat(sender, " ExecuteNonQuery"), e.ToString());
        }
    }

    #endregion

    #region ExecuteNonQuery_GetLastAutogeneratedID

    public static int ExecuteNonQuery_GetLastAutogeneratedID(string ConnectionString, ref string sql, string sender)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    command.ExecuteNonQuery();

                    //int intID;
                    //string strSQL;
                    //string strSQL = "select @@identity";
                    command.CommandText = "select @@identity";
                    //intID = Convert.ToInt32(command.ExecuteScalar());
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        catch (Exception e)
        {
            writeToSiteErrors(string.Concat(sender, " ExecuteNonQuery"), e.ToString());
            return -1;
        }
    }

    #endregion

    #region ExecuteScalar

    public static object ExecuteScalar(string ConnectionString, ref string sql, string sender)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    return(command.ExecuteScalar());
                }
            }
        }
        catch (Exception e)
        {
            writeToSiteErrors(string.Concat(sender, " ExecuteScalar"), e.ToString());
            return null;
        }
    }

    #endregion

    #region checkIfExists

    public static void checkIfExists(string connectionString, ref string sql, ref bool exists, string sender)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {             
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            exists = true;
                            //HelperFuncs.writeToSiteErrors("profileExists = true, sql=", sql);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("checkIfExists", e.ToString());
        }
    }

    #endregion

    #endregion

    #region isInvoicePNW

    public static bool isInvoicePNW(string Directory)
    {
        string sql = string.Concat("SELECT isPNW FROM DOCUMENTS WHERE Directory='", Directory, "'");
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAudit))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["isPNW"] != DBNull.Value && (bool)reader["isPNW"] == true)
                            {
                                return true;
                            }
                            //HelperFuncs.writeToSiteErrors("profileExists = true, sql=", sql);
                        }
                    }
                }
            }
            return false;
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("checkIfExists", e.ToString());
            return false;
        }
    }

    #endregion

    #region getSQL_ForSelectCarrierCompID

    #region getSQL_ForSelectCarrierCompID_ByCompName

    public static void getSQL_ForSelectCarrierCompID_ByCompName(ref string strSQL, string CarrierName)
    {
        if (CarrierName.Contains("YRC".ToLower()) && !CarrierName.Trim().Contains("max liability".ToLower())) // Without max liability
        {            
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "YRC");
        }
        else if (CarrierName.Contains("YRC".ToLower()) && CarrierName.Trim().Contains("max liability".ToLower())) // Without max liability
        {           
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "YRC - (LTD Max Liability $100)");
        }
        else if (CarrierName.Contains("SAIA".ToLower()) && !CarrierName.Trim().Contains("max liability".ToLower()))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Saia Motor Freight");
        }
        else if (CarrierName.Contains("SAIA".ToLower()) && CarrierName.Trim().ToLower().Contains("max liability".ToLower()))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "SAIA - (LTD Max Liability $100)");
        }
        else if (CarrierName.Contains("DYLT".ToLower()) || CarrierName.Contains("Daylight".ToLower()))
        {           
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "DAYLIGHT");
        }                                   
        else if (CarrierName.Contains("RDFS".ToLower()) || CarrierName.Contains("Roadrunner".ToLower()) ||
              CarrierName.Contains("Roadrunner Transportation".ToLower()) || CarrierName.Contains("Roadrunner SPC Pricing".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "ROADRUNNER TRANSPORTATION - RRTS");
        }  
        else if (CarrierName.Contains("Roadrunner Transportation Services2".ToLower()))
        {         
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "ROADRUNNER TRANSPORTATION SERVICES 2");
        }             
        else if (CarrierName.Contains("Oak Harbor".ToLower()) || CarrierName.Contains("Oak Harbor Freight Lines".ToLower()))
        {           
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "OAK HARBOR");
        }
        else if (CarrierName.Contains("Land Air".ToLower()))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "LAND AIR EXPRESS");
        }
        else if (CarrierName.Contains("Reddaway".ToLower()))
        {           
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "USF Reddaway");
        }
        else if (CarrierName.Contains("Holland".ToLower()))
        {         
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "USF Holland");
        }
        else if (CarrierName.Contains("Dayton Freight".ToLower()))
        {           
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "DAYTON FREIGHT LINES");
        }    
        else if (CarrierName.Contains("Estes Express".ToLower()) || CarrierName.Contains("Estes Express SPC Pricing".ToLower()) || CarrierName.Contains("Estes Express MWI Pricing".ToLower()) ||
            CarrierName.Contains("Estes Express Allmodes Pricing".ToLower()))
        {         
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "ESTES EXPRESS LINES");
        }     
        else if (CarrierName.Contains("A Duie Pyle".ToLower()))
        {           
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "A DUIE PYLE");
        }
        else if (CarrierName.Contains("AAA Cooper".ToLower()))
        {         
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "AAA COOPER");
        }  
        else if (CarrierName.Contains("Averitt Express".ToLower()))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Averitt Express");
        }
        else if (CarrierName.Contains("Benton Express".ToLower()))
        {           
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "BENTON EXPRESS");
        }            
        else if (CarrierName.Contains("Central Freight Lines".ToLower()))
        {           
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "CENTRAL FREIGHT LINES");
        }
        else if (CarrierName.Contains("Con-Way Freight".ToLower()))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Con-Way Transportation Services");
        }     
        else if (CarrierName.Contains("Dependable Highway Express".ToLower()) || CarrierName.Contains("DHE"))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "DEPENDABLE HIGHWAY EXPRESS");
        }
        else if (CarrierName.Contains("Dohrn Transfer Company".ToLower()))
        {         
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Dohrn Transfer Company");
        }      
        else if (CarrierName.Contains("Frontline Freight".ToLower()))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Frontline Freight Inc");
        }     
        else if (CarrierName.Contains("Lakeville Motor Express".ToLower()) || CarrierName.Contains("lakeville motor".ToLower()))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Lakeville Motor Express");
        }
        else if (CarrierName.Contains("Midwest Motor".ToLower()))
        {         
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "MIDWEST MOTOR EXPRESS");
        }    
        else if (CarrierName.Contains("New England Motor".ToLower()) || CarrierName.Contains("NEMF".ToLower()))
        {         
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "NEW ENGLAND MOTOR FREIGHT");
        }
        else if (CarrierName.Contains("New Penn".ToLower()))
        {           
            //getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "NEW PENN");
            getSQL_ForSelectCarrierCompID_ByCompID_Step_2(ref strSQL, "14162");
        } 
        else if (CarrierName.Contains("R+L".ToLower()) || CarrierName.Contains("R$L Carrier".ToLower()) || CarrierName.Contains("r $ l") ||
             CarrierName.Contains("R & L Carriers".ToLower()) || CarrierName.Contains("R&L".ToLower()))
        {           
            getSQL_ForSelectCarrierCompID_ByCompID_Step_2(ref strSQL, "158276");
        }     
        else if (CarrierName.Contains("Southeastern Freight Lines".ToLower()) || CarrierName.Contains("SEFL".ToLower()))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "SOUTHEASTERN FREIGHT LINES");
        }
        else if (CarrierName.Contains("Southwestern Motor".ToLower()))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "SOUTHWESTERN MOTOR TRANSPORT");
        }
        else if (CarrierName.Contains("Standard Forwarding Company".ToLower()))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Standard Forwarding Company");
        }    
        else if (CarrierName.Contains("UPS Freight".ToLower()))
        {        
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "UPS FREIGHT");
        }
        else if (CarrierName.Contains("Vitran Express".ToLower()))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "VITRAN");
        }
        else if (CarrierName.Contains("Ward Trucking".ToLower()))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "WARD TRUCKING");
        }
        else if (CarrierName.Contains("Wilson Trucking".ToLower()))
        {          
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "WILSON TRUCKING");
        }              
        else if (CarrierName.Contains("Pitt Ohio".ToLower()))
        {           
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "PITT OHIO EXPRESS");
        }
        else if (CarrierName.Contains("xpo"))
        {
            getSQL_ForSelectCarrierCompID_ByCompID_Step_2(ref strSQL, "98957");
        } 
        else if (CarrierName.Contains("Towne Air US".ToLower()))
        {         
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Towne Air Freight");
        }
        else if (CarrierName.Equals("Central Transport Pallet"))
        {         
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "CENTRAL TRANSPORT");
        }          
        else if (CarrierName.Contains("Frontline Freight".ToLower()))
        {         
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Frontline Freight Inc");
        }     
        else if (CarrierName.Contains("Old Dominion".ToLower()) || CarrierName.Contains("Old Dominion Freight Line".ToLower())) 
        {        
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Old Dominion Freight Line");
        }
        else if (CarrierName.Contains("FedEX National".ToLower()))
        {           
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "FED EX NATIONAL");
        }    
        else if (CarrierName.Contains("central transport"))
        {        
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "CENTRAL TRANSPORT");
        }     
        else if (CarrierName.Contains("us road"))
        {         
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "US ROAD");
        }
        else if (CarrierName.Contains("clear lane"))
        {        
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "CLEAR LANE FREIGHT SYSTEMS");
        }
        else if (CarrierName.Contains("UPS ".ToLower()))
        {         
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "UPS - SMALL PARCEL");
        }
        else if (CarrierName.Equals("Forward Air")) // Forward Air
        {        
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "FORWARD AIR INC");
        }  
        else if (CarrierName.Contains("ABF".ToLower())) // ABF
        {       
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "ABF Freight System, Inc.");
        }
        else if (CarrierName.Contains("Benjamin Best".ToLower())) // 
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Benjamin Best freight");
        }
        else
        {
            // RARE Carriers
            getSQL_ForSelectCarrierCompID_ByCompName_RARE_Carriers(ref strSQL, CarrierName);

            #region Not used
            // Test
            //string errorLoggingStr = string.Concat("CarrierName: ", CarrierName, " sql: ", strSQL);

            //HelperFuncs.writeToDispatchLogs("carNameTest not caught LIVE", errorLoggingStr);
            //HelperFuncs.MailUser("", errorLoggingStr, "", "", "", "dispatch logs carrier NOT caught LIVE", "");


            //strSQL = "select CompID from tbl_Company where Carrier = 1 AND CompName='" + CarrierName.Trim() + "'";
            #endregion
        }
    }

    #endregion

    #region getSQL_ForSelectCarrierCompID_ByCompName_RARE_Carriers

    private static void getSQL_ForSelectCarrierCompID_ByCompName_RARE_Carriers(ref string strSQL, string CarrierName)
    {
        if (CarrierName.Contains("KEY".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "KEY TRUCKING");
        }      
        else if (CarrierName.Contains("Pjax".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "PJAX FREIGHT");
        }       
        else if (CarrierName.Contains("Peninsula Truck".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "PENINSULA TRUCK LINES");
        }     
        else if (CarrierName.Contains("AMA Transportation".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "AMA EXPRESS");
        }      
        else if (CarrierName.Contains("Brandt Truck Line".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Brandt Truck Line");
        }
        else if (CarrierName.Contains("Central Arizona Freight".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Central Arizona Freight");
        }     
        else if (CarrierName.Contains("Dats Trucking".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "DATS TRUCKING");
        }    
        else if (CarrierName.Contains("Gold Coast Freightways".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Gold Coast Freightways");
        }
        else if (CarrierName.Contains("Horizon Freight Lines".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "HORIZON LINES");
        }      
        else if (CarrierName.Contains("Milan Express".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Milan Express");
        }
        else if (CarrierName.Contains("MTS Freight".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "MTS FREIGHT");
        }      
        else if (CarrierName.Contains("Price Truck Lines".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "PRICE TRUCKLINE");
        }    
        else if (CarrierName.Contains("RPM Transportation".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "RPM TRUCKING");
        }     
        else if (CarrierName.Contains("Sutton Transport".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Sutton Transport");
        }
        else if (CarrierName.Contains("Titan Freight".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "TITAN FREIGHT");
        }     
        else if (CarrierName.Contains("Crystal Motor Express".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Crystal Motor Express");
        }
        else if (CarrierName.Contains("Dugan Truck Line".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Dugan Truck Line");
        }
        else if (CarrierName.Contains("Dura Freight".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Dura Freight");
        }      
        else if (CarrierName.Contains("NYCE Trucking".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "NYCE Trucking");
        }
        else if (CarrierName.Contains("GCM Freight".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "ALG WORLDWIDE");
        }
        else if (CarrierName.Contains("TP Freight".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "TP FREIGHT");
        }     
        else if (CarrierName.Equals("Spee Dee Delivery"))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Spee Dee Delivery");
        }
        else if (CarrierName.Equals("Custom Freight Pallet"))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Custom Freight");
        }
        else if (CarrierName.Contains("Nots US".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "NOTS");
        }      
        else if (CarrierName.Contains("Nebraska Transport".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "Nebraska Transport Co");
        }    
        else if (CarrierName.Contains("mountain valley".ToLower()))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "MOUNTAIN VALLEY EXPRESS");
        }     
        else if (CarrierName.Contains("beaver express"))
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "BEAVER EXPRESS");
        }
        else if (CarrierName.Contains("GVKC")) // GVKC
        {
            getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref strSQL, "GVKC");
        }   
        else
        {
            // Test
            string errorLoggingStr = string.Concat("CarrierName: ", CarrierName, " sql: ", strSQL);

            HelperFuncs.writeToDispatchLogs("carNameTest not caught LIVE", errorLoggingStr);
            HelperFuncs.MailUser(AppCodeConstants.Alex_email, errorLoggingStr, "", "", "cs" + AppCodeConstants.email_domain, 
                "dispatch logs carrier NOT caught LIVE", "");

            strSQL = "select CompID from tbl_Company where Carrier = 1 AND CompName='" + CarrierName.Trim() + "'";
        }
    }

    #endregion

    #region getSQL_ForSelectCarrierCompID_ByCompName_Step_2
    
    private static void getSQL_ForSelectCarrierCompID_ByCompName_Step_2(ref string strSQL, string CompName)
    {
        strSQL = string.Concat("select CompID from tbl_Company where Carrier=1 AND CompName='", CompName, "'");
    }

    #endregion

    #region getSQL_ForSelectCarrierCompID_ByCompID_Step_2

    private static void getSQL_ForSelectCarrierCompID_ByCompID_Step_2(ref string strSQL, string CompID)
    {
        strSQL = string.Concat("select CompID from tbl_Company where Carrier=1 AND CompID=", CompID);
    }

    #endregion

    #endregion

    #region GetBOL_HTML

    public static void GetBOL_HTML(ref StringBuilder html, ref SharedLTL.BolParams BOL_Params)
    {


        #region Top part html

        html.Append(string.Concat("<!DOCTYPE html>", "<html>", "<head>"));

        GetStyle_Part(ref html);

        html.Append(string.Concat("</head>", "<body>"));

        #endregion

        GetBOL_CarrierToShipID_Part(ref html, ref BOL_Params);

        #region Top div

        // Open top div
        html.Append(string.Concat("<div style='border: 1px solid black;'>"));

        GetShipperAndConsigneeTH_Part(ref html);

        GetBOL_OrigAndDestComps_Part(ref html, ref BOL_Params);

        GetBOL_BillToAndAdditionalServices_Part(ref html, ref BOL_Params);

        GetPoToComments_Part(ref html, ref BOL_Params);

        GetBOL_Items_Part(ref html, ref BOL_Params);

        // Close top div
        html.Append(string.Concat("</div>"));

        #endregion

        GetBOL_Notes_Part(ref html, ref BOL_Params);

        // Open bottom table
        html.Append(string.Concat("<div class='table' >",
        // Open bottom row
        "<div class='table-row'>",

        #region Remit to per table cell

 "<div class='table-cell' style='width: 40%; border-top: 1px solid black; border-bottom: 1px solid black;'>"));

        GetBOL_FromRemitToPer_Part(ref html); // Table within table

        html.Append(string.Concat("</div>"));

        #endregion

        GetBOL_Section7ToSignatureOfConsignor_Part(ref html);

        #region COD paid by to Third Party bill to table cell

        html.Append(string.Concat("<div class='table-cell' style='width: 30%; border-top: 1px solid black; border-bottom: 1px solid black;'>"));

        GetBOL_FromCodPaidByToThirdPartyBillTo_Part(ref html); // Table within table

        html.Append(string.Concat("</div>",

        #endregion

        // Close bottom row
        "</div>",

        // Close bottom table
        "</div>"));

        GetBOL_RECEIVED_Part(ref html);

        GetBOL_ShipperAndCarrierCertify_Part(ref html);

        //GetBOL_PerToSingleShpt_Part(ref html);

        html.Append(string.Concat("</body>", "</html>"));

        HelperFuncs.writeToSiteErrors("GetBOL_HTML", html.ToString());
    }

    #endregion

    #region Functions to build dynamic HTML for BOL PDF

    #region GetBOL_CarrierToShipID_Part

    private static void GetBOL_CarrierToShipID_Part(ref StringBuilder html, ref SharedLTL.BolParams BOL_Params)
    {

        #region Carrier, Ship date, Shipment ID

        string ProNbr = string.Empty;

        if (BOL_Params.ProNbr != null)
        {
            ProNbr = BOL_Params.ProNbr;
        }

        // Carrier, Ship date, Shipment ID
        html.Append(string.Concat("<div class='table'>",
            "<div class='table-row'>",
              "<div class='table-cell' style='font-weight: bold;'>CARRIER</div>",
              "<div class='table-cell' style='font-weight: bold; padding-left: 10px;'>", BOL_Params.CarrierCompName, "</div>",
           "</div>",
           "<div class='table-row'>",
              "<div class='table-cell'></div>",
              "<div class='table-cell' style='font-size: 90%; padding-left: 10px;'>Phone:", BOL_Params.CarrierPhone, "</div>", // Add dynamic
           "</div>",
           "<div class='table-row'>",
              "<div class='table-cell'></div>",
              "<div class='table-cell' style='font-size: 90%; padding-left: 10px;'>Fax:", BOL_Params.CarrierFAX, "</div>", // Add dynamic
           "</div>",
            "<div class='table-row'>",
              "<div class='table-cell' style='font-weight: bold;'>SHIP DATE</div>",
              "<div class='table-cell' style='font-weight: bold; padding-left: 10px;'>", BOL_Params.PUDate, "</div>",
           "</div>",
            "<div class='table-row'>",
              "<div class='table-cell' style='font-weight: bold;'>Shipment ID</div>",
              "<div class='table-cell' style='font-weight: bold; padding-left: 10px;'>", BOL_Params.ShipmentID, "</div>",
           "</div>",
             "<div class='table-row'>",
               "<div class='table-cell'>Pro #</div>",
               "<div class='table-cell' style='font-weight: bold; padding-left: 10px;'>", ProNbr, "</div>",
            "</div>",
        "</div>"));

        #endregion
    }

    #endregion

    #region GetBOL_OrigAndDestComps_Part

    private static void GetBOL_OrigAndDestComps_Part(ref StringBuilder html, ref SharedLTL.BolParams BOL_Params)
    {

        #region Comp Name, address, city, phone, fax
        // Comp Name, address, city, phone, fax
        html.Append(string.Concat("<div class='table' style='width: 100%; font-size: 95%;'>",
           "<div class='table-row'>",
              "<div class='table-cell' style='width: 50%; font-weight: bold; padding-left: 3px;'>", BOL_Params.ShipperCompName, "</div>",
              "<div class='table-cell' style='width: 50%; font-weight: bold; border-left: 1px solid black; padding-left: 10px;'>", BOL_Params.ConsCompName, "</div>",
           "</div>",
           "<div class='table-row'>",
              "<div class='table-cell' style='width: 50%; padding-left: 3px;'>", BOL_Params.ShipperAddr1, "</div>",
              "<div class='table-cell' style='width: 50%; border-left: 1px solid black; padding-left: 10px;'>", BOL_Params.ConsAddr1, "</div>",
           "</div>",
            "<div class='table-row'>",
              "<div class='table-cell' style='width: 50%; padding-left: 3px;'>", BOL_Params.ShipperAddr2, "</div>",
              "<div class='table-cell' style='width: 50%; border-left: 1px solid black; padding-left: 10px;'>", BOL_Params.ConsAddr2, "</div>",
           "</div>",
            "<div class='table-row'>",
              "<div class='table-cell' style='width: 50%; padding-left: 3px;'>", BOL_Params.ShipperAddr3, "</div>",
              "<div class='table-cell' style='width: 50%; border-left: 1px solid black; padding-left: 10px;'>", BOL_Params.ConsAddr3, "</div>",
           "</div>",
            "<div class='table-row'>",
              "<div class='table-cell' style='width: 50%; font-weight: bold; padding-left: 3px;'>Phone&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", BOL_Params.ShipperPhone,
                "<span style='padding-left: 100px;'>Fax&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", BOL_Params.ShipperFAX, "</span></div>",
              "<div class='table-cell' style='width: 50%; font-weight: bold; border-left: 1px solid black; padding-left: 10px;'>Phone&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", BOL_Params.ConsPhone,
                "<span style='padding-left: 100px;'>Fax&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", BOL_Params.ConsFAX, "</span></div>",
           "</div>",
        "</div>"));

        #endregion
    }

    #endregion

    #region GetBOL_BillToAndAdditionalServices_Part

    private static void GetBOL_BillToAndAdditionalServices_Part(ref StringBuilder html, ref SharedLTL.BolParams BOL_Params)
    {

        #region Bill to, additional services
        // Bill to, additional services

        html.Append(string.Concat("<div class='table' style='width: 100%; border-top: 1px solid black; border-bottom: 1px solid black; font-size: 100%;'>",
    "<div class='table-row'>",

      "<div class='table-cell' style='width: 50%; font-weight: bold; padding-left: 3px;'>", BOL_Params.MsgBillToCaption, "</div>",//<input type='text' name='lname'>

      "<div class='table-cell' style='width: 50%; font-weight: bold; border-left: 1px solid black; padding-left: 10px;'>Additional Services</div>",
   "</div>",
   "<div class='table-row'>",

      "<div class='table-cell' style='width: 50%; font-size: 95%; padding-left: 3px;'>", BOL_Params.MsgBillTo, "</div>", //.Replace(",", "<br>")

      "<div class='table-cell' style='width: 50%; font-weight: bold; border-left: 1px solid black; padding-left: 10px;'>", BOL_Params.AdditionalServices, "</div>",
   "</div>",

//"<div class='table-row'>",
//     "<div class='table-cell' style='width: 50%; font-weight: bold; padding-left: 3px;'>Req. Delivery Date&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style='border: 1px solid black;'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></div>",
//    "<div class='table-cell' style='width: 50%; font-weight: bold; border-left: 1px solid black;'></div>",
//"</div>",

"</div>"));
        #endregion
    }

    #endregion

    #region GetBOL_Items_Part

    private static void GetBOL_Items_Part(ref StringBuilder html, ref SharedLTL.BolParams BOL_Params)
    {

        #region Items

        // Items
        html.Append(string.Concat("<div class='table' style='margin-bottom: 6px;'>",
            "<div class='table-row' >",
              "<div class='table-cell' style='width: 8%; font-weight: bold; text-decoration: underline; padding-left: 3px;'>Count</div>",
              "<div class='table-cell' style='width: 10%; font-weight: bold; text-decoration: underline;'>Kind</div>",
              "<div class='table-cell' style='width: 8%; font-weight: bold; text-decoration: underline;'>Units</div>",
              "<div class='table-cell' style='width: 8%; font-weight: bold; text-decoration: underline;'>HM</div>",
              "<div class='table-cell' style='width: 41%; font-weight: bold; text-decoration: underline;'>Description</div>",
              "<div class='table-cell' style='width: 8%; font-weight: bold; text-decoration: underline;'>Nmfc</div>",
              "<div class='table-cell' style='width: 7%; font-weight: bold; text-decoration: underline;'>Class</div>",
              "<div class='table-cell' style='width: 10%; font-weight: bold; text-decoration: underline;'>Weight</div>",
           "</div>"));

        string HR = string.Empty;

        for (byte i = 0; i < BOL_Params.bolItems.Count; i++)
        {
            if (BOL_Params.bolItems[i].HR.Equals("False"))
            {
                HR = string.Empty;
            }
            else
            {
                HR = BOL_Params.bolItems[i].HR;
            }
            html.Append(string.Concat("<div class='table-row'>",
                  "<div class='table-cell' style='width: 8%; padding-left: 3px;'>", BOL_Params.bolItems[i].Pcs, "</div>",
                  "<div class='table-cell' style='width: 10%;'>", BOL_Params.bolItems[i].Kind, "</div>",
                  "<div class='table-cell' style='width: 8%;'>", BOL_Params.bolItems[i].Units, "</div>",
                  "<div class='table-cell' style='width: 8%;'>", HR, "</div>",
                  "<div class='table-cell' style='width: 41%;'>", BOL_Params.bolItems[i].Descr.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;"), "</div>",
                  "<div class='table-cell' style='width: 8%;'>", BOL_Params.bolItems[i].Nmfc, "</div>",
                  "<div class='table-cell' style='width: 7%;'>", BOL_Params.bolItems[i].fClass, "</div>",
                  "<div class='table-cell' style='width: 10%;'>", BOL_Params.bolItems[i].WtLBS, "</div>",
               "</div>"));
        }

        #endregion
    }

    #endregion

    #region GetBOL_RECEIVED_Part

    private static void GetBOL_RECEIVED_Part(ref StringBuilder html)
    {

        #region RECEIVED, Subject to the classification paragraph

        html.Append(string.Concat("<div style='font-size: 84%; padding-top: 7px; padding-bottom: 7px; padding-left: 3px; padding-right: 3px;'>RECEIVED, Subject to the classification and transportation service contracts and /or tariffs in effect on the date of the issue of this Bill of Lading, the ",
                  "property described above, in apparent good order, except as noted (contents and condition of contents of packages unknown ), marked, consigned, and ",
                  "destined as shown above which said company (the word company being understood throughout this contract as meaning any person or corporation in ",
                  "possession of this property under the contract) agrees to carry to its usual place of delivery at said destination, if on its own railroad water line, highway ",
                  "route or routes, or within the territory of its highway operations, otherwise to deliver to another carrier on the route to said destination. It is mutually agreed, ",
                  "as to each carrier or all or any of said property over all or any portion of said route to destination, and as to each party at any time interested in all or any of ",
                  "said property, that every service to be performed hereunder shall be subject to all conditions not prohibited by law, whether printed or written herein ",
                  "contained, including the condition on back hereof, which are hereby agreed to by the shipper and accepted himself and his assigns. The carrier and also ",
                  "the Consignor agree to Section 7 (if signed) to be valid for all freight costs incurred by the transit of goods.",
                  "</div>"));

        #endregion
    }

    #endregion

    #region GetBOL_ShipperAndCarrierCertify_Part

    private static void GetBOL_ShipperAndCarrierCertify_Part(ref StringBuilder html)
    {

        html.Append(string.Concat("<div class='table' style='margin-left: auto; margin-right: auto;'>",

                        "<div class='table-row'>",
                           "<div class='table-cell-no-padding' style='width: 50%;'>"));

        #region ShipperAndCarrierCertify

        html.Append(string.Concat("<div class='table' style='width: 100%;'>",

                          "<div class='table-row'>",
                             "<div class='table-cell' style='width: 14%; border-top: 1px solid black; border-bottom: 1px solid black;'>SHIPPER</div>",
                             "<div class='table-cell' style='width: 36%; border: 1px solid black; padding-bottom: 25px;'>CARRIER</div>",

                          "</div>",
                "</div>"));

        html.Append(string.Concat(
           "<div class='table' style='width: 100%;'>",
                "<div class='table-row'>",
                  "<div class='table-cell' style='width: 14%;'>PER</div>",//border: 1px solid black;
                  "<div class='table-cell' style='width: 9%; border-left: 1px solid black; padding-bottom: 35px;'>DRIVER&nbsp;</div>", // padding-bottom: 5px; border-bottom: 1px solid black; 
                  "<div class='table-cell' style='width: 9%; border-left: 1px solid black;'>PCS</div>",
                  "<div class='table-cell' style='width: 9%; border-left: 1px solid black;'>DATE</div>",

                  "<div class='table-cell' style='width: 9%; border-left: 1px solid black; border-right: 1px solid black;'>",
                       "<div class='table'>", //style='text-align: center;'

                                    "<div class='table-row'>",
                                      "<div class='table-cell'>SINGLE SHPT</div>",
                                      "<div class='table-cell' style='font-weight: bold;'>(X)</div>", //padding-top: 5px; 
                                   "</div>",
                       "</div>",
                  "</div>",
              "</div>",
           "</div>"));

        #endregion

        html.Append(string.Concat("</div><div class='table-cell-no-padding' style='width: 50%; font-size: 90%; padding-left: 10px; padding-right: 3px;'>",
           "Shipper and carrier hereby certify that they agreed that freight is in good ",
                             "condition, unless otherwise stated on this Bill of Lading, and all packages ",
                             "are within the standards for movement of goods without being damaged. ",
                             "This is to certify that the above named materials are properly classified , ",
                             "described, packaged, marked, and labeled and are in proper condition for ",
                             "transportation according to the applicable regulations of the Department ",
                             "of Transportation.",
            "</div>"));
        html.Append(string.Concat("</div></div>")); // Close row
    }

    #endregion

    #region GetBOL_PerToSingleShpt_Part

    // Not used
    private static void GetBOL_PerToSingleShpt_Part(ref StringBuilder html)
    {

        #region PerToSingleShpt
        // PerToSingleShpt
        //html.Append(string.Concat(
        //    "<div class='table'>",
        //         "<div class='table-row'>",
        //           "<div class='table-cell' style='width: 14%;'>PER</div>",//border: 1px solid black;
        //           "<div class='table-cell' style='width: 9%; border-left: 1px solid black; padding-bottom: 40px;'>DRIVER&nbsp;</div>",
        //           "<div class='table-cell' style='width: 9%; border-left: 1px solid black;'>PCS</div>",
        //           "<div class='table-cell' style='width: 9%; border-left: 1px solid black;'>DATE</div>",

        //           "<div class='table-cell' style='width: 9%; border-left: 1px solid black; border-right: 1px solid black;'>",
        //                "<div class='table'>", //style='text-align: center;'

        //                             "<div class='table-row'>",
        //                               "<div class='table-cell'>SINGLE SHPT</div>",
        //                               "<div class='table-cell' style='padding-top: 5px; font-weight: bold;'>(X)</div>",
        //                            "</div>",
        //                "</div>",
        //           "</div>",
        //       "</div>",
        //    "</div>"));
        #endregion

    }

    #endregion

    #region GetBOL_Section7ToSignatureOfConsignor_Part

    private static void GetBOL_Section7ToSignatureOfConsignor_Part(ref StringBuilder html)
    {
        #region Section7ToSignatureOfConsignor
        // Section7ToSignatureOfConsignor
        html.Append(string.Concat("<div class='table-cell' style='width: 30%; border: 1px solid black;'><div style='text-align: center; font-weight: bold;'>SECTION 7</div>",
                       "<div style='font-size: 85%; padding-left: 3px;'>Subject to Section 7 of applicable bill of lading,",
                           "if this information is to be delivered to the ",
                           "consignee without recourse of the consignor ,",
                           "the consignor shall sign the following ",
                           "statement:</div>",

                           "<br>",

                           "<div style='font-weight: bold; font-size: 85%; padding-left: 3px;'>",
                               "\"The carrier shalt not make delivery of ",
                               "this shipment without payment of the ",
                               "freight and all other lawfull charges.\"",

                           "</div>",
                           "<div style='font-weight: bold; text-align: center; padding-top: 10px;'>",
                               "____________________________",
                           "</div>",
                           "<div style='font-weight: bold; text-align: center;'>",
                               "(Signature of Consignor)",
                           "</div>",
                      "</div>"));
        #endregion
    }

    #endregion

    #region GetBOL_FromCodPaidByToThirdPartyBillTo_Part

    private static void GetBOL_FromCodPaidByToThirdPartyBillTo_Part(ref StringBuilder html)
    {
        #region Table within table

        html.Append(string.Concat("<div style='font-weight: bold; text-align: center;'>C.O.D FEE TO BE PAID BY",
                         "</div>",
                         "<br>",
                          "<div class='table' style='font-weight: bold; text-align: center;'>",

                             "<div class='table-row'>",
                               "<div class='table-cell' style='font-weight: bold; font-size: 85%; text-align: left; padding-right: 10px;'>CONSIGNEE</div>",
                               "<div class='table-cell' style='width: 35%; border: 1px solid black;'>&nbsp;&nbsp;&nbsp;</div>",
                            "</div>",

                          "</div>",

                           "<br>",

                            "<div class='table' style='font-weight: bold; text-align: center;'>",

                                "<div class='table-row'>",
                                   "<div class='table-cell' style='font-weight: bold; font-size: 85%; text-align: left; padding-right: 10px;'>SHIPPER&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div>",
                                   "<div class='table-cell' style='width: 35%; border: 1px solid black;'>&nbsp;&nbsp;&nbsp;</div>",
                                "</div>",
                            "</div>",

                          "<div style='border-bottom: 1px solid black; padding-top: 3px;'>",
                          "</div>",

                          "<br>",

                            "<div class='table' style='font-weight: bold; text-align: center;'>",

                                 "<div class='table-row'>",
                                   "<div class='table-cell' style='font-weight: bold; text-align: left;'>TOTAL<br>CHARGES $&nbsp;&nbsp;</div>",
                                   "<div class='table-cell' style='width: 35%; border: 1px solid black;'>&nbsp;&nbsp;&nbsp;</div>",
                                "</div>",
                             "</div>",
                           "<br>",
                          "<div style='border-top: 1px solid black; font-size: 85%; padding-left: 3px;'>IF CHARGES ARE TO BE ",
                             "PREPAID WRITE OR STAMP TO ",
                             "BE PREPAID",
                          "</div>",
                          "<br>",
                          "<div style='font-weight: bold; text-align: center;'>Third Party Bill-to",
                          "</div>"));

        #endregion
    }

    #endregion

    #region GetBOL_FromRemitToPer_Part

    private static void GetBOL_FromRemitToPer_Part(ref StringBuilder html)
    {

        #region Table within table
        html.Append(string.Concat("<div class='table'>",
                                          "<div class='table-row'>",
                                            "<div class='table-cell' style='font-weight: bold; padding-right: 10px; padding-left: 3px;'>REMIT<br>C.O.D.<br>TO</div>",
                                            "<div class='table-cell' style='width: 80%; font-weight: bold;'>If consignee's personal or company check is acceptable for C.O.D, please note:</div>",
                                         "</div>",
                                         "<div class='table-row'>",
                                            "<div class='table-cell'></div>",
                                            "<div class='table-cell' style='width: 80%; font-weight: bold; padding-bottom: 10px;'>C.O.D. AMT $______________</div>",
                                         "</div>",

                                      "</div>",

                                      "<br>",

                                      "<div class='table' style='border-top: 1px solid black;'>",
                                           "<div class='table-row'>",
                                               "<div class='table-cell' style='width: 100%; font-size: 90%; padding-left: 3px; padding-right: 3px; padding-top: 10px;'>",
                                                   "NOTE: Where the rate is dependent on value. Shippers are",
                                                   "required to state specifically in writing the agreed or declared",
                                                   "value of the property. The agreed or declared value of the",
                                                   "property is hereby specifically stated by the shipper to be not",
                                                   "exceeding",
                                               "</div>",
                                               "<div class='table-cell'></div>",
                                           "</div>",
                                       "</div>",

                                       "<div class='table'>",
                                           "<div class='table-row'>",
                                               "<div class='table-cell' style='width: 100%; font-size: 90%; text-align: center; padding-top: 10px; font-weight: bold;'>",
                                                   "PER",
                                               "</div>",
                                               "<div class='table-cell'></div>",
                                           "</div>",
                                       "</div>"));

        #endregion
    }

    #endregion

    #region GetBOL_Notes_Part

    private static void GetBOL_Notes_Part(ref StringBuilder html, ref SharedLTL.BolParams BOL_Params)
    {

        #region Notes

        // Notes
        html.Append("<div class='table'");

        if (BOL_Params.isSaia.Equals(false)) // Only add this if carrier is not Saia
        {
            html.Append(" style='border-bottom: 1px solid black;");
        }
        //else
        //{
        //    html.Append(" style='margin-top: 10px;");
        //}

        html.Append(string.Concat("'>",
         "<div class='table-row'>",
              "<div class='table-cell' style='width: 100%; text-align: center;'>", BOL_Params.ItemNotes.Replace(" < ", "&lt;").Replace(" > ", " &gt; ").Replace("&", " &amp; "), "</div>",
         "</div>",
        "</div>"));

        if (BOL_Params.isSaia.Equals(false)) // Only add this if carrier is not Saia
        {
            html.Append(string.Concat("<div style='width: 100%; font-weight: bold; text-align: center; font-size: 88%;'>",
             "IF ADDITIONAL SERVICES ARE REQUESTED BY CONSIGNEE THAT ARE NOT LISTED ON THE ORIGINAL BILL OF LADING<br>THE PARTY REQUESTING THESE SERVICES ARE RESPONSIBLE FOR THE CHARGES",
             "</div>"));
        }

        #endregion
    }

    #endregion

    #region GetShipperAndConsigneeTH_Part

    private static void GetShipperAndConsigneeTH_Part(ref StringBuilder html)
    {

        #region SHIPPER, CONSIGNEE
        // SHIPPER, CONSIGNEE

        html.Append(string.Concat("<div class='table' style='width: 100%; border-bottom: 1px solid black;'>",

        "<div class='table-row'>",
          "<div class='table-cell' style='font-weight: bold; text-align: center;'>SHIPPER</div>",
          "<div class='table-cell' style='font-weight: bold; text-align: center;'>CONSIGNEE</div>",
       "</div>",

    "</div>"));


        #endregion
    }

    #endregion

    #region GetPoToComments_Part

    private static void GetPoToComments_Part(ref StringBuilder html, ref SharedLTL.BolParams BOL_Params)
    {
        //HelperFuncs.writeToSiteErrors("BOL_Params.PONumber", BOL_Params.PONumber);
        //HelperFuncs.writeToSiteErrors("BOL_Params.PONumber after replace",
        //    BOL_Params.PONumber.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;").Replace(" ", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"));
        
        
        #region P.O. Ref #, Comments

        // P.O. Ref #, Comments
        html.Append(string.Concat("<div class='table'>",
"<div class='table-row'>",
  "<div class='table-cell' style='padding-right: 10px; padding-left: 3px;'>P.O. Ref #</div>",
  "<div class='table-cell' style='width: 80%;'>", BOL_Params.PONumber.Replace(" < ", "&lt;").Replace(" > ", "&gt;").Replace("&", "&amp;").Replace(" ", "&nbsp;").Replace(",", "&nbsp;").Replace("addATabSpace", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"), "</div>",
"</div>",
"<div class='table-row'>",
  "<div class='table-cell' style='padding-right: 10px; padding-left: 3px;'>Comments</div>", // Not used
  "<div class='table-cell' style='width: 80%; border: 1px solid black;'></div>",
"</div>",
"</div><br>"));

        #endregion
    }

    #endregion

    #region GetStyle_Part

    private static void GetStyle_Part(ref StringBuilder html)
    {
        #region Style

        html.Append(string.Concat("<style>",
                       ".table",
       "{",
          "display:table;",
       "}",

       ".table-row",
       "{",
          "display:table-row;",
       "}",

        ".table-cell",
        "{",
           "display:table-cell;",
           "font-family: \"Times New Roman\", Georgia, Serif;",
           "font-size: 93%;",
           "padding-top: 2px;",
           "padding-bottom: 2px;",
        "}",

         ".table-cell-no-padding",
        "{",
           "display:table-cell;",
           "font-family: \"Times New Roman\", Georgia, Serif;",
           "font-size: 93%;",
        "}",

        ".div",
        "{",
            "font-family: \"Times New Roman\", Georgia, Serif;",
        "}",

       /*
        ".table-cell",
       "{",
          "display:table-cell;",
          "font-family: Arial, Helvetica, sans-serif;",
          "font-size: 93%;",
          "padding-top: 2px;",
          "padding-bottom: 2px;",
       "}",

        ".table-cell-no-padding",
       "{",
          "display:table-cell;",
          "font-family: Arial, Helvetica, sans-serif;",
          "font-size: 93%;",
       "}",

       ".div",
       "{",
           "font-family: Arial, Helvetica, sans-serif;",
       "}",
       */

       "</style>"));


        #endregion
    }

    #endregion

    #endregion

    #region PdfSharpConvert

    public static Byte[] PdfSharpConvert(String html, ref string reportPath)
    {
        Byte[] res = null;
        //using (MemoryStream ms = new MemoryStream())
        //{
        //    var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
        //    pdf.Save(ms);
        //    res = ms.ToArray();

        //    //System.IO.File.WriteAllBytes("hello.pdf", res);
        //    System.IO.File.WriteAllBytes(reportPath, res);
        //}
        return res;
    }

    #endregion

    #region SetReportParameters

    #region SetReportParameters

    public static void SetReportParameters(ref SharedLTL.BolParams BOL_Params, ref LTLResult lResult, ref SqlCommand comm, ref SqlDataAdapter da, ref int intSegmentID, ref int intShipmentID,
        ref int intCustCompanyID, ref string destinationState, bool _IsStoreFrontCarrierBooking, StoreFrontAddress _SFAddress, bool isClearViewPO,
        ref List<string> combinedPOsList, ref string poNumber)
    {
        //////////////Set Parameter for Bill To Message////////////

        object objMsgBillTo;
        string strMsgBillTo = "";
        string strFirstPart = "", strSecondpart = "";
        string strSQL;
        object objTemp;
        bool isNemfInboundCollectBillTo = false;

        if (lResult.CarrierDisplayName.Trim().ToLower().Equals("Roadrunner Transportation Services2".ToLower()))
            comm.CommandText = "select MsgBillTo from AESData.dbo.tbl_LKUP_Docs where RptID=49";
        else if (lResult.CarrierKey.Trim().ToLower().Equals("UPS".ToLower()))
            comm.CommandText = "select MsgBillTo from AESData.dbo.tbl_LKUP_Docs where RptID=52";
        else if (lResult.CarrierDisplayName.Trim().ToLower().Contains("max liability"))
            comm.CommandText = "select MsgBillTo from AESData.dbo.tbl_LKUP_Docs where RptID=48";
        else if (lResult.CarrierKey.Trim().Equals("ALG Worldwide"))
            comm.CommandText = "select MsgBillTo from tbl_LKUP_Docs where RptID=47";
        else if (lResult.CarrierKey.Trim().ToLower().Contains("estesicat"))
            comm.CommandText = "select MsgBillTo from tbl_LKUP_Docs where RptID=62";
        else if (lResult.CarrierKey != null && lResult.CarrierKey.Trim().Length > 0)
        {
            if (lResult.CarrierKey.Trim().Equals("MFW") && !lResult.CarrierDisplayName.Trim().ToLower().Contains("max liability"))
                comm.CommandText = "select MsgBillTo from AESData.dbo.tbl_LKUP_Docs where RptID=47";
            else
                comm.CommandText = "select MsgBillTo from AESData.dbo.tbl_LKUP_Docs where RptID=46";
        }
        //
        else
            comm.CommandText = "select MsgBillTo from AESData.dbo.tbl_LKUP_Docs where RptID=46";

        if (lResult.CarrierKey.Trim().EndsWith(" GT"))
        {
            comm.CommandText = "select MsgBillTo from AESData.dbo.tbl_LKUP_Docs where RptID=55";
        }
        if (lResult.CarrierKey.Trim().EndsWith(" BG"))
        {
            comm.CommandText = "select MsgBillTo from AESData.dbo.tbl_LKUP_Docs where RptID=59";
        }
        if (lResult.CarrierKey.Trim().EndsWith(" US"))
        {
            //This is a unishippers shipment, so set the carrier quote number to "UNISHIPPERS"
            comm.CommandText = "UPDATE tbl_SEGMENTS " +
                "SET CarrierQuoteNum = 'UNISHIPPERS' " +
                "WHERE SegmentID = " + intSegmentID + " ";

            comm.ExecuteNonQuery();

            comm.CommandText = "select MsgBillTo from AESData.dbo.tbl_LKUP_Docs where RptID=63";
        }

        objMsgBillTo = comm.ExecuteScalar();
        if (objMsgBillTo != null)
        {
            strMsgBillTo = objMsgBillTo.ToString();
        }

        writeToSiteErrors("comm.CommandText", "comm.CommandText: " + comm.CommandText);
        writeToSiteErrors("strMsgBillTo", "msgbill: " + strMsgBillTo);

        //------------------------------------------------------------------------------------------
        //try
        //{
        //    writeToSiteErrors("catchCarrierKey1", "carKey: " + lResult.CarrierKey.Trim() + " msgBillTo: " + strMsgBillTo);
        //}
        //catch { }
        //------------------------------------------------------------------------------------------

        if (strMsgBillTo.Contains("\r\n"))
        {
            int intIndex;
            intIndex = strMsgBillTo.IndexOf("\r\n");
            strSQL = "select comp.BillingLabel from AESData.dbo.tbl_Company_CAR comp"
                + " inner join tbl_SEGMENTS seg on seg.CompID_CAR=comp.CompID_CARRIER where seg.ShipmentID=" + intShipmentID + " and CompID_CUST=" + intCustCompanyID;

            //writeToSiteErrors("black", "sql: " + strSQL);
            comm.CommandText = strSQL;

            string strBillingLabel = "";
            //objTemp = comm.ExecuteScalar();
            //
            //if (objTemp != null)
            //    strBillingLabel = objTemp.ToString();

            if (lResult.CarrierKey.Trim().Equals("RRTS"))
            {
                strBillingLabel = "Acct # 1050131";
            }
            else if (lResult.CarrierKey.Trim().Equals("SAIA"))
            {
                strBillingLabel = "Acct # 0861567";
            }

            //writeToSiteErrors("black", "carKey " + lResult.CarrierKey.Trim() + " displayName: " + lResult.CarrierDisplayName.Trim());

            ////////////
            if (lResult.CarrierDisplayName.Trim().ToLower().Contains(" spc "))
            {
                strBillingLabel = "";
            }
            ///////////
            if (lResult.CarrierKey.Trim().Equals("YRCSPC") || lResult.CarrierKey.Trim().Equals("Pitt OhioSPC"))
            {
                strBillingLabel = "Sierra Pacific Crafts";
            }
            else if (lResult.CarrierKey.Trim().Equals("USFREDSPCPP") || lResult.CarrierKey.Trim().Equals("USFREDSPC"))
            {
                strBillingLabel = "Sierra Pacific Crafts";
            }
            else if (lResult.CarrierKey.Trim().Equals("USFHOLSPCPP") || lResult.CarrierKey.Trim().Equals("USFHOLSPC"))
            {
                strBillingLabel = "Sierra Pacific Crafts";
            }
            else if (lResult.CarrierKey.Trim().Equals("SAIASPC"))
            {
                strBillingLabel = "Sierra Pacific Crafts";
            }
            else if (lResult.CarrierKey.Trim().Equals("RRTSSPC"))
            {
                //strBillingLabel = "Sierra Pacific Crafts";
            }
            else if (lResult.CarrierKey.Trim().Equals("EstesClickship"))
            {
                strBillingLabel = "ClickShipNGo Acct 5029048";
            }
            else if (lResult.CarrierKey.Trim().Equals("EstesOnline"))
            {
                strBillingLabel = "Online Stores acct# 5013974";
            }
            else if (lResult.CarrierKey.Trim().Equals("Central Freight Lines"))
            {
                strBillingLabel = "acct # 4399759001";
            }
            else if (lResult.CarrierKey.Trim().Equals("NEMF"))
            {
                string[] nemfDStatesForBillTo = "ME,NH,VT,MA,DE,MD,VA,OH,PA,CT,NY,RI,NJ".Split(',');
                try
                {
                    for (int i = 0; i < nemfDStatesForBillTo.Length; i++)
                    {
                        if (nemfDStatesForBillTo[i] == destinationState)
                        {
                            isNemfInboundCollectBillTo = true;
                            break;
                        }
                    }
                }
                catch (Exception exNemf)
                {
                    writeToSiteErrors("nemf dest states bill to problem", exNemf.ToString());
                }
            }

            //writeToSiteErrors("black", "billLabel " + strBillingLabel);
            //writeToSiteErrors("black", "carrierKey " + lResult.CarrierKey.Trim());

            strFirstPart = strMsgBillTo.Substring(0, intIndex + 2);
            strSecondpart = strMsgBillTo.Substring(intIndex + 2);

            if (lResult.CarrierDisplayName.EndsWith(" RRD"))
            {
                //strSecondpart = string.Concat("\r\nDLS WORLDWIDE ", "\r\n",
                //                              "1000 WINDHAM PARKWAY ", "\r\n",
                //                              "BOLINGBROOK, IL 60490");

                strSecondpart = string.Concat("DLS WORLDWIDE ", "<br>",
                                             "1000 WINDHAM PARKWAY ", "<br>",
                                             "BOLINGBROOK, IL 60490");

                HelperFuncs.writeToSiteErrors("ends with RRD", lResult.CarrierDisplayName + " " + strSecondpart);
            }
            else
            {
                strSecondpart = strSecondpart.Replace("\r\n", "<br>");
                HelperFuncs.writeToSiteErrors("does not end with RRD", lResult.CarrierDisplayName + " " + strSecondpart);
            }

            //13756
            strMsgBillTo = strBillingLabel + strSecondpart;

            //writeToSiteErrors("black", "msgBillto " + strMsgBillTo);
        }

        //------------------------------------------------------------------------------------------
        //try
        //{
        //    writeToSiteErrors("catchCarrierKey2", "carKey: " + lResult.CarrierKey.Trim() + " msgBillTo: " + strMsgBillTo);
        //}
        //catch { }
        //------------------------------------------------------------------------------------------


        //rdBOL.SetParameterValue("pmMsgBillToCaption", strFirstPart);
        if (isNemfInboundCollectBillTo == true)
        {
            //rdBOL.SetParameterValue("pmMsgBillToCaption", "Bill third party inbound collect to:");
            BOL_Params.MsgBillToCaption = "Bill third party inbound collect to:";
        }
        else
        {
            //rdBOL.SetParameterValue("pmMsgBillToCaption", strFirstPart);
            BOL_Params.MsgBillToCaption = strFirstPart;
        }


        if (_IsStoreFrontCarrierBooking && _SFAddress != null)
        {
            strMsgBillTo = "";
            strMsgBillTo += _SFAddress.Company + "\r\n";
            strMsgBillTo += _SFAddress.Address1 + "\r\n";
            if (!string.IsNullOrEmpty(_SFAddress.Address2))
            {
                strMsgBillTo += _SFAddress.Address2 + "\r\n";
            }
            strMsgBillTo += _SFAddress.City + ", " + _SFAddress.State + " " + _SFAddress.Zip;
        }

        //rdBOL.SetParameterValue("pmMsgBillTo", strMsgBillTo);
        BOL_Params.MsgBillTo = strMsgBillTo;

        //////////////Set Parameter for Item Notes////////////
        strSQL = "select ItemNotes from tbl_SHIPMENTS where ShipmentID=" + intShipmentID;

        comm.CommandText = strSQL;
        objTemp = comm.ExecuteScalar();

        string strItemNotes = "";

        if (objTemp != null)
        {
            strItemNotes = objTemp.ToString();
        }

        //rdBOL.SetParameterValue("pmItemNotes", strItemNotes);
        BOL_Params.ItemNotes = strItemNotes;

        //////////////Set Parameter for Pickup Date////////////
        strSQL = "select PUDate from tbl_SEGMENTS where SegmentID=" + intSegmentID;

        comm.CommandText = strSQL;
        objTemp = comm.ExecuteScalar();

        string strPUDate = "";

        if (objTemp != null)
        {
            strPUDate = Convert.ToDateTime(objTemp).ToString(@"MM/dd/yyyy");
        }

        //rdBOL.SetParameterValue("pmPUDate", strPUDate);
        BOL_Params.PUDate = strPUDate;

        #region Not used
        //////////////////////////////////////////////////////////

        //////////////Set Parameter for Pickup Date////////////
        //strSQL = "select PONumber from tbl_PO where ShipmentID=" + intShipmentID;

        //comm.CommandText = strSQL;
        //objTemp = comm.ExecuteScalar();

        //string strPONumber = "";

        //if (objTemp != null)
        //    strPONumber = objTemp.ToString();
        #endregion

        string strAllPONumbers = "";

        if (isClearViewPO.Equals(true))
        {
            //bool foundOldPO = false;
            string poSysPOs = "";
            foreach (string strPONo in combinedPOsList)
            {
                poSysPOs += string.Concat(strPONo, ",");
            }
            //rdBOL.SetParameterValue("pmPONumber", poSysPOs.TrimEnd(','));
            BOL_Params.PONumber = poSysPOs.TrimEnd(',');

            HelperFuncs.writeToSiteErrors("clearview pmPONumber for pdf", poSysPOs.TrimEnd(','));
        }
        else
        {
            foreach (string strPO in poNumber.Trim().Split(','))
            {
                if (strAllPONumbers.Trim().Length <= 0)
                    strAllPONumbers = strPO.Trim();
                else
                    strAllPONumbers += ", " + strPO.Trim();
            }
            //rdBOL.SetParameterValue("pmPONumber", strAllPONumbers);
            BOL_Params.PONumber = strAllPONumbers;

            HelperFuncs.writeToSiteErrors("not clearview pmPONumber for pdf", strAllPONumbers);
        }


        //////////////Set Parameter for Pickup Date////////////
        //rdBOL.SetParameterValue("pmShipmentID", intShipmentID);
        BOL_Params.ShipmentID = intShipmentID.ToString();

        //////////////Set Parameter for Pickup Date////////////
        DataTable dtTemp;
        dtTemp = GetAdditionalServices(ref intSegmentID, ref comm, ref da);
        string strAddionalServices = "";
        foreach (DataRow drRow in dtTemp.Rows)
        {
            if (strAddionalServices.Trim().Length <= 0)
            {
                if (drRow["LkUpValue"].ToString().ToUpper() != "INSURANCE")
                {
                    strAddionalServices = drRow["LkUpValue"].ToString();
                }
            }
            else
            {
                if (drRow["LkUpValue"].ToString().ToUpper() != "INSURANCE")
                {
                    strAddionalServices += ", " + drRow["LkUpValue"];
                }
            }

        }
        //rdBOL.SetParameterValue("pmAdditionalServices", strAddionalServices);
        BOL_Params.AdditionalServices = strAddionalServices;

        #region Not used
        //////////////Set Parameter for Bill To Message////////////

        /*
        comm.CommandText = "select MsgPallet from AESData.dbo.tbl_LKUP_Docs";
        objTemp = comm.ExecuteScalar();

        string strMsgPallet = "";

        if (objTemp != null)
        {
            strMsgPallet = objTemp.ToString();
        }

        rdBOL.SetParameterValue("pmMsgPallet", strMsgPallet);

        */

        //////////////Set Parameter for Segment Comments////////////
        //comm.CommandText = "select Comments from tbl_SEGMENTS where SegmentID=" + intSegmentID;
        //objTemp = comm.ExecuteScalar();
        #endregion

        string strSegmentComments = "";

        //if (objTemp != null)
        //  strSegmentComments = objTemp.ToString();

        //rdBOL.SetParameterValue("pmSegmentComments", strSegmentComments);
        BOL_Params.SegmentComments = strSegmentComments;
    }

    #endregion

    #region SetShipperRptParameters

    public static void SetShipperRptParameters(ref SharedLTL.BolParams BOL_Params, int segId, ref int intShipmentID)
    {
        try
        {
            string sql = string.Concat(
                "select comp.CompName,comp.Addr1,comp.Addr2,(City + ', ' + State + ' ' + Zip) as Addr3,comp.Phone,comp.FAX from tbl_COMPANY comp",
                " inner join tbl_SEGMENTS seg on seg.CompID_PU=comp.CompID where seg.ShipmentID=", intShipmentID,
                " and seg.SegmentID=", segId);
            //bool found = false;
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = sql;
                    conn.Open();
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //found = true;
                            if (reader["CompName"] != null && reader["CompName"] != DBNull.Value)
                            {
                                //HelperFuncs.writeToSiteErrors("pmShipperCompName", reader["CompName"].ToString());
                                //rdBOL.SetParameterValue("pmShipperCompName", reader["CompName"].ToString());
                                BOL_Params.ShipperCompName = reader["CompName"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmShipperCompName", string.Empty);
                                BOL_Params.ShipperCompName = string.Empty;
                            }

                            if (reader["Addr1"] != null && reader["Addr1"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue("pmShipperAddr1", reader["Addr1"].ToString());
                                BOL_Params.ShipperAddr1 = reader["Addr1"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmShipperAddr1", string.Empty);
                                BOL_Params.ShipperAddr1 = string.Empty;
                            }

                            if (reader["Addr2"] != null && reader["Addr2"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue("pmShipperAddr2", reader["Addr2"].ToString());
                                BOL_Params.ShipperAddr2 = reader["Addr2"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmShipperAddr2", string.Empty);
                                BOL_Params.ShipperAddr2 = string.Empty;
                            }

                            if (reader["Addr3"] != null && reader["Addr3"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue("pmShipperAddr3", reader["Addr3"].ToString());
                                BOL_Params.ShipperAddr3 = reader["Addr3"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmShipperAddr3", string.Empty);
                                BOL_Params.ShipperAddr3 = string.Empty;
                            }

                            if (reader["Phone"] != null && reader["Phone"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue("pmShipperPhone", reader["Phone"].ToString());
                                BOL_Params.ShipperPhone = reader["Phone"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmShipperPhone", string.Empty);
                                BOL_Params.ShipperPhone = string.Empty;
                            }

                            if (reader["FAX"] != null && reader["FAX"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue("pmShipperFAX", reader["FAX"].ToString());
                                BOL_Params.ShipperFAX = reader["FAX"].ToString();
                            }
                            else
                            {
                                BOL_Params.ShipperFAX = string.Empty;
                            }
                        }
                    }
                }
            }
            //if (!found)
            //{
            //    rdBOL.SetParameterValue("pmShipperCompName", string.Empty);
            //    rdBOL.SetParameterValue("pmShipperAddr1", string.Empty);
            //    rdBOL.SetParameterValue("pmShipperAddr2", string.Empty);
            //    rdBOL.SetParameterValue("pmShipperAddr3", string.Empty);
            //    rdBOL.SetParameterValue("pmShipperPhone", string.Empty);
            //    rdBOL.SetParameterValue("pmShipperFAX", string.Empty);
            //}
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("SetShipperRptParameters", e.ToString());
        }
    }

    #endregion

    #region SetConsigneeRptParameters

    public static void SetConsigneeRptParameters(ref SharedLTL.BolParams BOL_Params, int segId, ref int intShipmentID)
    {
        try
        {

            string sql = string.Concat(
                "select comp.CompName,comp.Addr1,comp.Addr2,(City + ', ' + State + ' ' + Zip) as Addr3,comp.Phone,comp.FAX from tbl_COMPANY comp",
                " inner join tbl_SEGMENTS seg on seg.CompID_DELIV=comp.CompID where seg.ShipmentID=", intShipmentID,
                " and seg.SegmentID=", segId);
            bool found = false;
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = sql;
                    conn.Open();
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //found = true;
                            if (reader["CompName"] != null && reader["CompName"] != DBNull.Value)
                            {
                                //HelperFuncs.writeToSiteErrors("pmShipperCompName", reader["CompName"].ToString());
                                //rdBOL.SetParameterValue("pmConsCompName", reader["CompName"].ToString());
                                BOL_Params.ConsCompName = reader["CompName"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmConsCompName", string.Empty);
                                BOL_Params.ConsCompName = string.Empty;
                            }

                            if (reader["Addr1"] != null && reader["Addr1"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue("pmConsAddr1", reader["Addr1"].ToString());
                                BOL_Params.ConsAddr1 = reader["Addr1"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmConsAddr1", string.Empty);
                                BOL_Params.ConsAddr1 = string.Empty;
                            }

                            if (reader["Addr2"] != null && reader["Addr2"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue("pmConsAddr2", reader["Addr2"].ToString());
                                BOL_Params.ConsAddr2 = reader["Addr2"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmConsAddr2", string.Empty);
                                BOL_Params.ConsAddr2 = string.Empty;
                            }

                            if (reader["Addr3"] != null && reader["Addr3"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue("pmConsAddr3", reader["Addr3"].ToString());
                                BOL_Params.ConsAddr3 = reader["Addr3"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmConsAddr3", string.Empty);
                                BOL_Params.ConsAddr3 = string.Empty;
                            }

                            if (reader["Phone"] != null && reader["Phone"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue("pmConsPhone", reader["Phone"].ToString());
                                BOL_Params.ConsPhone = reader["Phone"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmConsPhone", string.Empty);
                                BOL_Params.ConsPhone = string.Empty;
                            }

                            if (reader["FAX"] != null && reader["FAX"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue("pmConsFAX", reader["FAX"].ToString());
                                BOL_Params.ConsFAX = reader["FAX"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmConsFAX", string.Empty);
                                BOL_Params.ConsFAX = string.Empty;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("SetConsigneeRptParameters", e.ToString());
        }
    }

    #endregion

    #region SetCarrierRptParameters

    public static void SetCarrierRptParameters(ref SharedLTL.BolParams BOL_Params, int segId, ref int intShipmentID)
    {
        try
        {
            string sql = string.Concat(
                "select comp.CompName, comp.Addr1, comp.Addr2, (City + ', ' + State + ' ' + Zip) as Addr3, comp.Phone, comp.FAX from tbl_COMPANY comp",
                " inner join tbl_SEGMENTS seg on seg.CompID_CAR=comp.CompID where seg.ShipmentID=", intShipmentID, " and seg.SegmentID=", segId);

            bool found = false;
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = sql;
                    conn.Open();
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //found = true;
                            if (reader["CompName"] != null && reader["CompName"] != DBNull.Value)
                            {
                                HelperFuncs.writeToSiteErrors("pmCarrierCompName", reader["CompName"].ToString());
                                //rdBOL.SetParameterValue("pmCarrierCompName", reader["CompName"].ToString());
                                BOL_Params.CarrierCompName = reader["CompName"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmCarrierCompName", string.Empty);
                                BOL_Params.CarrierCompName = string.Empty;
                            }

                            if (reader["Phone"] != null && reader["Phone"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue("pmCarrierPhone", reader["Phone"].ToString());
                                BOL_Params.CarrierPhone = reader["Phone"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmCarrierPhone", string.Empty);
                                BOL_Params.CarrierPhone = string.Empty;
                            }

                            if (reader["FAX"] != null && reader["FAX"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue("pmCarrierFAX", reader["FAX"].ToString());
                                BOL_Params.CarrierFAX = reader["FAX"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue("pmCarrierFAX", string.Empty);
                                BOL_Params.CarrierFAX = string.Empty;
                            }
                        }
                    }
                }
            }
            //if (!found)
            //{
            //    rdBOL.SetParameterValue("pmCarrierCompName", string.Empty);
            //    rdBOL.SetParameterValue("pmCarrierPhone", string.Empty);
            //    rdBOL.SetParameterValue("pmCarrierFAX", string.Empty);
            //}
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("SetCarrierRptParameters", e.ToString());
        }
    }

    #endregion

    #region SetItemsRptParameters

    public static void SetItemsRptParameters(ref SharedLTL.BolParams BOL_Params, ref int intShipmentID)
    {
        try
        {
            string sql = string.Concat(
                "select Class,Descr,HR,Kind,Nmfc,Pcs,Units,WtLBS from tbl_ITEMS",
                " where SHIPMENTID=", intShipmentID);

            //byte i = 1;

            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = sql;
                    conn.Open();
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //HelperFuncs.writeToSiteErrors("read i=", i.ToString());
                            SharedLTL.bolItem bolItem = new SharedLTL.bolItem();
                            if (reader["Class"] != null && reader["Class"] != DBNull.Value)
                            {

                                //rdBOL.SetParameterValue(string.Concat("pmClass", i), reader["Class"].ToString());
                                bolItem.fClass = reader["Class"].ToString();
                                //HelperFuncs.writeToSiteErrors(string.Concat("pmClass", i), reader["Class"].ToString());
                            }
                            else
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmClass", i), string.Empty);
                                bolItem.fClass = string.Empty;
                            }

                            if (reader["Descr"] != null && reader["Descr"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmDescr", i), reader["Descr"].ToString());
                                bolItem.Descr = reader["Descr"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmDescr", i), string.Empty);
                                bolItem.Descr = string.Empty;
                            }

                            //if (reader["HR"] != null && reader["HR"] != DBNull.Value)
                            //{
                            //    HelperFuncs.writeToSiteErrors("HR Live", reader["HR"].ToString());
                            //}
                            //else
                            //{
                            //    HelperFuncs.writeToSiteErrors("HR Live", "reader was null");
                            //}


                            if (reader["HR"] != null && reader["HR"] != DBNull.Value && (bool)reader["HR"] == true)
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmHR", i), reader["HR"].ToString());
                                //bolItem.HR = reader["HR"].ToString();
                                bolItem.HR = "YES";
                            }
                            else
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmHR", i), string.Empty);

                                bolItem.HR = string.Empty;
                            }

                            if (reader["Kind"] != null && reader["Kind"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmKind", i), reader["Kind"].ToString());
                                bolItem.Kind = reader["Kind"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmKind", i), string.Empty);
                                bolItem.Kind = string.Empty;
                            }

                            if (reader["Nmfc"] != null && reader["Nmfc"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmNmfc", i), reader["Nmfc"].ToString());
                                bolItem.Nmfc = reader["Nmfc"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmNmfc", i), string.Empty);
                                bolItem.Nmfc = string.Empty;
                            }

                            if (reader["Pcs"] != null && reader["Pcs"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmPcs", i), reader["Pcs"].ToString());
                                bolItem.Pcs = reader["Pcs"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmPcs", i), string.Empty);
                                bolItem.Pcs = string.Empty;
                            }

                            if (reader["Units"] != null && reader["Units"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmUnits", i), reader["Units"].ToString());
                                bolItem.Units = reader["Units"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmUnits", i), string.Empty);
                                bolItem.Units = string.Empty;
                            }

                            if (reader["WtLBS"] != null && reader["WtLBS"] != DBNull.Value)
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmWtLBS", i), reader["WtLBS"].ToString());
                                bolItem.WtLBS = reader["WtLBS"].ToString();
                            }
                            else
                            {
                                //rdBOL.SetParameterValue(string.Concat("pmWtLBS", i), string.Empty);
                                bolItem.WtLBS = string.Empty;
                            }

                            //i++;
                            //HelperFuncs.writeToSiteErrors("done read i=", i.ToString());
                            BOL_Params.bolItems.Add(bolItem);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("SetItemsRptParameters", e.ToString());
        }
    }

    #endregion

    #region GetAdditionalServices

    private static DataTable GetAdditionalServices(ref int intSegmentID, ref SqlCommand comm, ref SqlDataAdapter da)
    {
        string strSQL;
        strSQL = "select lkup.LkUpValue from AESData.dbo.tbl_LKUP lkup inner join tbl_AccDetail acc on acc.LKUPID_ACC=lkup.LkUpID"
            + " where acc.SEGMENTID=" + intSegmentID + " and acc.ACCType='SEG'"; ;

        DataTable dt = new DataTable();
        comm.CommandText = strSQL;
        da.SelectCommand = comm;
        da.Fill(dt);
        return dt;
    }

    #endregion

    #endregion

    // This is on the way to be replaced with processCyberSourcePayment
    #region processCyberSourcePaymentViaAlex2015

    //public static void processCyberSourcePaymentViaAlex2015(ref bool isPaymentSuccess, ref string ccDecision, ref string reasonCode, string requester,
    //    ref int intShipmentID, string InvoiceNbr, ref NameValueCollection requestedValues, ref double insuranceCost)
    //{
    //    //Initialize web service/API object
    //    Alex2015WebReference.RateService2 rsAlex2015 = new Alex2015WebReference.RateService2();
    //    Alex2015WebReference.CC_PaymentInfo info = new Alex2015WebReference.CC_PaymentInfo();
    //    Alex2015WebReference.CC_PaymentResponse res = new Alex2015WebReference.CC_PaymentResponse();
    //    //string requester = "BookingPayment";

    //    try
    //    {
    //        if (requester.Equals("BookingPayment4"))
    //        {
    //            info.InvoiceNbr = InvoiceNbr;
    //        }
    //        else
    //        {
    //            info.InvoiceNbr = string.Empty;
    //        }
    //        info.shipID = intShipmentID;
    //        info.firstName = requestedValues["txtBFirstName"];
    //        info.lastName = requestedValues["txtBLastName"];
    //        info.street = requestedValues["txtBStreet"];
    //        info.city = requestedValues["txtBCity"];
    //        info.state = requestedValues["ddlBState"];
    //        info.zip = requestedValues["txtBPostalCode"];
    //        info.country = "US";
    //        info.email = requestedValues["txtBEmail"];
    //        info.ccNumber = requestedValues["txtCreditCard"];
    //        info.expMonth = requestedValues["ddlExpMonth"];
    //        info.expYear = requestedValues["ddlExpYear"];
    //        info.rate = Convert.ToDouble(requestedValues["rate"]) + insuranceCost;
    //        info.cw = requestedValues["txtCVV"];

    //        res = rsAlex2015.processCyberSourcePayment(requester, info);

    //        // Set the out variables
    //        isPaymentSuccess = res.isPaymentSuccess;
    //        ccDecision = res.decision;
    //        reasonCode = res.reasonCode;
    //    }
    //    catch (Exception e)
    //    {
    //        writeToSiteErrors("processCyberSourcePaymentViaAlex2015", e.ToString());
    //    }
    //}

    #endregion

    #region processCyberSourcePayment

    //public static void processCyberSourcePayment(ref bool isPaymentSuccess, ref string requestID, ref string requestToken, ref string ccDecision, ref string reasonCode,
    //    string requester, ref int intShipmentID, string InvoiceNbr, ref NameValueCollection requestedValues, ref double insuranceCost)
    //{

    //    CC_PaymentInfo info = new CC_PaymentInfo();
    //    CC_PaymentResponse res = new CC_PaymentResponse();

    //    try
    //    {
    //        if (requester.Equals("BookingPayment4"))
    //        {
    //            info.InvoiceNbr = InvoiceNbr;
    //        }
    //        else
    //        {
    //            info.InvoiceNbr = string.Empty;
    //        }
    //        info.shipID = intShipmentID;
    //        info.firstName = requestedValues["txtBFirstName"];
    //        info.lastName = requestedValues["txtBLastName"];
    //        info.street = requestedValues["txtBStreet"];
    //        info.city = requestedValues["txtBCity"];
    //        info.state = requestedValues["ddlBState"];
    //        info.zip = requestedValues["txtBPostalCode"];
    //        info.country = "US";
    //        info.email = requestedValues["txtBEmail"];
    //        info.ccNumber = requestedValues["txtCreditCard"];
    //        info.expMonth = requestedValues["ddlExpMonth"];
    //        info.expYear = requestedValues["ddlExpYear"];
    //        info.rate = Convert.ToDouble(requestedValues["rate"]) + insuranceCost;
    //        info.cw = requestedValues["txtCVV"];

    //        res = processCyberSourcePayment_2(requester, info);

    //        // Set the out variables
    //        isPaymentSuccess = res.isPaymentSuccess;
    //        ccDecision = res.decision;
    //        reasonCode = res.reasonCode;
    //        requestID = res.requestID;
    //        requestToken = res.requestToken;
    //    }
    //    catch (Exception e)
    //    {
    //        writeToSiteErrors("processCyberSourcePayment", e.ToString());
    //    }
    //}

    #endregion

    #region  processCyberSourcePayment_2

    //public static CC_PaymentResponse processCyberSourcePayment_2(string requester, CC_PaymentInfo info)
    //{
    //    try
    //    {
    //        HelperFuncs.writeToSiteErrors("processCyberSourcePayment", "hit function");

    //        CreditCardPaymentProcessor ccpp = new CreditCardPaymentProcessor();

    //        CC_PaymentResponse pmtRes = new CC_PaymentResponse();

    //        string idOfShipment = string.Empty;
    //        if (requester.Equals("BookingPayment4"))
    //        {
    //            idOfShipment = info.InvoiceNbr;
    //        }
    //        else
    //        {
    //            idOfShipment = info.shipID.ToString();
    //        }

    //        HelperFuncs.writeToSiteErrors("processCyberSourcePayment info", string.Concat("idOfShipment ", idOfShipment, " info.firstName ", info.firstName, " info.lastName ",
    //            info.lastName, " info.street ", info.street, " info.city ", info.city, " info.state ", info.state, " info.zip ", info.zip, " info.country ", info.country,
    //            " info.email ", info.email, " info.ccNumber ", info.ccNumber, " info.expMonth ", info.expMonth, " info.expYear ", info.expYear, " info.rate ", info.rate,
    //            " info.cw ", info.cw));

    //        pmtRes.isPaymentSuccess = ccpp.CreatePayment(idOfShipment, info.firstName, info.lastName,
    //          info.street, info.city, info.state,
    //          info.zip, info.country, info.email, "", info.ccNumber,
    //          info.expMonth, info.expYear, info.rate, info.cw);

    //        Hashtable reply = ccpp.getReply();

    //        if (pmtRes.isPaymentSuccess.Equals(true))
    //        {
    //            pmtRes.decision = string.Empty;
    //            pmtRes.reasonCode = string.Empty;
    //        }
    //        else
    //        {
    //            pmtRes.error = "Payment failure" + "<br />" + reply["reasonCode"].ToString();
    //            pmtRes.decision = CreditCardPaymentProcessor.GetPaymentDecision(reply);
    //            pmtRes.reasonCode = reply["reasonCode"].ToString();
    //        }

    //        #region Get requestID and requestToken 

    //        if (reply["requestID"] != null)
    //        {
    //            pmtRes.requestID = reply["requestID"].ToString();
    //            HelperFuncs.writeToSiteErrors("processCyberSourcePayment requestID", pmtRes.requestID);
    //        }
    //        else
    //        {
    //            pmtRes.requestID = string.Empty;
    //            HelperFuncs.writeToSiteErrors("processCyberSourcePayment requestID", "was null");
    //        }

    //        if (reply["requestToken"] != null)
    //        {
    //            pmtRes.requestToken = reply["requestToken"].ToString();
    //            HelperFuncs.writeToSiteErrors("processCyberSourcePayment requestToken", pmtRes.requestToken);
    //        }
    //        else
    //        {
    //            pmtRes.requestToken = string.Empty;
    //            HelperFuncs.writeToSiteErrors("processCyberSourcePayment requestToken", "was null");
    //        }

    //        #endregion

    //        return pmtRes;

    //    }
    //    catch (Exception e)
    //    {
    //        #region Catch code

    //        HelperFuncs.writeToSiteErrors("processCyberSourcePayment", e.ToString());

    //        CC_PaymentResponse pmtRes = new CC_PaymentResponse();
    //        return pmtRes;

    //        #endregion
    //    }
    //}

    #endregion

    public struct CC_PaymentInfo
    {
        public int shipID;
        public double rate;
        public string InvoiceNbr, firstName, lastName, street, city, state, zip, country, email, ccNumber, expMonth, expYear, cw;
    }

    public struct CC_PaymentResponse
    {
        public bool isPaymentSuccess;
        //Hashtable reply;
        public string error, decision, reasonCode, requestID, requestToken;
    }

    public struct CybsCC_PaymentInfo
    {
        public string cardAccountNumber, cardExpirationMonth, cardExpirationYear, cardType, city, state, postalCode, country, email, currency, firstName, lastName,
            cw, street1, subscriptionID, decision, reasonCode;
    }

    #region GetLastAutogeneratedID

    public static int GetLastAutogeneratedID(SqlCommand comm) // Overloaded function
    {
        int intID;
        string strSQL;
        strSQL = "select @@identity";
        comm.CommandText = strSQL;
        intID = Convert.ToInt32(comm.ExecuteScalar());
        return intID;
    }

    #endregion

    #region updateStatusesAndPro
    public static void updateStatusesAndPro(string proNbr, int shipID, int segID, string newStatus)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    string sql;
                    conn.Open();
                    sql = string.Concat("UPDATE tbl_SEGMENTS SET SegStatus = \'", newStatus, "\', ProNbr = '", proNbr, "' ",
                              "WHERE SegmentID = " + segID);
                    command.Connection = conn;
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                    sql = string.Concat("UPDATE tbl_SHIPMENTS SET ShipStatus = \'", newStatus, "\'  WHERE ShipmentID = ", shipID);
                    command.Connection = conn;
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception e)
        {
            writeToSiteErrors("updateStatusesAndPro", e.ToString());
        }
    }
    #endregion

    #region updateStatusesAndDispatchName
    public static void updateStatusesAndDispatchName(string shipID, string segID, string newStatus, string dispatchName)
    {
        //string newStatus = "INTRANS";
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData);
        SqlCommand command = new SqlCommand();
        string sql;

        try
        {
            conn.Open();
            sql = "UPDATE tbl_SEGMENTS SET SegStatus = \'" + newStatus + "\', DispatchName = '" + dispatchName + "' " +
                      "WHERE SegmentID = " + segID;
            command.Connection = conn;
            command.CommandText = sql;
            command.ExecuteNonQuery();

            sql = "UPDATE tbl_SHIPMENTS SET ShipStatus = \'" + newStatus + "\' WHERE ShipmentID = " + shipID;
            command.Connection = conn;
            command.CommandText = sql;
            command.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }
        catch (Exception e)
        {
            try
            {
                writeToSiteErrors("updateStatusesAndDispatchName", e.ToString());
                conn.Close();
                conn.Dispose();
                command.Dispose();

            }
            catch
            {
            }
        }
    }
    #endregion

    #region updateStatuses
    public static void updateStatuses(string shipID, string segID, string newStatus)
    {

        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData);
        SqlCommand command = new SqlCommand();
        string sql;

        try
        {
            conn.Open();
            sql = "UPDATE tbl_SEGMENTS SET SegStatus = \'" + newStatus + "\' " +
                      "WHERE SegmentID = " + segID;
            command.Connection = conn;
            command.CommandText = sql;
            command.ExecuteNonQuery();

            sql = "UPDATE tbl_SHIPMENTS SET ShipStatus = \'" + newStatus + "\' WHERE ShipmentID = " + shipID;
            command.Connection = conn;
            command.CommandText = sql;
            command.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }
        catch (Exception e)
        {
            try
            {
                writeToSiteErrors("updateStatuses", e.ToString());
                conn.Close();
                conn.Dispose();
                command.Dispose();

            }
            catch
            {
            }
        }
    }
    #endregion

    #region GetStates
    private static string[] _states ={"", "AA", "AE", "AP", "AL", "AK", "AS", "AZ", "AR", "CA", "CO",
        "CT", "DE", "DC", "FM", "FL", "GA", "GU", "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME",
        "MH", "MD", "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ", "NM", "NY", "NC",
        "ND", "MP", "OH", "OK", "OR", "PW", "PA", "PR", "RI", "SC", "SD", "TN", "TX", "UT", "VT", "VI", "VA",
        "WA", "WV", "WI", "WY"};

    // Used on bookingpayment
    public static StringCollection GetStates()
    {
        StringCollection states = new StringCollection();
        states.AddRange(_states);
        return states;
    }
    #endregion

    #region GetInches, GetLB
    public static double GetInches(double val, string unit)
    {
        double inch = 0;
        if (unit.Equals("cm"))
        {
            inch = val * 0.3936;
            return inch;
        }
        else
        {
            return val;
        }
    }

    public static double GetLB(double val, string unit)
    {
        double lb = 0;
        if (unit.Equals("kg"))
        {
            lb = val * 2.20462262;
            return lb;
        }
        else
        {
            return val;
        }
    }
    #endregion

    #region insertIntoGCM_STATS, updateGCM_STATS
    // Used by reefer
    public static void insertIntoGCM_STATS(string dest, string useAccount, string orig)
    {
        SqlConnection conn = new SqlConnection(connStringRater2009);
        SqlCommand comm = new SqlCommand();
        try
        {
            string sql = "INSERT INTO SQL_STATS_GCM (UserName, Type, Direction, Service, Origin, Destination, Day, Count, Completed) VALUES ('"
                + useAccount + "', 'ReeferLTL', 'Domestic', 'D2D', '" + orig + "', '" + dest + "', '" + DateTime.Now.Date.ToString("MM/dd/yyyy") + "', 1, 'NO')";

            conn.Open();
            comm.Connection = conn;
            comm.CommandText = sql;
            comm.ExecuteNonQuery();

            conn.Close();
            conn.Dispose();
            comm.Dispose();
        }
        catch (Exception cE)
        {
            try
            {
                conn.Close();
                conn.Dispose();
                comm.Dispose();
            }
            catch
            {
            }
            HelperFuncs.writeToSiteErrors("InsertToSQL_STATS_GCM (Live)", cE.ToString(), "");
        }
    }

    public static void updateGCM_STATS(string dest, string useAccount, string orig)
    {
        SqlConnection conn = new SqlConnection(connStringRater2009);
        SqlCommand comm = new SqlCommand();
        try
        {
            string sql = "SELECT ID, Count FROM SQL_STATS_GCM WHERE UserName = '"
                + useAccount + "' AND Type='ReeferLTL' AND Direction = 'Domestic' AND Service = 'D2D' AND Origin = '" + orig + "' AND Destination = '" + dest + "' AND Day = '"
                + DateTime.Now.Date.ToString("MM/dd/yyyy") + "' ORDER BY ID DESC";


            conn.Open();
            comm.Connection = conn;
            comm.CommandText = sql;
            SqlDataReader reader;
            reader = comm.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                int xCount = reader.GetInt32(reader.GetOrdinal("Count"));
                xCount += 1;
                int logId = reader.GetInt32(reader.GetOrdinal("ID"));
                sql = "UPDATE SQL_STATS_GCM SET Completed = 'YES' WHERE ID = " + logId;

                reader.Close();
                //SqlCommand comm2 = new SqlCommand();
                //comm2.Connection = conn;
                comm.CommandText = sql;
                comm.ExecuteNonQuery();
            }


            conn.Close();
            conn.Dispose();
            comm.Dispose();
        }
        catch (Exception cE)
        {
            try
            {
                conn.Close();
                conn.Dispose();
                comm.Dispose();
            }
            catch
            {
            }
            HelperFuncs.writeToSiteErrors("UpdateSQL_STATS_GCM (Live)", cE.ToString(), "");
        }
    }
    #endregion

    #region getDocs
    
    // Pulls documents/images for a particular shipment
    public static List<string> getDocs(int id)
    {
        List<string> data = new List<string>();
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAudit);
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        try
        {
            string sql = "";

            sql = "SELECT Directory " +
                "FROM DOCUMENTS WHERE ShipmentID = '" + id.ToString() + "'";

            command.CommandText = sql;
            conn.Open();
            command.Connection = conn;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Directory"] != DBNull.Value)
                {
                    data.Add((string)reader["Directory"]);
                }
            }

            reader.Close();
            reader.Dispose();
            conn.Close();
            conn.Dispose();
            command.Dispose();
            return data;
        }
        catch (Exception e)
        {
            try
            {
                conn.Close();
                conn.Dispose();
                command.Dispose();
            }
            catch
            {
            }
            HelperFuncs.writeToSiteErrors("getDocs", e.ToString(), "");
            return data;
        }
    }
    
    #endregion

    #region getDocs

    public static void getDocs(ref List<string> data, ref string id)
    {

        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAudit);
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        try
        {
            string sql = "";

            sql = "SELECT Directory " +
                "FROM DOCUMENTS WHERE ShipmentID = '" + id + "'";

            command.CommandText = sql;
            conn.Open();
            command.Connection = conn;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Directory"] != DBNull.Value)
                {
                    data.Add((string)reader["Directory"]);
                }
            }

            reader.Close();
            reader.Dispose();
            conn.Close();
            conn.Dispose();
            command.Dispose();
            //return data;
        }
        catch (Exception e)
        {
            try
            {
                conn.Close();
                conn.Dispose();
                command.Dispose();
            }
            catch (Exception d)
            {
            }
            HelperFuncs.writeToSiteErrors("BOL Editor Get Docs", e.ToString());
            //return data;
        }
    }

    #endregion

    #region getPos
    public static List<string> getPos(int id)
    {
        List<string> data = new List<string>();
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData);
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        try
        {
            string sql = "SELECT PONumber " +
            "FROM tbl_PO " +
            "WHERE ShipmentID = " + "'" + id + "'" + "";

            command.CommandText = sql;
            conn.Open();
            command.Connection = conn;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (reader["PONumber"] != null && reader["PONumber"] != DBNull.Value)
                {
                    data.Add((string)reader["PONumber"]);
                }
            }

            reader.Close();
            reader.Dispose();
            conn.Close();
            conn.Dispose();
            command.Dispose();
            return data;
        }
        catch (Exception e)
        {
            try
            {
                conn.Close();
                conn.Dispose();
                command.Dispose();
            }
            catch
            {
            }
            HelperFuncs.writeToSiteErrors("Tracking", e.ToString(), "");
            return data;
        }
    }
    #endregion

    #region getDbFields
    public static List<object> getDbFields(string sql, string connString, List<string> columnNames)
    {
        List<object> objects = new List<object>();
        SqlConnection conn = new SqlConnection(connString);
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        command.CommandText = sql;
        command.Connection = conn;
        conn.Open();

        try
        {
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                for (int i = 0; i < columnNames.Count; i++)
                {
                    objects.Add((object)reader[columnNames[i]]);
                }
                reader.Close();
                reader.Dispose();
                conn.Close();
                conn.Dispose();
                command.Dispose();
                return objects;
            }
            else
            {
                reader.Close();
                reader.Dispose();
                conn.Close();
                conn.Dispose();
                command.Dispose();
                return null;
            }
        }
        catch (Exception e)
        {
            try
            {
                conn.Close();
                conn.Dispose();
                command.Dispose();
            }
            catch
            {
            }
            HelperFuncs.writeToSiteErrors("getDbFields", e.ToString(), "");
            return null;
        }
    }
    #endregion

    //---------------------------------------------------------------------------------------

    #region For Audit Page

    #region isValidDbInfo
    public static bool isValidDbInfo(SqlDataReader reader, string column)
    {
        if (reader[column] != DBNull.Value)
            return true;
        else
            return false;
    }
    #endregion

    // Pulls addresses for a particular shipment
    public static string[] getAddresses(int id)
    {
        string[] data = new string[7];
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAudit);
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        string sql = "";

        sql = "SELECT * " +
            "FROM ADDRESSES WHERE AddressID = '" + id.ToString() + "'";

        command.CommandText = sql;
        conn.Open();
        command.Connection = conn;
        reader = command.ExecuteReader();

        if (reader.Read())
        {
            data[0] = (string)reader["CompanyName"];
            if (reader["Address1"] != DBNull.Value)
            {
                data[1] = (string)reader["Address1"];
            }
            else data[1] = "";
            if (reader["Address2"] != DBNull.Value)
            {
                data[2] = (string)reader["Address2"];
            }
            else data[2] = "";
            if (reader["Address3"] != DBNull.Value)
            {
                data[3] = (string)reader["Address3"];
            }
            else data[3] = "";

            if (reader["City"] != DBNull.Value)
            {
                data[4] = (string)reader["City"];
            }
            else data[4] = "";
            if (reader["State"] != DBNull.Value)
            {
                data[5] = (string)reader["State"];
            }
            else data[5] = "";
            if (reader["PostalCode"] != DBNull.Value)
            {
                data[6] = (string)reader["PostalCode"];
            }
            else data[6] = "";
        }

        reader.Close();
        reader.Dispose();
        conn.Close();
        conn.Dispose();
        command.Dispose();
        return data;
    }

    // Returns freight classes 
    public static List<double> getClasses(int id)
    {
        List<double> fClasses = new List<double>();
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAudit);
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        string sql = "";

        sql = "SELECT fClass " +
            "FROM CLASSES WHERE ShipmentID = '" + id.ToString() + "'";

        command.CommandText = sql;
        conn.Open();
        command.Connection = conn;
        reader = command.ExecuteReader();

        while (reader.Read())
        {
            //data.weights.Add((int)reader["Weight"]);
            if (reader["fClass"] != DBNull.Value)
            {
                fClasses.Add((double)reader["fClass"]);
            }
        }

        reader.Close();
        reader.Dispose();
        conn.Close();
        conn.Dispose();
        command.Dispose();
        return fClasses;
    }

    //returns additional info for shipment
    public static string getAddInfo(int id)
    {
        string addInfo = "", sql = "";
        bool found = false;
        int counter = 0;
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAudit);
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;


        sql = "SELECT Description " +
            "FROM ADDITIONAL_INFO WHERE ShipmentID = '" + id.ToString() + "'";

        command.CommandText = sql;
        conn.Open();
        command.Connection = conn;
        reader = command.ExecuteReader();

        //System.Data.DataTable dt = reader.GetSchemaTable();
        //int numRows = dt.Rows.Count;
        //if(numRows != 1)
        //{
        //    string s = "";
        //}


        while (reader.Read())
        {
            counter++;
            if (reader["Description"] != DBNull.Value)
            {
                addInfo += "*" + (string)reader["Description"];
                //if(numRows > counter)
                addInfo += "<br>";
            }
            found = true;
        }
        if (found == false) addInfo = "";

        reader.Close();
        reader.Dispose();
        conn.Close();
        conn.Dispose();
        command.Dispose();
        return addInfo;
    }

    public static string getDateByBatchID(string batchID)
    {
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAudit);
        SqlCommand command = new SqlCommand();
        string sql = "SELECT Date FROM BATCHES WHERE BatchID = '" + batchID + "'";
        command.CommandText = sql;
        conn.Open();
        command.Connection = conn;
        string batchDate = command.ExecuteScalar().ToString();

        conn.Close();
        conn.Dispose();
        command.Dispose();
        return batchDate;
    }

    public static int getLatestBatchID()
    {
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAudit);
        SqlCommand command = new SqlCommand();
        string sql = "SELECT MAX(BatchID) FROM BATCHES";
        command.CommandText = sql;
        conn.Open();
        command.Connection = conn;
        int batchID = (Int32)command.ExecuteScalar();

        conn.Close();
        conn.Dispose();
        command.Dispose();
        return batchID;
    }

    //get batch id by date
    public static int getBatchID(DateTime date)
    {

        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAudit);
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        string sql = "";

        sql = "SELECT BatchID, Date, ShipmentCount " +
            "FROM BATCHES WHERE Date = '" + date.ToShortDateString() + "'";

        command.CommandText = sql;

        conn.Open();
        command.Connection = conn;
        reader = command.ExecuteReader();


        int batchId = 0, shipmentsCnt = 0;
        DateTime dt = DateTime.Today;

        if (reader.Read())
        {
            batchId = (int)reader["BatchID"];
            shipmentsCnt = (int)reader["ShipmentCount"];
            dt = (DateTime)reader["Date"];
        }


        reader.Close();
        reader.Dispose();
        conn.Close();
        conn.Dispose();
        command.Dispose();

        return batchId;
    }

    //get batch id by shipment ID
    public static int getBatchID(string sql)
    {

        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAudit);
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        //string sql = "";

        //sql = "SELECT BatchID " +
        //    "FROM SHIPMENTS WHERE ShipmentID = '" + Convert.ToInt32(shipID) + "'";

        command.CommandText = sql;

        conn.Open();
        command.Connection = conn;
        reader = command.ExecuteReader();


        int batchId = 0;


        if (reader.Read())
        {
            batchId = (int)reader["BatchID"];
        }
        else
        {
            reader.Close();
            reader.Dispose();
            conn.Close();
            conn.Dispose();
            command.Dispose();
            return -1;
        }


        reader.Close();
        reader.Dispose();
        conn.Close();
        conn.Dispose();
        command.Dispose();

        return batchId;
    }
    // Not used here, can delete
    public static int getNumFlagged(int batchID)
    {
        int numFlagged = 0;
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAudit);
        SqlCommand command = new SqlCommand();
        string sql = "SELECT COUNT(*) FROM SHIPMENTS WHERE BatchID ='" + batchID + "' AND Flagged='True'";
        command.CommandText = sql;
        conn.Open();
        command.Connection = conn;
        numFlagged = (int)command.ExecuteScalar();
        conn.Close();
        conn.Dispose();
        command.Dispose();
        return numFlagged;
    }

    public static void updateSellRate(string sellRate, string segmentID, string sessID)
    {
        //check if there is an entry in ratequotes with the segmentID, if yes update sellRate,
        //if not make an entry into ratequotes with all info
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData);
        SqlCommand command = new SqlCommand();
        bool has = hasRatequote(segmentID);
        string sql;

        if (has)
        {
            sql = "UPDATE tbl_RATEQUOTES SET AESQuote='" + sellRate + "', NetCharge='" + sellRate +
                "' WHERE SEGMENTID='" + segmentID + "'";
        }
        else
        {
            string name = "";
            List<object> objects = new List<object>();
            List<string> columnNames = new List<string>();
            sql = "SELECT Name FROM tbl_PRIVILEGES WHERE EmployeeID=' " + sessID + "'";
            columnNames.Add("Name");
            objects = getDbFields(sql, AppCodeConstants.connStringAesData, columnNames);
            if (objects[0] != DBNull.Value)
            {
                name = (string)objects[0];
            }
            sql = "INSERT INTO tbl_RATEQUOTES(SEGMENTID, NetCharge, AESQuote, Initials) " +
           "VALUES('" + segmentID + "', '" + sellRate + "', '" + sellRate + "', '" + name + "')";
        }

        command.CommandText = sql;
        conn.Open();
        command.Connection = conn;
        int retVal = command.ExecuteNonQuery();
        conn.Close();
        conn.Dispose();
        command.Dispose();
    }

    public static void updateBuyRate(string buyRate, string segmentID)
    {
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData);
        SqlCommand command = new SqlCommand();
        string sql = "UPDATE tbl_SEGMENTS SET CarrierQuoteAmt='" + buyRate +
            "' WHERE SEGMENTID='" + segmentID + "'";
        //writeToSiteErrors("sql", sql);
        command.CommandText = sql;
        conn.Open();
        command.Connection = conn;
        int retVal = command.ExecuteNonQuery();
        conn.Close();
        conn.Dispose();
        command.Dispose();
    }

    public static bool hasRatequote(string segmentID)
    {
        bool has = false;
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData);
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        string sql = "";

        sql = "SELECT RATEQUOTEID " +
            "FROM tbl_RATEQUOTES WHERE SEGMENTID = '" + segmentID + "'";

        command.CommandText = sql;

        conn.Open();
        command.Connection = conn;
        reader = command.ExecuteReader();

        if (reader.Read())
        {
            has = true;
        }

        reader.Close();
        reader.Dispose();
        conn.Close();
        conn.Dispose();
        command.Dispose();

        return has;
    }
    #endregion

    #region writeToSiteErrors

    public static void writeToSiteErrors(string carrier, string exception) //overloaded, this one writes to LTL Rater database
    {
        DB.Log(carrier, exception);
       
    }

    public static void writeToSiteErrors(string carrier, string exception, string str) //overloaded, this one writes to LTL Rater database
    {
        DB.Log(carrier, exception);
       
    }

    public static void writeToDispatchLogs(string carrier, string exception)
    {
        //DB.Log(carrier, exception);
       
        SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aes_daylightSS"].ConnectionString);
        SqlCommand command = new SqlCommand();
        string sql, location = "Dispatch.aspx.cs";
        try
        {
            conn.Open();

            sql = string.Concat("INSERT INTO Dispatch_Logs(Exception, Page, Location, Source, ErrorDate, ErrorTime) ",
                "VALUES('", exception.Replace("'", ""), "', '", location, "', '", carrier.Replace("'", ""), "', 'LiveGCM','",
                DateTime.Today.ToShortDateString(), "', '", DateTime.Now.ToShortTimeString(), "')");

            command.Connection = conn;
            command.CommandText = sql;
            command.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
            command.Dispose();
        }
        catch (Exception e)
        {
            string str = e.ToString();
            //try
            //{
            //    writeToSiteErrors("writeToSiteErrors", e.ToString());
            //    conn.Close();
            //    conn.Dispose();
            //    command.Dispose();
            //}
            //catch
            //{
            //}
        }
       
    }

    public static void writeToDispatchLogs(string carrier, string exception, string str)
    {
        DB.Log(carrier, exception);
        /*
        SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aes_daylightSS"].ConnectionString);
        SqlCommand command = new SqlCommand();
        string sql, location = "Dispatch.aspx.cs";
        try
        {
            conn.Open();

            sql = string.Concat("INSERT INTO Dispatch_Logs(Exception, Page, Location, Source, ErrorDate, ErrorTime) ",
                "VALUES('", exception.Replace("'", ""), "', '", location, "', '", carrier.Replace("'", ""), "', 'LiveGCM','",
                DateTime.Today.ToShortDateString(), "', '", DateTime.Now.ToShortTimeString(), "')");

            command.Connection = conn;
            command.CommandText = sql;
            command.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
            command.Dispose();
        }
        catch (Exception e)
        {
            try
            {
                writeToSiteErrors("writeToSiteErrors", e.ToString());
                conn.Close();
                conn.Dispose();
                command.Dispose();
            }
            catch
            {
            }
        }
        */
    }

    #region writeBugReport
    //public static void writeBugReport(string s)
    //{
    //    string bugRpt;
    //    string path = "";
    //    //int ind;
    //    bugRpt = DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToShortTimeString() + "\r\n\r\n" +
    //        s +
    //        "--------------------------------------------------------------------------------------\r\n\r\n";
    //    string RailsFile = path + "\\b.txt";
    //    //WriteToStartOfFile(RailsFile, bugRpt);       
    //}

    public static void WriteToStartOfFile(string file, string newValue)
    {
        char[] buffer = new char[2048];
        string tempFile = file + ".tmp";
        File.Move(file, tempFile);
        using (StreamReader reader = new StreamReader(tempFile))
        {
            using (StreamWriter writer = new StreamWriter(file, false))
            {
                writer.Write(newValue);
                int totalRead;

                while ((totalRead = reader.Read(buffer, 0, buffer.Length)) > 0)

                    writer.Write(buffer, 0, totalRead);
            }
        }
        File.Delete(tempFile);
    }
    #endregion

    #region writeToLogs

    public static void writeToLogs(string carrier, string exception)
    {
        try
        {

            #region writeToLogs

            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringLTLRater))
            {

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = string.Concat("INSERT INTO GCM_Logs(Exception, Page, Location, ErrorDate, ErrorTime) ",
                                          "VALUES('", exception.Replace("'", ""), "', '", string.Empty, "', '", carrier.Replace("'", ""), "', '",
                                          DateTime.Today.ToShortDateString(), "', '", DateTime.Now.ToShortTimeString(), "')"); ;
                    conn.Open();
                    command.ExecuteNonQuery();

                }
            }

            #endregion
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("writeToLogs", e.ToString());
        }
    }

    #endregion

    #endregion

    #region Library Functions
    #region scrapeFromPage
    public static string scrapeFromPage(string[] tokens, string doc)
    {
        int ind;
        for (int i = 0; i < tokens.Length - 1; i++)
        {
            ind = doc.IndexOf(tokens[i]);
            if (ind != -1)
            {
                doc = doc.Substring(ind + 1);
            }
            else return ("not found");
        }
        ind = doc.IndexOf(tokens[tokens.Length - 1]);
        if (ind != -1)
        {
            doc = doc.Remove(ind);
        }
        else return ("not found");

        return doc.Trim();
    }
    #endregion

    #region getViewAndEvent
    public static string[] getViewAndEvent(string tmp)
    {
        string[] info = new string[2];
        int ind;
        //scrape viewstate, eventvalidation

        ind = tmp.IndexOf("__VIEWSTATE");
        tmp = tmp.Substring(ind);
        ind = tmp.IndexOf("value=");
        tmp = tmp.Substring(ind + 7);
        ind = tmp.IndexOf("\"");
        string viewstate = tmp.Remove(ind).Replace("/", "%2F").Replace("=", "%3D").Replace("+", "%2B");

        string eventvalidation = "";
        ind = tmp.IndexOf("__EVENTVALIDATION");
        if (ind != -1)
        {
            tmp = tmp.Substring(ind);
            ind = tmp.IndexOf("value=");
            tmp = tmp.Substring(ind + 7);
            ind = tmp.IndexOf("\"");
            eventvalidation = tmp.Remove(ind).Replace("/", "%2F").Replace("=", "%3D").Replace("+", "%2B");
        }

        info[0] = viewstate;
        info[1] = eventvalidation;
        return info;
    }
    #endregion

    #region EncodeTo64
    public static string EncodeTo64(string toEncode)
    {
        byte[] toEncodeAsBytes = System.Text.Encoding.Unicode.GetBytes(toEncode);
        string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
        return returnValue;
    }
    #endregion

    #region DecodeFrom64
    public static string DecodeFrom64(string encodedData)
    {
        byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
        string returnValue = System.Text.Encoding.Unicode.GetString(encodedDataAsBytes);
        return returnValue;
    }
    #endregion

    #endregion

    #region Reverse
    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
    #endregion

    #region IsOdd
    public static bool IsOdd(int value)
    {
        return value % 2 != 0;
    }
    #endregion

    #region UppercaseFirst
    public static string UppercaseFirst(string s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    }
    #endregion

    #region UppercaseFirstInEachWord
    public static string UppercaseFirstInEachWord(string s)
    {
        string[] forSplit = s.Split(' ');
        s = "";
        for (int j = 0; j < forSplit.Length; j++)
        {
            s += HelperFuncs.UppercaseFirst(forSplit[j]) + ' ';
        }
        return s;
    }
    #endregion

    #region HTTP Request

    // Special one for SEFL
    public static object generic_http_request_SEFL(string return_type, CookieContainer container, string req_url, string req_referrer, string req_content_type, string req_accept,
          string req_method, string data_string, bool req_allow_redirect)
    {
        #region BuildRequest
        // Used to build entire input
        StringBuilder sb = new StringBuilder();
        String DataParameters;

        // Used on each read operation
        byte[] buf = new byte[8192];

        // Prepare the web page we will be asking for
        HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(req_url);
        request.Referer = req_referrer;

        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.83 Safari/535.11";

        request.KeepAlive = true;
        request.Method = req_method;
        request.ContentType = req_content_type;
        request.Accept = req_accept;
        request.CookieContainer = new CookieContainer();
        request.CookieContainer = container;
        request.Timeout = 12000;

        #endregion

        #region GetResponse

        if (req_method == "POST")
        {
            #region Write to data stream
            DataParameters = data_string;
            byte[] postData;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            postData = encoding.GetBytes(DataParameters);
            request.ContentLength = postData.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(postData, 0, postData.Length);
            }
            #endregion
        }

        // Execute the request
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            if (return_type == "string")
            {
                // We will read data via the response stream
                #region Read data
                using (Stream resStream = response.GetResponseStream())
                {
                    string tempString = null;
                    int count = 0;

                    do
                    {
                        // Fill the buffer with data
                        count = resStream.Read(buf, 0, buf.Length);

                        // Make sure we read some data
                        if (count != 0)
                        {
                            // Translate from bytes to ASCII text
                            tempString = Encoding.ASCII.GetString(buf, 0, count);

                            // Continue building the string
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0); // Any more data to read?
                }
                #endregion
            }

            request.Abort();

            #region Return result
            if (return_type == "string")
            {
                if (req_referrer == "https://www.sefl.com/SeflRateQuote/servlet?action=JSP_FORWARD&wantsToBeSecure=Y&callingPage=rateQuote.jsp&jspPage=rateQuote.jsp")
                {
                    string doc2 = sb.ToString();
                    CookieCollection collection = new CookieCollection();
                    collection = response.Cookies;
                    foreach (Cookie cookie in collection)
                    {
                        if (cookie.Name == "SEFLGUID")
                        {
                            doc2 += "<()>" + cookie.Value;
                            response.Close();
                            return doc2;
                        }
                    }
                }
                response.Close();
                return sb.ToString();
            }
            else if (return_type == "collection")
            {
                CookieCollection collection = new CookieCollection();
                collection = response.Cookies;
                response.Close();
                return collection;
            }
            else
            {
                response.Close();
                return null;
            }
            #endregion
        }
        #endregion
    }

    public static object generic_http_request(string return_type, CookieContainer container, string req_url, string req_referrer, string req_content_type, string req_accept,
          string req_method, string data_string, bool req_allow_redirect)
    {
        #region BuildRequest
        // used to build entire input
        StringBuilder sb = new StringBuilder();
        String DataParameters;

        // used on each read operation
        byte[] buf = new byte[8192];

        // prepare the web page we will be asking for
        HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(req_url);
        request.Referer = req_referrer;

        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.83 Safari/535.11";

        request.KeepAlive = true;
        request.Method = req_method;
        request.ContentType = req_content_type;
        request.Accept = req_accept;
        request.CookieContainer = new CookieContainer();
        request.CookieContainer = container;
        request.Timeout = 12000;

        #endregion

        #region GetResponse

        if (req_method == "POST")
        {
            #region Write to data stream
            DataParameters = data_string;
            byte[] postData;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            postData = encoding.GetBytes(DataParameters);
            request.ContentLength = postData.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(postData, 0, postData.Length);
            }
            #endregion
        }

        // Execute the request
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            if (return_type == "string")
            {
                // We will read data via the response stream
                #region Read data
                using (Stream resStream = response.GetResponseStream())
                {
                    string tempString = null;
                    int count = 0;

                    do
                    {
                        // Fill the buffer with data
                        count = resStream.Read(buf, 0, buf.Length);

                        // Make sure we read some data
                        if (count != 0)
                        {
                            // Translate from bytes to ASCII text
                            tempString = Encoding.ASCII.GetString(buf, 0, count);

                            // Continue building the string
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0); // Any more data to read?
                }
                #endregion
            }

            request.Abort();

            #region Return result
            if (return_type == "string")
            {
                response.Close();
                return sb.ToString();
            }
            else if (return_type == "collection")
            {
                CookieCollection collection = new CookieCollection();
                collection = response.Cookies;
                response.Close();
                return collection;
            }
            else
            {
                response.Close();
                return null;
            }
            #endregion
        }
        #endregion
    }

    // Overloaded, has time out parameter
    public static object generic_http_request(string return_type, CookieContainer container, string req_url, string req_referrer, string req_content_type, string req_accept,
           string req_method, string data_string, bool req_allow_redirect, int timeOut)
    {
        #region BuildRequest
        // used to build entire input
        StringBuilder sb = new StringBuilder();
        String DataParameters;

        // used on each read operation
        byte[] buf = new byte[8192];

        // prepare the web page we will be asking for
        HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(req_url);
        request.Referer = req_referrer;

        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.83 Safari/535.11";

        request.KeepAlive = true;
        request.Method = req_method;
        request.ContentType = req_content_type;
        request.Accept = req_accept;
        request.CookieContainer = new CookieContainer();
        request.CookieContainer = container;
        request.Timeout = timeOut;

        #endregion

        #region GetResponse

        if (req_method == "POST")
        {
            #region Write to data stream
            DataParameters = data_string;
            byte[] postData;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            postData = encoding.GetBytes(DataParameters);
            request.ContentLength = postData.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(postData, 0, postData.Length);
            }
            #endregion
        }

        // Execute the request
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            if (return_type == "string")
            {
                // We will read data via the response stream
                #region Read data
                using (Stream resStream = response.GetResponseStream())
                {
                    string tempString = null;
                    int count = 0;

                    do
                    {
                        // Fill the buffer with data
                        count = resStream.Read(buf, 0, buf.Length);

                        // Make sure we read some data
                        if (count != 0)
                        {
                            // Translate from bytes to ASCII text
                            tempString = Encoding.ASCII.GetString(buf, 0, count);

                            // Continue building the string
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0); // Any more data to read?
                }
                #endregion
            }
            request.Abort();

            #region Return result
            if (return_type == "string")
            {
                response.Close();
                return sb.ToString();
            }
            else if (return_type == "collection")
            {
                CookieCollection collection = new CookieCollection();
                collection = response.Cookies;
                response.Close();
                return collection;
            }
            else
            {
                response.Close();
                return null;
            }
            #endregion
        }
        #endregion
    }

    public static object generic_http_request_3(string return_type, CookieContainer container, string req_url, string req_referrer, string req_content_type, string req_accept,
               string req_method, string data_string, bool req_allow_redirect, bool addHeader, string str, string auth)
    {
        #region BuildRequest

        StringBuilder sb = new StringBuilder();
        String DataParameters;

        byte[] buf = new byte[8192];

        HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(req_url);
        request.Referer = req_referrer;

        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.83 Safari/535.11";

        request.KeepAlive = true;
        request.Method = req_method;
        request.ContentType = req_content_type;
        request.Accept = req_accept;
        request.CookieContainer = new CookieContainer();
        request.CookieContainer = container;
        request.Timeout = 20000;
        request.AllowAutoRedirect = req_allow_redirect;

        #endregion

        #region GetResponse

        if (req_method == "POST")
        {
            #region Write to data stream
            DataParameters = data_string;
            byte[] postData;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            postData = encoding.GetBytes(DataParameters);
            request.ContentLength = postData.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(postData, 0, postData.Length);
            }
            #endregion
        }

        // Execute the request
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            if (return_type == "string" || return_type == "CollectionLocationDocument")
            {
                // We will read data via the response stream
                #region Read data
                using (Stream resStream = response.GetResponseStream())
                {
                    string tempString = null;
                    int count = 0;

                    do
                    {
                        // Fill the buffer with data
                        count = resStream.Read(buf, 0, buf.Length);

                        // Make sure we read some data
                        if (count != 0)
                        {
                            // Translate from bytes to ASCII text
                            tempString = Encoding.ASCII.GetString(buf, 0, count);

                            // Continue building the string
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0); // Any more data to read?
                }
                #endregion
            }
            request.Abort();

            #region Return result
            if (return_type == "string")
            {
                response.Close();

                return sb.ToString();

            }
            else if (return_type == "collection")
            {
                CookieCollection collection = new CookieCollection();
                collection = response.Cookies;
                response.Close();
                return collection;
            }
            else if (return_type == "location")
            {
                string s = response.Headers["Location"];
                response.Close();
                return s;
            }
            else if (return_type == "CollectionLocationDocument")
            {
                CollectionLocationDoc collLocDoc = new CollectionLocationDoc();
                collLocDoc.coll = new CookieCollection();
                collLocDoc.coll = response.Cookies;
                collLocDoc.doc = sb.ToString();
                collLocDoc.location = response.Headers["Location"];
                response.Close();
                return collLocDoc;
            }
            else
            {
                response.Close();
                return null;
            }
            #endregion
        }
        #endregion
    }

    #region generic_http_request_addHeaders
    public static object generic_http_request_addHeaders(string return_type, CookieContainer container, string req_url, string req_referrer, string req_content_type, string req_accept,
              string req_method, string data_string, bool req_allow_redirect, string[] headerName, string[] headerValue)
    {
        #region BuildRequest

        StringBuilder sb = new StringBuilder();
        String DataParameters;

        byte[] buf = new byte[8192];

        HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(req_url);
        request.Referer = req_referrer;

        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.83 Safari/535.11";

        request.KeepAlive = true;
        request.Method = req_method;
        request.ContentType = req_content_type;
        request.Accept = req_accept;
        request.CookieContainer = new CookieContainer();
        request.CookieContainer = container;
        request.Timeout = 1000000;
        request.AllowAutoRedirect = req_allow_redirect;

        for (int i = 0; i < headerName.Length; i++)
        {
            request.Headers[headerName[i]] = headerValue[i];
        }

        #endregion

        #region GetResponse

        if (req_method == "POST")
        {
            #region Write to data stream
            DataParameters = data_string;
            byte[] postData;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            postData = encoding.GetBytes(DataParameters);
            request.ContentLength = postData.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(postData, 0, postData.Length);
            }
            #endregion
        }

        // Execute the request
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            if (return_type == "string" || return_type == "CollectionLocationDocument")
            {
                // We will read data via the response stream
                #region Read data
                using (Stream resStream = response.GetResponseStream())
                {
                    string tempString = null;
                    int count = 0;

                    do
                    {
                        // Fill the buffer with data
                        count = resStream.Read(buf, 0, buf.Length);

                        // Make sure we read some data
                        if (count != 0)
                        {
                            // Translate from bytes to ASCII text
                            tempString = Encoding.ASCII.GetString(buf, 0, count);

                            // Continue building the string
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0); // Any more data to read?
                }
                #endregion
            }
            request.Abort();

            #region Return result
            if (return_type == "string")
            {
                response.Close();

                return sb.ToString();

            }
            else if (return_type == "collection")
            {
                CookieCollection collection = new CookieCollection();
                collection = response.Cookies;
                response.Close();
                return collection;
            }
            else if (return_type == "location")
            {
                string s = response.Headers["Location"];
                response.Close();
                return s;
            }
            else if (return_type == "CollectionLocationDocument")
            {
                CollectionLocationDoc collLocDoc = new CollectionLocationDoc();
                collLocDoc.coll = new CookieCollection();
                collLocDoc.coll = response.Cookies;
                collLocDoc.doc = sb.ToString();
                collLocDoc.location = response.Headers["Location"];
                response.Close();
                return collLocDoc;
            }
            else
            {
                response.Close();
                return null;
            }
            #endregion
        }
        #endregion
    }
    #endregion

    #region generic_http_request_BasicAuth

    public static object generic_http_request_BasicAuth(string return_type, CookieContainer container, string req_url, string req_referrer, string req_content_type, string req_accept,
        string req_method, string data_string, bool req_allow_redirect, bool addHeader, string str, string auth)
    {
        #region BuildRequest

        StringBuilder sb = new StringBuilder();
        String DataParameters;

        byte[] buf = new byte[8192];
        HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(req_url);
        request.Referer = req_referrer;

        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.4 (KHTML, like Gecko) Chrome/22.0.1229.79 Safari/537.4";

        request.KeepAlive = true;
        request.Method = req_method;
        request.ContentType = req_content_type;
        request.Accept = req_accept;
        request.CookieContainer = new CookieContainer();
        request.CookieContainer = container;
        request.Timeout = 20000;
        request.AllowAutoRedirect = req_allow_redirect;

        if (addHeader == true)
        {
            request.Headers.Add("Authorization", "Basic " + auth);
        }

        #endregion

        #region GetResponse

        if (req_method == "POST")
        {
            #region Write to data stream
            DataParameters = data_string;
            byte[] postData;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            postData = encoding.GetBytes(DataParameters);
            request.ContentLength = postData.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(postData, 0, postData.Length);
            }
            #endregion
        }

        // Execute the request
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            if (return_type == "string" || return_type == "collectionLocationDocument")
            {
                // We will read data via the response stream
                #region Read data
                using (Stream resStream = response.GetResponseStream())
                {
                    string tempString = null;
                    int count = 0;

                    do
                    {
                        // Fill the buffer with data
                        count = resStream.Read(buf, 0, buf.Length);

                        // Make sure we read some data
                        if (count != 0)
                        {
                            // Translate from bytes to ASCII text
                            tempString = Encoding.ASCII.GetString(buf, 0, count);

                            // Continue building the string
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0); // Any more data to read?
                }
                #endregion
            }
            request.Abort();

            #region Return result
            if (return_type == "string")
            {
                response.Close();

                return sb.ToString();

            }
            else if (return_type == "collection")
            {
                CookieCollection collection = new CookieCollection();
                collection = response.Cookies;
                response.Close();
                return collection;
            }
            else if (return_type == "collectionLocationDocument")
            {
                CollectionLocationDoc collLocDoc = new CollectionLocationDoc();
                collLocDoc.coll = new CookieCollection();
                collLocDoc.coll = response.Cookies;
                collLocDoc.doc = sb.ToString();
                collLocDoc.location = response.Headers["Location"];
                response.Close();
                return collLocDoc;
            }
            else
            {
                response.Close();
                return null;
            }
            #endregion
        }
        #endregion
    }

    #endregion

    #endregion

    #region Send Email

    #region sendEmailToCustomer

    public static void sendEmailToCustomer(string shipName, string consName, string pro, string ata, string shipID, string PO, string carName, string email, string mode)
    {
        string fromName = "Global Cargo Manager";
        string fromAddress = "cs" + AppCodeConstants.email_domain;
        string messageSubject = "Shipment for \"" + (string)shipName + "\" has been picked up by \"" + carName + "\"";

        string emailBody = "";

        if (mode == "demo")
        {
            emailBody += "This is a test email. If the program was live, the email would have been sent to: " + email +
                 " (if there's no email - there was no email address in the database)<br><br><br>";
        }

        emailBody += "Notice from Global Cargo Manager: Your shipment has been picked up and is in transit." + "<br>" +

        "Shipment ID: " + shipID.ToString() + "<br>" +
        "Shipper Name: " + (string)shipName + "<br>" +
        "Consignee: " + (string)consName + "<br>" +
        "PO/Ref #" + (string)PO + "<br>" +
        "Carrier: " + carName + "<br>" +
        "Pro Number: " + pro + "<br>" +
        "Estimated delivery date (as published on carrier's website): " + ata + " (Transit time is estimated unless your shipment was booked as guaranteed.)<br><br><br>" +
        "Call or email customer service with any questions 877-890-2295/cs" + AppCodeConstants.email_domain;
        
        if (mode == "demo")
        {
            MailUser(AppCodeConstants.Alex_email, "", emailBody, fromName, fromAddress, messageSubject, ""); 
        }
        else if (mode == "live")
        {
            MailUser(email, "", emailBody, fromName, fromAddress, messageSubject, "");            
        }
    }

    #endregion

    #region MailUser

    public static void MailUser(string email, string bugRpt, string prosUpdated, string fromName, string fromAddress, string messageSubject, string bccEmail)
    {
        try
        {
            string[] forSplit;
            if (email.Contains(".") == false) //validate email address
            {
                throw new Exception("Invalid email address: " + email);
            }
            //forSplit = email.Split('@');  //validate email address
            //if (forSplit.Length != 2)
            //{
            //    throw new Exception("Invalid email address: " + email);
            //}

            string mailServer = "", toAddressList = "", messageText = "";
            mailServer = AppCodeConstants.mail_server_ip;
            toAddressList = email;
            messageText += prosUpdated;
            messageText += bugRpt;

            System.Net.Mail.SmtpClient objClient = new System.Net.Mail.SmtpClient(mailServer);

            MailMessage objMessage = new System.Net.Mail.MailMessage();
            if (bccEmail != "")
            {
                //objMessage.Bcc.Add(bccEmail);
                forSplit = bccEmail.Split(' ');
                for (byte i = 0; i < forSplit.Length; i++)
                {
                    objMessage.Bcc.Add(forSplit[i].Trim());
                }
            }
            MailAddress objFrom = new System.Net.Mail.MailAddress(fromAddress, fromName);
           
            objMessage.From = objFrom;
            //objMessage.To.Add(objTo);
            forSplit = email.Split(' ');
            for (int i = 0; i < forSplit.Length; i++)
            {
                objMessage.To.Add(forSplit[i].Trim());
            }
            objMessage.Subject = messageSubject;
            objMessage.IsBodyHtml = true;
            objMessage.Body = messageText;

            objClient.Send(objMessage);
        }
        catch (Exception e)
        {
            try
            {
                //if (!e.Message.Contains("Invalid email address"))
                //{
                MailUser(AppCodeConstants.Alex_email, e.ToString(), bugRpt, fromName, fromAddress, messageSubject, "");
                //}
            }
            catch { }
        }
    }

    #endregion

    #region MailUser2

    public static void MailUser2(string email, string bugRpt, string prosUpdated, string fromName, string fromAddress, string messageSubject, string bccEmail,
        string attachment)
    {
        try
        {


            string[] forSplit;
            if (email.Contains(".") == false) //validate email address
            {
                throw new Exception("Invalid email address: " + email);
            }
            //forSplit = email.Split('@');  //validate email address
            //if (forSplit.Length != 2)
            //{
            //    throw new Exception("Invalid email address: " + email);
            //}

            string mailServer = "", toAddressList = "", messageText = "";
            mailServer = AppCodeConstants.mail_server_ip;//"smtp.gmail.com"
            toAddressList = email;
            messageText += prosUpdated;
            messageText += bugRpt;

            System.Net.Mail.SmtpClient objClient = new System.Net.Mail.SmtpClient(mailServer);

            MailMessage objMessage = new System.Net.Mail.MailMessage();
            //if (bccEmail != "")
            //{
            //    objMessage.Bcc.Add(bccEmail);
            //}

            forSplit = bccEmail.Split(' ');
            for (int i = 0; i < forSplit.Length; i++)
            {
                objMessage.Bcc.Add(forSplit[i].Trim());
            }

            MailAddress objFrom = new System.Net.Mail.MailAddress(fromAddress, fromName);
          
            objMessage.From = objFrom;
            //objMessage.To.Add(objTo);
            forSplit = email.Split(' ');
            for (int i = 0; i < forSplit.Length; i++)
            {
                objMessage.To.Add(forSplit[i].Trim());
            }
            objMessage.Subject = messageSubject;
            objMessage.IsBodyHtml = true;
            objMessage.Body = messageText;


            System.Net.Mail.Attachment pdfBOL;
            pdfBOL = new System.Net.Mail.Attachment(attachment);
            objMessage.Attachments.Add(pdfBOL);

            objClient.Send(objMessage);
        }
        catch (Exception e)
        {
            try
            {
                //if (!e.Message.Contains("Invalid email address"))
                //{
                MailUser(AppCodeConstants.Alex_email, e.ToString(), bugRpt, fromName, fromAddress, messageSubject, "");
                //}
            }
            catch { }
        }
    }

    #endregion
    
    #region IsValidEmail

    public static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #endregion

    #region UPS Package

    #region scrapeLoginToUPS

    //public static void scrapeLoginToUPS(ref CookieContainer container)
    //{
    //    #region Variables

    //    string url = "", referrer, contentType, accept, method, doc = "", data = "";

    //    #endregion

    //    string username = "", password = "";

    //    #region Login

    //    url = "http://www.ups.com/?Site=Corporate&cookie=us_en_home&setCookie=yes";
    //    referrer = "";
    //    contentType = "";
    //    accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
    //    method = "GET";
    //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);

    //    //-------------------------------------------------------------------------------------------------------------------

    //    referrer = url;
    //    url = "https://www.ups.com/one-to-one/login?loc=en_US&returnto=http%3A%2F%2Fwww.ups.com%2Fcontent%2Fus%2Fen%2Findex.jsx%3Fcookie%3Dus_en_home%26amp%3BSite%3DCorporate%26amp%3BsetCookie%3Dyes&USB=true";
    //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);

    //    #region getCSRFToken

    //    string[] tokens = new string[4];
    //    tokens[0] = "name=\"CSRFToken";
    //    tokens[1] = "value=";
    //    tokens[2] = "\"";
    //    tokens[3] = "\"";
    //    string CSRFToken = HelperFuncs.scrapeFromPage(tokens, doc);

    //    #endregion
    //    //-------------------------------------------------------------------------------------------------------------------

    //    referrer = url;
    //    url = "https://www.ups.com/one-to-one/login";
    //    contentType = "application/x-www-form-urlencoded";
    //    method = "POST";
    //    //data = "sysid=null&appid=null&lang=null&langc=null&method=null&returnto=http%253A%252F%252Fwww.ups.com%252Fcontent%252Fus%252Fen%252Findex.jsx%253Fcookie%253Dus_en_home%2526Site%253DCorporate%2526setCookie%253Dyes&loc=en_US&ioBlackBox=&ioElapsedTime=5007&connectWithFB=0" +
    //    //    "&uid=" + username + "&password=" + password + "&next=Log+In" +
    //    //    "&CSRFToken=" + CSRFToken;
    //    data = string.Concat("sysid=null&appid=null&lang=null&langc=null&method=null&returnto=http%253A%252F%252Fwww.ups.com%252Fcontent%252Fus%252Fen%252Findex.jsx%253Fcookie%253Dus_en_home%2526Site%253DCorporate%2526setCookie%253Dyes&loc=en_US&ioBlackBox=&ioElapsedTime=5007&connectWithFB=0",
    //        "&uid=", username, "&password=", password, "&next=Log+In",
    //        "&CSRFToken=", CSRFToken);

    //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, data, false);

    //    //-------------------------------------------------------------------------------------------------------------------

    //    url = "https://www.ups.com/one-to-one/finishlogin?returnto=http%3A%2F%2Fwww.ups.com%2Fcontent%2Fus%2Fen%2Findex.jsx%3Fcookie%3Dus_en_home%26Site%3DCorporate%26setCookie%3Dyes&loc=en_US&returnto=http%3A%2F%2Fwww.ups.com%2Fcontent%2Fus%2Fen%2Findex.jsx%3Fcookie%3Dus_en_home%26Site%3DCorporate%26setCookie%3Dyes";
    //    referrer = "";
    //    contentType = "";
    //    method = "GET";
    //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);

    //    //-------------------------------------------------------------------------------------------------------------------

    //    url = "http://www.ups.com/content/us/en/index.jsx?cookie=us_en_home&Site=Corporate&setCookie=yes";
    //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);

    //    //-------------------------------------------------------------------------------------------------------------------

    //    url = "https://www.ups.com/content/us/en/index.jsx?cookie=us_en_home&Site=Corporate&setCookie=yes";
    //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);

    //    //-------------------------------------------------------------------------------------------------------------------

    //    referrer = url;
    //    url = "https://www.ups.com/webservices/Login";
    //    contentType = "application/x-www-form-urlencoded";
    //    method = "POST";
    //    accept = "application/xml, text/xml, */*; q=0.01";
    //    data = "<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:v1='http://www.ups.com/XMLSchema/XOLTWS/UPSS/v1.0' xmlns:v11='http://www.ups.com/XMLSchema/XOLTWS/Login/v1.1' xmlns:v12='http://www.ups.com/XMLSchema/XOLTWS/Common/v1.0'><soapenv:Header><v1:UPSSecurity><v1:UsernameToken><v1:Username></v1:Username><v1:Password></v1:Password></v1:UsernameToken><v1:ServiceAccessToken><v1:AccessLicenseNumber></v1:AccessLicenseNumber></v1:ServiceAccessToken></v1:UPSSecurity></soapenv:Header><soapenv:Body><v11:GetAuthenticationTokenWithCookieRequest></v11:GetAuthenticationTokenWithCookieRequest></soapenv:Body></soapenv:Envelope>";
    //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, data, false);

    //    // Scrape Auth Token
    //    #region get Auth Token

    //    string[] tokens1 = new string[3];
    //    tokens1[0] = ":AuthenticationToken";
    //    tokens1[1] = ">";
    //    tokens1[2] = "<";
    //    string AuthToken = HelperFuncs.scrapeFromPage(tokens1, doc);

    //    #endregion

    //    // Scrape Licence Token
    //    #region Get Licence Token

    //    //tokens1[0] = "AccessLicenseNumber";         
    //    //string LicenseToken = HelperFuncs.scrapeFromPage(tokens1, doc);
    //    #endregion


    //    //-------------------------------------------------------------------------------------------------------------------

    //    url = "https://www.ups.com/webservices/DeliveryPlanner";
    //    //data = "<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:v1='http://www.ups.com/XMLSchema/XOLTWS/UPSS/v1.0' xmlns:v11='http://www.ups.com/XMLSchema/XOLTWS/DeliveryPlanner/v1.0' xmlns:v12='http://www.ups.com/XMLSchema/XOLTWS/Common/v1.0'><soapenv:Header><v1:UPSSecurity><v1:UsernameToken>" +
    //    //"<v1:AuthenticationToken>" + AuthToken +
    //    //    //"4PQNT7CCA5kJOh18HE6jTrJDb+FDbqLy7bZ1noDKtWffixhHY0JEsUtaV9Ti9yjZzODoAdYcMJKnyvPJNJOwU80JNEMnwHJkldSo9ipVZVPwMAS+NCfLkBpIWm0D5pbwi5" +
    //    //"</v1:AuthenticationToken></v1:UsernameToken><v1:ServiceAccessToken>" +
    //    //"<v1:AccessLicenseNumber>" + "" + "</v1:AccessLicenseNumber></v1:ServiceAccessToken></v1:UPSSecurity>" +
    //    //"</soapenv:Header><soapenv:Body><v11:DeliveryPlannerDataRequest><v12:Request><v12:RequestOption></v12:RequestOption><v12:TransactionReference><v12:CustomerContext>UPS HOME PAGE MC PLANNER</v12:CustomerContext><v12:TransactionIdentifier></v12:TransactionIdentifier></v12:TransactionReference></v12:Request><v11:RequestData><v11:ClientId>mcdphp</v11:ClientId><v11:DeliveryPlannerRequest>1</v11:DeliveryPlannerRequest><v11:DateRange>" +
    //    //"<v11:StartDate>20140614</v11:StartDate><v11:EndDate>20140827</v11:EndDate></v11:DateRange><v11:Locale>en_US</v11:Locale><v11:MaxNumberShipments>5</v11:MaxNumberShipments><v11:AddressToken></v11:AddressToken></v11:RequestData></v11:DeliveryPlannerDataRequest></soapenv:Body></soapenv:Envelope>";

    //    data = string.Concat("<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:v1='http://www.ups.com/XMLSchema/XOLTWS/UPSS/v1.0' xmlns:v11='http://www.ups.com/XMLSchema/XOLTWS/DeliveryPlanner/v1.0' xmlns:v12='http://www.ups.com/XMLSchema/XOLTWS/Common/v1.0'><soapenv:Header><v1:UPSSecurity><v1:UsernameToken>",
    //   "<v1:AuthenticationToken>", AuthToken,
    //   "</v1:AuthenticationToken></v1:UsernameToken><v1:ServiceAccessToken>",
    //   "<v1:AccessLicenseNumber>", "", "</v1:AccessLicenseNumber></v1:ServiceAccessToken></v1:UPSSecurity>",
    //   "</soapenv:Header><soapenv:Body><v11:DeliveryPlannerDataRequest><v12:Request><v12:RequestOption></v12:RequestOption><v12:TransactionReference><v12:CustomerContext>UPS HOME PAGE MC PLANNER</v12:CustomerContext><v12:TransactionIdentifier></v12:TransactionIdentifier></v12:TransactionReference></v12:Request><v11:RequestData><v11:ClientId>mcdphp</v11:ClientId><v11:DeliveryPlannerRequest>1</v11:DeliveryPlannerRequest><v11:DateRange>",
    //   "<v11:StartDate>20140614</v11:StartDate><v11:EndDate>20140827</v11:EndDate></v11:DateRange><v11:Locale>en_US</v11:Locale><v11:MaxNumberShipments>5</v11:MaxNumberShipments><v11:AddressToken></v11:AddressToken></v11:RequestData></v11:DeliveryPlannerDataRequest></soapenv:Body></soapenv:Envelope>");

    //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, data, false);

    //    //-------------------------------------------------------------------------------------------------------------------
    //    //-------------------------------------------------------------------------------------------------------------------
    //    //-------------------------------------------------------------------------------------------------------------------
    //    //-------------------------------------------------------------------------------------------------------------------

    //    url = "https://wwwapps.ups.com/ctc/request?loc=en_US&WBPM_lid=homepage/ct1.html_pnl_ctc";
    //    referrer = "https://www.ups.com/content/us/en/index.jsx";
    //    contentType = "";
    //    accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
    //    method = "GET";
    //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);

    //    //-------------------------------------------------------------------------------------------------------------------

    //    referrer = url;
    //    url = "https://wwwapps.ups.com/ctc/request?loc=en_US&WBPM_lid=homepage/ct1.html_pnl_ctc";
    //    contentType = "application/x-www-form-urlencoded";
    //    accept = "text/html, */*; q=0.01";
    //    method = "POST";
    //    data = "requestType=getUPSAccessPointEligibility&origCountryValue=US&destCountryValue=US";
    //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);

    //    //-------------------------------------------------------------------------------------------------------------------

    //    referrer = url;
    //    url = "https://wwwapps.ups.com/ctc/defaultService";
    //    data = "loc=en_US&origCtryCode=US&destCtryCode=US";
    //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);

    //    //-------------------------------------------------------------------------------------------------------------------

    //    #endregion
    //}

    #endregion

    #region UPS_PackageServiceToCode

    public static void UPS_PackageServiceToCode(ref string serviceName)
    {
        switch (serviceName)
        {
            case "UPS Ground":
                {
                    serviceName = "03";
                    return;
                }
            case "UPS 3 Day Select":
                {
                    serviceName = "12";
                    return;
                }
            case "UPS Next Day Air": // To do add the other UPS Next Day Air that has a different transit time
                {
                    serviceName = "01";
                    return;
                }
            case "UPS Next Day Air Saver":
                {
                    serviceName = "13";
                    return;
                }
            case "UPS 2nd Day Air":
                {
                    serviceName = "02";
                    return;
                }
            case "UPS 2nd Day Air A.M.":
                {
                    serviceName = "59";
                    return;
                }
            case "UPS Next Day Air Early A.M.": // To do add the other UPS Next Day Air that has a different transit time
                {
                    serviceName = "14";
                    return;
                }
            case "UPS First-Class Mail":
                {
                    serviceName = "M2";
                    return;
                }
            case "UPS Priority Mail":
                {
                    serviceName = "M3";
                    return;
                }
            case "UPS Expedited Mail Innovations": //to do add the other UPS Next Day Air that has a different transit time
                {
                    serviceName = "M4";
                    return;
                }

            default:
                {
                    serviceName = "UPS 3 Day Select"; //the middle ? to do see if need to throw exception here
                    HelperFuncs.writeToSiteErrors("UPS Package HelperFuncs", "unrecognized service name: " + serviceName, "");
                    return;
                }
        }
    }

    // Overloaded
    public static string UPS_PackageServiceToCode(string serviceName)
    {
        switch (serviceName)
        {
            case "UPS Ground":
                {
                    serviceName = "03";
                    return serviceName;
                }
            case "UPS 3 Day Select":
                {
                    serviceName = "12";
                    return serviceName;
                }
            case "UPS Next Day Air": // To do add the other UPS Next Day Air that has a different transit time
                {
                    serviceName = "01";
                    return serviceName;
                }
            case "UPS Next Day Air Saver":
                {
                    serviceName = "13";
                    return serviceName;
                }
            case "UPS 2nd Day Air":
                {
                    serviceName = "02";
                    return serviceName;
                }
            case "UPS 2nd Day Air A.M.":
                {
                    serviceName = "59";
                    return serviceName;
                }
            case "UPS Next Day Air Early A.M.": // To do add the other UPS Next Day Air that has a different transit time
                {
                    serviceName = "14";
                    return serviceName;
                }
            case "UPS First-Class Mail":
                {
                    serviceName = "M2";
                    return serviceName;
                }
            case "UPS Priority Mail":
                {
                    serviceName = "M3";
                    return serviceName;
                }
            case "UPS Expedited Mail Innovations": //to do add the other UPS Next Day Air that has a different transit time
                {
                    serviceName = "M4";
                    return serviceName;
                }

            default:
                {
                    serviceName = "UPS 3 Day Select"; //the middle ? to do see if need to throw exception here
                    HelperFuncs.writeToSiteErrors("UPS Package AES", "unrecognized service name: " + serviceName, "");
                    return serviceName;
                }
        }
    }

    #endregion

    #region SaveLabels_UPS_Package

    private static void SaveLabels_UPS_Package() // ref labelImgStrings // ref string[] base64GifList
    {
        //to do add try catch for session variable intShipmentID
        //string folderName = @"BOLReports";
        ////string subFolder = "Labels_" + HttpContext.Current.Session["UPS_Package_ShipID"].ToString(); // to do make sure this gets added when shipID is created

        ////string subFolder = "LabelsUPS_" + Session["svUserID"].ToString() + "_" + intShipmentID.ToString();
        //string subFolder = "LabelsUPS_" + Session["svUserID"].ToString() + "_" + intShipmentID.ToString();


        ////HelperFuncs.writeToSiteErrors("Labels subfolder", subFolder);

        ////HelperFuncs.writeToSiteErrors("Labels sess ship id", Session["intShipmentID"].ToString());

        //string pathString = System.IO.Path.Combine(folderName, subFolder);

        //System.IO.Directory.CreateDirectory(pathString);

        //if (isAAFES_Shipment.Equals(false))
        //{
        //    dynamicButtons.Text = "";
        //}

        //if (labelImgStrings != null)
        //{
        //    for (int i = 0; i < labelImgStrings.Length; i++)
        //    {
        //        SaveImage(pathString, labelImgStrings[i], "img" + i.ToString() + ".gif");
        //        //Session["UPS_Package_LabelPath"] = subFolder + "\\" + "img" + i.ToString() + ".gif";

        //        Session["UPS_Package_Label_" + i.ToString() + "_Path"] = subFolder + "\\" + "img" + i.ToString() + ".gif";

        //        if (isAAFES_Shipment.Equals(false))
        //        {
        //            dynamicButtons.Text += string.Concat("<input type=\"button\" id=\"btnLabel\"", i.ToString(), " runat=\"server\" ",
        //                "value=\"Click here to view UPS Parcel Label ", (i + 1).ToString(), "\" ",
        //                "onclick=\"javascript:openUpsLabelsInNewTabDemo(", i.ToString(), ");\" /><br>"); // Set literal text
        //        }
        //    }
        //}

        //if (isAAFES_Shipment.Equals(false))
        //{
        //    UPDispatch.Update(); // Update the UpdatatePanel
        //}

    }

    #endregion

    #region SaveImage

    //private void SaveImage(string newFolderName, string base64, string imgName)
    //{
    //    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
    //    {
    //        using (Bitmap bm2 = new Bitmap(ms))
    //        {
    //            bm2.Save(newFolderName + "\\" + imgName);
    //            imgUPS_Package.Add(newFolderName + "\\" + imgName);            
    //        }
    //    }
    //}

    #endregion

    #endregion

    #region savePDF_FromBase64String

    public static void savePDF_FromBase64String(ref string base64String, ref string path, ref string newFileName)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            //System.IO.FileStream stream = new FileStream(@"", FileMode.CreateNew);
            System.IO.FileStream stream = new FileStream(string.Concat(path, "\\", newFileName, ".pdf"), FileMode.CreateNew);
            System.IO.BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(bytes, 0, bytes.Length);
            writer.Close();
        }
        catch (Exception e)
        {
            string eStr = e.ToString();
            HelperFuncs.writeToSiteErrors("Could not save pdf", e.ToString());
        }
    }

    #endregion

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------

    #region Cost Additions

    public static double addSPC_Addition(double totalCharges)
    {
        //double accVal = 0;

        //if (totalCharges == 0)
        //{
        //    //do nothing
        //}
        //else if (totalCharges - accVal < 125)
        //{
        //    totalCharges += 25;
        //}
        //else if (totalCharges - accVal < 400)
        //{
        //    totalCharges += 50;
        //}
        //else if (totalCharges - accVal < 600)
        //{
        //    totalCharges += 75;
        //}
        //else
        //{
        //    totalCharges += 100;
        //}

        // Calculate 33% markup
        double markup = totalCharges * 0.33;

        HelperFuncs.writeToSiteErrors("spc live totalCharges", totalCharges.ToString());
        HelperFuncs.writeToSiteErrors("spc live markup", markup.ToString());

        if (markup < 40)
        {
            markup = 40;
        }
        else if (markup > 250)
        {
            markup = 250;
        }

        totalCharges += markup;
        HelperFuncs.writeToSiteErrors("spc live totalCharges with markup", totalCharges.ToString());

        return totalCharges;

    }

    public static double addClipperSubdomain_Addition(double totalCharges)
    {
        if (totalCharges == 0)
        {
            //do nothing
        }
        else if (totalCharges < 200)
        {
            totalCharges += 30;
        }
        else if (totalCharges < 400)
        {
            totalCharges += 50;
        }
        else if (totalCharges < 600)
        {
            totalCharges += 75;
        }
        else
        {
            totalCharges += 100;
        }

        return totalCharges;
    }
    #endregion

    #region Special Functions

    // Used by LTLRater.aspx.cs, Determines if is direct Pitt Ohio zone
    public static void isDirectPittOhio(ref bool isDirect, ref string origZip, ref string destZip)
    {
        SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aes_daylightSS"].ConnectionString);
        SqlCommand comm = new SqlCommand();
        try
        {
            //string direct = "";

            string sql = "SELECT Direct " +
           "FROM PittOhioDirect " +
           "WHERE originZip ='" + origZip + "' AND destZip ='" + destZip + "'";
            conn.Open();
            comm.Connection = conn;
            comm.CommandText = sql;
            SqlDataReader dr = comm.ExecuteReader();

            if (dr.Read())
            {
                //direct = (string)dr[0];
                if (dr["Direct"].ToString().Equals("Y"))
                {
                    isDirect = true;
                }
                else
                {
                }
            }
            //else
            //{
            //    throw new Exception("Could not find state by zipcode: " + zip);
            //}

            dr.Close();
            conn.Close();
            conn.Dispose();
            comm.Dispose();
        }
        catch (Exception cE)
        {
            try
            {
                HelperFuncs.writeToSiteErrors("isDirectPittOhio", cE.ToString());
                conn.Close();
                conn.Dispose();
                comm.Dispose();
            }
            catch
            {
            }
        }
    }

    public static string getMonth(int mon)
    {
        switch (mon)
        {
            case 1:
                return "January";
            case 2:
                return "February";
            case 3:
                return "March";
            case 4:
                return "April";
            case 5:
                return "May";
            case 6:
                return "June";
            case 7:
                return "July";
            case 8:
                return "August";
            case 9:
                return "September";
            case 10:
                return "October";
            case 11:
                return "November";
            case 12:
                return "December";
            default:
                return "January";
        }
    }

    public static string GetCountryByZip(string zip, bool origin, string oState, string dState)
    {
        try
        {
            string state;
            if (origin)
            {
                state = oState;
            }
            else
            {
                state = dState;
            }
            SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aesrater_dataConnectionStringSS"].ConnectionString);
            conn.Open();

            string sql = "select Country from SQL_ZIPS where Zip='" + zip + "' " + "and State='" + state.Trim() + "' ";
            HelperFuncs.writeToSiteErrors("get country sql", sql);

            SqlCommand comm = new SqlCommand("select Country from SQL_ZIPS where Zip='" + zip + "' " + "and State='" + state.Trim() + "' ", conn);


            SqlDataReader dr = comm.ExecuteReader();
            string strCountry = "US";
            if (dr.Read())
            {
                strCountry = dr["Country"].ToString();
            }

            dr.Close();
            comm.Dispose();
            conn.Close();
            conn.Dispose();

            return strCountry;
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("get country sql", e.ToString());
            return "";
        }
    }

    public static string insertBRs(string[] forSplit)
    {
        string tmp = "";
        for (int i = 0; i < forSplit.Length; i++)
        {
            tmp += forSplit[i];
            if (IsOdd(i))
            {
                tmp += "<br>";
            }
            else tmp += ' ';
        }
        return tmp;
    }

    public static double getZipCostAddition(string zip, int carID)
    {
        double costAdd = 0;
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009);
        SqlCommand comm = new SqlCommand();
        try
        {
            string sql = "SELECT CostAddition " +
           "FROM LTL_HIGH_COST_ZIPS " +
           "WHERE Zip ='" + zip + "' AND CarrierID=" + carID;
            conn.Open();
            comm.Connection = conn;
            comm.CommandText = sql;
            SqlDataReader dr = comm.ExecuteReader();

            if (dr.Read())
            {
                costAdd = (double)dr[0];
            }

            dr.Close();
            conn.Close();
            conn.Dispose();
            comm.Dispose();
            return costAdd;
        }
        catch (Exception cE)
        {
            try
            {
                conn.Close();
                conn.Dispose();
                comm.Dispose();
            }
            catch
            {
            }
            writeToSiteErrors("GetZipCostAddition (Live)", cE.ToString(), "");
            return costAdd;
        }
    }
    #endregion

    #region getCustomerCompByShipID
    //-----------------------------------------------------------------------------------------------------------------

    public struct shipCustCompInfo
    {
        public string zip, billZip, city, state, username, password, compID_Cust, compName;
    }

    public static void getCustomerCompByShipID(ref int shipID, ref shipCustCompInfo custCompInfo)
    {
        try
        {
            string sql = string.Concat("SELECT CompID_CUST, CompName, Zip, BillZip ",

                "FROM tbl_SHIPMENTS, tbl_COMPANY ",
                "WHERE ShipmentID=", shipID, " AND CompID_CUST=tbl_Company.CompID");

            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["CompID_CUST"] != DBNull.Value)
                            {
                                custCompInfo.compID_Cust = reader["CompID_CUST"].ToString();
                            }
                            if (reader["Zip"] != DBNull.Value)
                            {
                                custCompInfo.zip = reader["Zip"].ToString();
                            }
                            if (reader["BillZip"] != DBNull.Value)
                            {
                                custCompInfo.billZip = reader["BillZip"].ToString();
                            }
                            if (reader["CompName"] != DBNull.Value)
                            {
                                custCompInfo.compName = reader["CompName"].ToString();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            writeToSiteErrors("HelperFuncs getCustomerCompByShipID", e.ToString());
        }
    }

    //-----------------------------------------------------------------------------------------------------------------
    #endregion

    #region Booking Payment

  
    #endregion

    #region GetLoginsByCarID
    public struct Credentials
    {
        public string username, password, account;
    }

    public static List<Credentials> GetLoginsByCarID(int id)
    {
        List<Credentials> crds = new List<Credentials>();
        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData);
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        string sql = "";

        try
        {
            sql = "SELECT Username, Password, AccountNum " +
                "FROM tbl_CARRIER_ACCOUNTS " +
                "WHERE CarrierID = " + id.ToString() + " AND IsActive = 'True'";

            command.CommandText = sql;
            conn.Open();
            command.Connection = conn;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                Credentials crd = new Credentials();
                crd.username = reader["Username"].ToString();
                crd.password = reader["Password"].ToString();
                if (reader["AccountNum"] != DBNull.Value)
                    crd.account = reader["AccountNum"].ToString();
                else
                    crd.account = "";
                crds.Add(crd);
            }

            reader.Close();
            reader.Dispose();
            conn.Close();
            conn.Dispose();
            command.Dispose();
        }
        catch
        {
            try
            {
                conn.Close();
                conn.Dispose();
                command.Dispose();
            }
            catch { }
        }
        return crds;
    }
    #endregion

    #region getCompID

    public static int getCompID2(string username)
    {
        int compID = 0;
        List<int> compIDs = new List<int>();

        //HelperFuncs.writeToSiteErrors("aesComp", strAESCompID);

        try
        {
            string sql = "SELECT AESCompID " +
                    "FROM tbl_LOGIN " +
                    "WHERE UserName=" + "'" + username + "'" + " ORDER BY AESCompID";

            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = sql;
                    conn.Open();
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            if (reader["AESCompID"] != null && reader["AESCompID"] != DBNull.Value)
                            {
                                compID = (int)reader["AESCompID"]; //getting the last (highest number id of a group of id's) seems to work..
                                compIDs.Add((int)reader["AESCompID"]);

                            }
                        }
                    }
                }
            }

            //HelperFuncs.writeToSiteErrors("compid", compID.ToString());
            return compID;
        }
        catch (Exception e)
        {

            HelperFuncs.writeToSiteErrors("getCompID2", e.ToString());
            return compID;
        }

    }

    public static int getCompID(string username)
    {
        int compID = 0;
        List<int> compIDs = new List<int>();
        //string strAESCompID = GetAESAccountNumberByGCMLoginID((string)Session["svUserID"]);
        string strAESCompID = GetAESAccountNumberByGCMLoginID(username);

        //HelperFuncs.writeToSiteErrors("aesComp", strAESCompID);

        SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData);
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        try
        {
            string sql = "SELECT CompID " +
                    "FROM tbl_COMPANY " +
                    "WHERE AccountNbr = " + "'" + strAESCompID.Replace("'", "''") + "'" + " ORDER BY CompID";

            command.CommandText = sql;
            conn.Open();
            command.Connection = conn;
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (reader["CompID"] != null && reader["CompID"] != DBNull.Value)
                {
                    compID = (int)reader["CompID"]; //getting the last (highest number id of a group of id's) seems to work..
                    compIDs.Add((int)reader["CompID"]);

                }
            }


            reader.Close();
            reader.Dispose();
            conn.Close();
            conn.Dispose();
            command.Dispose();
            //HelperFuncs.writeToSiteErrors("compid", compID.ToString());
            return compID;
        }
        catch (Exception e)
        {
            try
            {
                conn.Close();
                conn.Dispose();
                command.Dispose();
            }
            catch
            {
            }
            HelperFuncs.writeToSiteErrors("Tracking", e.ToString(), "");
            return compID;
        }

    }

    #endregion

    #region getUsernameByCompID

    public static string getUsernameByCompID(int CompID)
    {

        string UserName = "";

        //HelperFuncs.writeToSiteErrors("getUsernameByCompID", CompID.ToString());

        try
        {
            string sql = string.Concat("SELECT UserName ",
                    "FROM tbl_LOGIN ",
                    "WHERE AESCompID=", CompID);

            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = sql;
                    conn.Open();
                    command.Connection = conn;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            if (reader["UserName"] != null && reader["UserName"] != DBNull.Value)
                            {
                                UserName = reader["UserName"].ToString();
                            }
                        }
                    }
                }
            }

            //HelperFuncs.writeToSiteErrors("UserName", UserName);
            //HelperFuncs.writeToSiteErrors("compid", compID.ToString());
            return UserName;
        }
        catch (Exception e)
        {

            HelperFuncs.writeToSiteErrors("getUsernameByCompID", e.ToString());
            return "";
        }

    }

    #endregion

    #region GetAESAccountNumberByGCMLoginID
    private static string GetAESAccountNumberByGCMLoginID(string strUserID)
    {

        SqlConnection connDom = new SqlConnection(AppCodeConstants.connStringAesData);


        SqlCommand commDom = new SqlCommand();
        commDom.Connection = connDom;

        string strSQL;
        ////////
        strSQL = "SELECT cmp.AccountNbr FROM tbl_Company cmp inner join tbl_LOGIN li on li.AESCompID=cmp.CompID WHERE li.UserName = '" + strUserID.Replace("'", "''") + "'";
        commDom.CommandText = strSQL;


        /////////////
        object obj;

        connDom.Open();
        try
        {
            obj = commDom.ExecuteScalar();
            connDom.Close();
            connDom.Dispose();
            commDom.Dispose();

            if (obj != null)
                return obj.ToString();
            else
                return "";
        }
        catch (Exception e)
        {
            try
            {
                connDom.Close();
                connDom.Dispose();
                HelperFuncs.writeToSiteErrors("Tracking", e.ToString(), "");
                return "";
            }
            catch (Exception d)
            {
                string str = d.ToString();
                HelperFuncs.writeToSiteErrors("Tracking", e.ToString(), "");
                return "";
            }
        }
    }
    #endregion

    #region GetOverlengthFee

    public static void GetOverlengthFee(ref LTLPiece[] m_lPiece, ref int overlengthFee, int inches_A, int inches_B, int inches_C, int sum_A, int sum_B, int sum_C)
    {
        #region Overlength

        for (int i = 0; i < m_lPiece.Length; i++)
        {
            // Length
            if (m_lPiece[i].Length > inches_A)
            {
                if (m_lPiece[i].Length > inches_B)
                {
                    if (m_lPiece[i].Length > inches_C)
                    {
                        overlengthFee = sum_C;
                    }
                    else if (overlengthFee < sum_B)
                    {
                        overlengthFee = sum_B;
                    }
                }
                else if (overlengthFee == 0)
                {
                    overlengthFee = sum_A;
                }
            }
            // Width
            if (m_lPiece[i].Width > inches_A)
            {
                if (m_lPiece[i].Width > inches_B)
                {
                    if (m_lPiece[i].Width > inches_C)
                    {
                        overlengthFee = sum_C;
                    }
                    else if (overlengthFee < sum_B)
                    {
                        overlengthFee = sum_B;
                    }
                }
                else if (overlengthFee == 0)
                {
                    overlengthFee = sum_A;
                }
            }
            // Height
            if (m_lPiece[i].Height > inches_A)
            {
                if (m_lPiece[i].Height > inches_B)
                {
                    if (m_lPiece[i].Height > inches_C)
                    {
                        overlengthFee = sum_C;
                    }
                    else if (overlengthFee < sum_B)
                    {
                        overlengthFee = sum_B;
                    }
                }
                else if (overlengthFee == 0)
                {
                    overlengthFee = sum_A;
                }
            }
        }

        #endregion
    }

    #endregion

    #region getUSF_OverlengthFee

    public static int getUSF_OverlengthFee(ref LTLPiece[] m_lPiece)
    {
        int overlengthFee = 0;
        bool is26 = false, is20 = false, is16 = false, is12 = false, is8 = false;

        #region Find the largest length

        for (int i = 0; i < m_lPiece.Length; i++)
        {
            if (m_lPiece[i].Length >= 312 || m_lPiece[i].Width >= 312 || m_lPiece[i].Height >= 312)
            {
                // Greater than equal to 26 feet
                is26 = true;
            }
            else if (m_lPiece[i].Length >= 240 || m_lPiece[i].Width >= 240 || m_lPiece[i].Height >= 240)
            {
                // Greater than equal to 20 feet
                is20 = true;
            }
            else if (m_lPiece[i].Length >= 192 || m_lPiece[i].Width >= 192 || m_lPiece[i].Height >= 192)
            {
                // Greater than equal to 16 feet
                is16 = true;
            }
            else if (m_lPiece[i].Length >= 144 || m_lPiece[i].Width >= 144 || m_lPiece[i].Height >= 144)
            {
                // Greater than equal to 12 feet
                is12 = true;
            }
            else if (m_lPiece[i].Length >= 96 || m_lPiece[i].Width >= 96 || m_lPiece[i].Height >= 96)
            {
                // Greater than equal to 8 feet
                is8 = true;
            }
            else
            {
                // Do nothing
            }
        }

        #endregion

        if (is26.Equals(true))
        {
            overlengthFee = 1100;
        }
        else if (is20.Equals(true))
        {
            overlengthFee = 400;
        }
        else if (is16.Equals(true))
        {
            overlengthFee = 225;
        }
        else if (is12.Equals(true))
        {
            overlengthFee = 150;
        }
        else if (is8.Equals(true))
        {
            overlengthFee = 85;
        }
        else
        {
            // Do nothing
        }

        return overlengthFee;
    }

    #endregion

    #region IsOverlength

    public static bool IsOverlength(ref LTLPiece[] m_lPiece, int maxLength, int maxWidth, int maxHeight)
    {

        for (int i = 0; i < m_lPiece.Length; i++)
        {
            // Length
            if (m_lPiece[i].Length > maxLength)
            {
                return true;
            }
            // Width
            if (m_lPiece[i].Width > maxWidth)
            {
                return true;
            }
            // Height
            if (m_lPiece[i].Height > maxHeight)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region IsOverlength

    public static bool IsOverlength_RPM(ref LTLPiece[] m_lPiece, int maxLength, int maxWidth, int maxHeight)
    {

        for (int i = 0; i < m_lPiece.Length; i++)
        {
            // Length
            if (m_lPiece[i].Length > maxLength)
            {
                return true;
            }
            // Width
            if (m_lPiece[i].Width > maxWidth)
            {
                return true;
            }
            // Height
            //if (m_lPiece[i].Height > maxHeight)
            //{
            //    return true;
            //}
        }

        return false;
    }

    #endregion

    #region GetMaxDimension

    public static void GetMaxDimension(ref LTLPiece[] m_lPiece, ref double maxDim)
    {

        for (int i = 0; i < m_lPiece.Length; i++)
        {
            // Length
            if (m_lPiece[i].Length > maxDim)
            {
                maxDim = m_lPiece[i].Length;
            }
            // Width
            if (m_lPiece[i].Width > maxDim)
            {
                maxDim = m_lPiece[i].Width;
            }
            // Height
            if (m_lPiece[i].Height > maxDim)
            {
                maxDim = m_lPiece[i].Height;
            }
        }

        //return maxDim;
    }

    #endregion

    #region GetMaxLengthDimension

    public static void GetMaxLengthDimension(ref LTLPiece[] m_lPiece, ref double maxDim)
    {

        for (int i = 0; i < m_lPiece.Length; i++)
        {
            // Length
            if (m_lPiece[i].Length > maxDim)
            {
                maxDim = m_lPiece[i].Length;
            }
            //// Width
            //if (m_lPiece[i].Width > maxDim)
            //{
            //    maxDim = m_lPiece[i].Width;
            //}
            //// Height
            //if (m_lPiece[i].Height > maxDim)
            //{
            //    maxDim = m_lPiece[i].Height;
            //}
        }

        //return maxDim;
    }

    #endregion

    #region setAccessorialsObject

    public static void setAccessorialsObject(ref QuoteData quoteData, ref AccessorialsObj accessorials, bool RESPU, bool RESDEL, bool CONPU, bool CONDEL,
        bool INSDEL, bool APTPU, bool APTDEL, bool TRADEPU, bool TRADEDEL, bool LGPU, bool LGDEL, bool MILIPU, bool MILIDEL, bool GOVPU, bool GOVDEL)
    {
        accessorials.RESPU = RESPU; // Residential Pickup
        accessorials.RESDEL = RESDEL; // Residential Delivery
        accessorials.CONPU = CONPU; // Construction Pickup
        accessorials.CONDEL = CONDEL; // Construction Delivery
        accessorials.INSDEL = INSDEL; // Inside Delivery
        accessorials.APTPU = APTPU; // Appointment Pickup
        accessorials.APTDEL = APTDEL; // Appointment Delivery
        accessorials.TRADEPU = TRADEPU; // Tradeshow Pickup
        accessorials.TRADEDEL = TRADEDEL; // Tradeshow Delivery
        accessorials.LGPU = LGPU; // Liftgate Pickup
        accessorials.LGDEL = LGDEL; // Liftgate Delivery

        accessorials.MILIPU = MILIPU; // Appointment Pickup
        accessorials.MILIDEL = MILIDEL; // Appointment Delivery
        accessorials.GOVPU = GOVPU; // Appointment Pickup
        accessorials.GOVDEL = GOVDEL; // Appointment Delivery

        if (RESPU || RESDEL || CONPU || CONDEL || INSDEL || APTPU || APTDEL || TRADEPU || TRADEDEL || LGPU || LGDEL 
            || MILIPU || MILIDEL || GOVPU || GOVDEL)
        {
            quoteData.hasAccessorials = true;
        }
    }
    
    #endregion

    #region setQuoteData

    public static void setQuoteData(string username, ref LTLPiece[] m_lPiece, out QuoteData quoteData,
        ref string origZip, ref string origCity, ref string origState,
        ref string destZip, ref string destCity, ref string destState, ref bool isHazmat, string pickupDate)
    {
        quoteData = new QuoteData();
        try
        {
            quoteData.username = username;

            quoteData.origZip = origZip;
            quoteData.origCity = origCity;
            quoteData.origState = origState;
            quoteData.origCountry = "";

            quoteData.destZip = destZip;
            quoteData.destCity = destCity;
            quoteData.destState = destState;
            quoteData.destCountry = "";

            quoteData.isHazmat = isHazmat;

            //quoteData.pickupDate = pickupDate;

            #region Dimensions
            quoteData.hasDimensions = true;
            for (byte i = 0; i < m_lPiece.Length; i++)
            {
                if (!(m_lPiece[i].Length > 0 && m_lPiece[i].Width > 0 && m_lPiece[i].Height > 0))
                {
                    quoteData.hasDimensions = false;
                    break;
                }
            }
            #endregion

            if (quoteData.hasDimensions.Equals(true))
            {
                #region If has dims set density
                quoteData.densities = new double[m_lPiece.Length];

                double cube;
                for (byte i = 0; i < m_lPiece.Length; i++)
                {
                    cube = ((m_lPiece[i].Length * m_lPiece[i].Width * m_lPiece[i].Height) / 1728) * m_lPiece[i].Quantity;
                    quoteData.densities[i] = m_lPiece[i].Weight / cube;


                    quoteData.totalWeight += m_lPiece[i].Weight;
                    quoteData.totalCube += cube;
                }

                quoteData.totalDensity = quoteData.totalWeight / quoteData.totalCube;

                // Test
                //writeToSiteErrors("quote data", "length: " + m_lPiece.Length + " totalWeight: " + quoteData.totalWeight + " totalCube: " + quoteData.totalCube +
                //    " totalDensity: " + quoteData.totalDensity);
                #endregion
            }
            else
            {
                for (byte i = 0; i < m_lPiece.Length; i++)
                {

                    quoteData.totalWeight += m_lPiece[i].Weight;

                }
                //writeToSiteErrors("quote data", "no dims");
            }
        }
        catch (Exception e)
        {
            writeToSiteErrors("quote data", e.ToString());
        }
    }
    
    #endregion

    #region fixQuoteDataDest

    public static void fixQuoteDataDest(ref QuoteData quoteData, ref string midDest, ref string midDestZip)
    {
        quoteData.destZip = midDestZip;
        string[] forSplit = midDest.Split(',');
        if (forSplit.Length.Equals(2))
        {
            quoteData.destCity = forSplit[0].Trim();
            quoteData.destState = forSplit[1].Trim();
        }
        else
        {
            writeToSiteErrors("fixQuoteDataDest midDest", midDest);
        }
    }

    #endregion

    #region fixQuoteDataOrig

    public static void fixQuoteDataOrig(ref QuoteData quoteData, ref string midOrig, ref string midOrigZip)
    {
        quoteData.origZip = midOrigZip;
        string[] forSplit = midOrig.Split(',');
        if (forSplit.Length.Equals(2))
        {
            quoteData.origCity = forSplit[0].Trim();
            quoteData.origState = forSplit[1].Trim();
        }
        else
        {
            writeToSiteErrors("fixQuoteDataOrig midOrig", midOrig);
        }
    }

    #endregion

    #region UseGoogleAPI_ToGetMilesBetweenCities
    public static void UseGoogleAPI_ToGetMilesBetweenCities(ref string origCity, ref string origState, ref string destCity, ref string destState, ref double miles)
    {
        try
        {
            //CookieContainer container = new CookieContainer();
            string url = string.Concat("http://maps.googleapis.com/maps/api/distancematrix/xml?origins=", origCity.Replace(" ", "+"), "+", origState,
                "&destinations=", destCity.Replace(" ", "+"), "+", destState, "&units=imperial&sensor=false");

            string doc = (string)HelperFuncs.generic_http_request("string", null, url, "", "", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
                "GET", "", false);

            // Scrape zone
            string[] tokens = new string[4];
            tokens[0] = "<distance>";
            tokens[1] = "<text>";
            tokens[2] = ">";
            tokens[3] = "<";

            double.TryParse(HelperFuncs.scrapeFromPage(tokens, doc).Replace("mi", "").Replace(",", "").Trim(), out miles);
        }
        catch (Exception e)
        {
            writeToSiteErrors("UseGoogleAPI_ToGetMilesBetweenCities", e.ToString());
        }
    }
    #endregion

    #region UseWebservicexAPI_ToGetTimeZoneByZipCode
    public static void UseWebservicexAPI_ToGetTimeZoneByZipCode(ref string zipCode, ref string timeZone)
    {
        try
        {
            string url = "http://www.webservicex.net/uszip.asmx/GetInfoByZIP";

            string doc = (string)HelperFuncs.generic_http_request("string", null, url, url, "application/x-www-form-urlencoded",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8", "POST", string.Concat("USZip=", zipCode), false);

            // Scrape zone
            string[] tokens = new string[3];
            tokens[0] = "<TIME_ZONE>";
            tokens[1] = ">";
            tokens[2] = "<";

            timeZone = HelperFuncs.scrapeFromPage(tokens, doc);
        }
        catch (Exception e)
        {
            writeToSiteErrors("UseWebservicexAPI_ToGetTimeZoneByZipCode", e.ToString());
        }
    }
    #endregion

    #region IsSalesRep
    
    public static bool IsSalesRep(ref string userID, ref string initials)
    {
        bool isSalesRep = false;

        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                string sql = string.Concat("SELECT SalesRep, Initials FROM tbl_Initials ",
                                           "WHERE Username='", userID, "'");

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["SalesRep"] != DBNull.Value)
                            {
                                isSalesRep = (bool)reader["SalesRep"];
                            }
                            if (reader["Initials"] != DBNull.Value)
                            {
                                initials = reader["Initials"].ToString();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            AAFES.writeToAAFES_Logs("isSalesRep", e.ToString());
        }

        return isSalesRep;
    }
    
    #endregion

    #region ShowDLSRates
    public static bool ShowDLSRates(string Username)
    {
        bool showDLSRates = false;

        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                string sql = string.Concat("SELECT DLSRates FROM tbl_Login ",
                                           "WHERE Username='", Username, "'");

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["DLSRates"] != DBNull.Value)
                            {
                                showDLSRates = (bool)reader["DLSRates"];
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("ShowDLSRates", e.ToString());
        }

        return showDLSRates;
    }
    #endregion

    #region IsDUR

    public static bool IsDUR(string Username)
    {
        bool myBool = false;

        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                string sql = string.Concat("SELECT Terms FROM tbl_Company ",
                                           "WHERE AccountNbr='", Username, "'");

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["Terms"] != DBNull.Value && reader["Terms"].ToString().Trim().Equals("DUR"))
                            {
                                myBool = true;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("IsDUR", e.ToString());
        }

        return myBool;
    }

    #endregion

    #region IsAssociationID_5

    public static bool IsAssociationID_5(string Username)
    {
        bool myBool = false;

        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                string sql = string.Concat("SELECT AssociationID FROM tbl_Company ",
                                           "WHERE AccountNbr='", Username, "'");

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["AssociationID"] != DBNull.Value && reader["AssociationID"].ToString().Trim().Equals("5"))
                            {
                                myBool = true;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("IsAssociationID_5", e.ToString());
        }

        return myBool;
    }

    #endregion

    #region GetAssociationID_5_SellRate

    public static void GetAssociationID_5_SellRate(string ourRate, ref double assoc5SellRate)
    {
        double dblCarrierQuoteAmount = 0.0;
        double assoc5RateWith33Percent = 0.0; // assoc5SellRate = 0.0, 

        if (ourRate != null && ourRate.Length > 0 &&
            double.TryParse(ourRate, out dblCarrierQuoteAmount))
        {
            //assoc5SellRate = dblRate;
            assoc5RateWith33Percent = Math.Round(dblCarrierQuoteAmount * 1.33, 2);
        }

        double assoc5markup = (assoc5RateWith33Percent - dblCarrierQuoteAmount);

        HelperFuncs.writeToSiteErrors("dblCarrierQuoteAmount", dblCarrierQuoteAmount.ToString());
        HelperFuncs.writeToSiteErrors("assoc5RateWith33Percent", assoc5RateWith33Percent.ToString());
        HelperFuncs.writeToSiteErrors("assoc5markup", assoc5markup.ToString());

        //HelperFuncs.writeToSiteErrors("", "");

        if (assoc5markup < 35)
        {
            assoc5SellRate = dblCarrierQuoteAmount + 35;
        }
        else if (assoc5markup > 175)
        {
            assoc5SellRate = dblCarrierQuoteAmount + 175;
        }
        else
        {
            assoc5SellRate = dblCarrierQuoteAmount + assoc5markup;
        }

        HelperFuncs.writeToSiteErrors("assoc5SellRate", Math.Round(assoc5SellRate, 2).ToString());
    }

    #endregion

    #region GetDLS_Markup

    public static void GetDLS_Markup(string Username, ref dlsMarkup markup)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                string sql = string.Concat("SELECT DLSMU, DLSMin$ ",
                    "FROM tbl_LOGIN WHERE Username='", Username, "'");

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["DLSMU"] != DBNull.Value)
                            {
                                markup.DLSMU = (int)reader["DLSMU"];
                            }
                            if (reader["DLSMin$"] != DBNull.Value)
                            {
                                markup.DLSMinDollar = (int)reader["DLSMin$"];
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("GetDLS_Markup", e.ToString());
        }

        //return showDLSRates;
    }

    // Overloaded
    public static void GetDLS_Markup(int CompID, ref dlsMarkup markup)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                string sql = string.Concat("SELECT DLSMU, DLSMin$ ",
                    "FROM tbl_LOGIN WHERE AESCompID=", CompID);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["DLSMU"] != DBNull.Value)
                            {
                                markup.DLSMU = (int)reader["DLSMU"];
                            }
                            if (reader["DLSMin$"] != DBNull.Value)
                            {
                                markup.DLSMinDollar = (int)reader["DLSMin$"];
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("GetDLS_Markup", e.ToString());
        }

        //return showDLSRates;
    }

    public struct dlsMarkup
    {
        public int DLSMU, DLSMinDollar;
    }

    #endregion

    #region GetCarrierQuoteNumByShipmentID

    public static void GetCarrierQuoteNumByShipmentID(ref int ShipmentID, ref string CarrierQuoteNum)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                #region SQL

                string sql = string.Concat("SELECT CarrierQuoteNum ",

                    "FROM tbl_SEGMENTS ",

                    "WHERE ShipmentID=", ShipmentID);

                #endregion

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["CarrierQuoteNum"] != DBNull.Value)
                            {
                                CarrierQuoteNum = reader["CarrierQuoteNum"].ToString();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("GetCarrierQuoteNumByShipmentID", e.ToString());
        }
    }

    #endregion

    #region updateCarrierQuoteNum

    public static void updateCarrierQuoteNum(ref int ShipmentID, ref string CarrierQuoteNum)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                string sql = string.Concat("UPDATE tbl_SEGMENTS SET CarrierQuoteNum='", CarrierQuoteNum, "' ",
                                           "WHERE ShipmentID=", ShipmentID);

                writeToSiteErrors("updateCarrierQuoteNum", sql);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception e)
        {
            writeToSiteErrors("updateCarrierQuoteNum", e.ToString());
        }
    }

    #endregion

    #region PoSystem

    #region insertIntoItemsPoSystem
    public static void insertIntoItemsPoSystem(ref PoSystem.Info info, ref int ShipmentID)
    {
        try
        {
            string sql = "", cubeText = "";
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                conn.Open();
                for (byte i = 0; i < info.weight.Count; i++)
                {
                    //if (aafesItemsInfoList[i].Cube > 0)
                    //{
                    //    cubeText = string.Concat(aafesItemsInfoList[i].Cube.ToString(), " Cube");
                    //}
                    //else
                    //{
                    //    cubeText = "";
                    //}

                    cubeText = "";

                    sql = string.Concat("INSERT INTO tbl_ITEMS(ShipmentID,Class,Descr,Kind,Nmfc,Pcs,Units,WtLBS,VolCF,DimsHt,DimsL,DimsW)",
                                               " VALUES(", ShipmentID, ",", Math.Round(info.fClass[i], 1), ",'",
                    info.commodity[i], " ", cubeText, "',",
                    "'',",
                    Math.Round(info.nmfc[i], 2), ",",
                    info.pieces[i], ",",
                    "0", ",",
                    info.weight[i], ",",
                    info.cube[i], ",",

                    info.length[i], ",",
                    info.width[i], ",",
                    info.height[i],

                    ")");

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (Exception e1)
        {
            AAFES.writeToAAFES_Logs("insertIntoItemsPoSystem", e1.ToString());
        }
    }
    #endregion

    #region IsFreeFreight
    public static void IsFreeFreight(int poNum, ref bool isFreeFreight)
    {
        try
        {

            isFreeFreight = false;
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
            {
                #region SQL

                string sql = string.Concat("SELECT FreeFreight ",

                    "FROM Podata ",

                    "WHERE id=", poNum);

                #endregion

                //byte counter = 0;

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["FreeFreight"] != DBNull.Value)
                            {
                                isFreeFreight = (bool)reader["FreeFreight"];
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("IsFreeFreight", e.ToString());
        }
    }
    #endregion

    #region IsSPCSubdomain
    public static void IsSPCSubdomain(string UserName, ref bool isSPCSubdomain)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                #region SQL

                string sql = string.Concat("SELECT Subdomain ",

                    "FROM tbl_Login ",

                    "WHERE UserName='", UserName, "'");

                #endregion

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["Subdomain"] != DBNull.Value)
                            {
                                if (reader["Subdomain"].ToString().ToLower().Trim().Equals("spc"))
                                {
                                    isSPCSubdomain = true;
                                }
                                HelperFuncs.writeToSiteErrors("Subdomain", reader["Subdomain"].ToString().ToLower().Trim());
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("IsSPCSubdomain", e.ToString());
        }
    }
    #endregion

    #endregion

    #region GetQuoteInfoByQuoteID
    
    public static void GetQuoteInfoByQuoteID(ref int QuoteID, ref string oZip, ref string dZip, ref DateTime Day, ref DateTime Time)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
            {
                #region SQL

                string sql = string.Concat("SELECT Origin, Destination, Day, Time ",

                    "FROM SQL_STATS_GCM ",

                    "WHERE ID=", QuoteID);

                #endregion

                //byte counter = 0;

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["Origin"] != DBNull.Value)
                            {
                                oZip = reader["Origin"].ToString();
                            }

                            if (reader["Destination"] != DBNull.Value)
                            {
                                dZip = reader["Destination"].ToString();
                            }

                            if (reader["Day"] != DBNull.Value)
                            {
                                Day = (DateTime)reader["Day"];
                            }

                            if (reader["Time"] != DBNull.Value)
                            {
                                Time = (DateTime)reader["Time"];
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("GetQuoteInfoByQuoteID", e.ToString());
        }
    }
    
    #endregion

    #region getCityStateByZip
    public static string[] getCityStateByZip(string zip)
    {
        string[] cityState = new string[2];
        cityState[0] = "";
        cityState[1] = "";
        //int zipInt;
        SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aesrater_dataConnectionStringSS"].ConnectionString);
        SqlCommand comm = new SqlCommand();
        try
        {
            string sql = "";

            sql = "SELECT City, State " +
           "FROM SQL_ZIPS " +
           "WHERE Zip =" + "'" + zip + "' AND (Country='US' OR Country = 'CANADA')";
            conn.Open();
            comm.Connection = conn;
            comm.CommandText = sql;
            SqlDataReader dr = comm.ExecuteReader();

            if (dr.Read())
            {
                cityState[0] = (string)dr["City"];
                cityState[1] = (string)dr["State"];
            }
            else
            {
                HelperFuncs.writeToSiteErrors("getCityStateByZip", "zip " + zip + " was not found");
            }

            dr.Close();
            conn.Close();
            conn.Dispose();
            comm.Dispose();
            return cityState;
        }
        catch (Exception cE)
        {
            string str = cE.ToString();
            try
            {
                conn.Close();
                conn.Dispose();
                comm.Dispose();
            }
            catch
            {
            }

            return cityState;
        }
    }
    #endregion

    #region ClearviewPO_LoginLogic

    //public static void ClearviewPO_LoginLogic(string user, ref bool isClearviewPO)
    //{
    //    int compID = HelperFuncs.getCompID2(user);
    //    bool isCompClearviewPO = false;
    //    PoSystem.IsCompClearviewPO(ref compID, ref isCompClearviewPO);
    //    if (isCompClearviewPO.Equals(true))
    //    {
    //        //defaultPageName = "poSystem/poData.aspx";
    //        //HelperFuncs.writeToSiteErrors("ltlrater demo", "clearview");
    //    }
    //    else
    //    {
    //        //HelperFuncs.writeToSiteErrors("ltlrater demo", "not clearview");
    //        //this.Master.FindControl("menuPO").Visible = false;
    //        isClearviewPO = true;
    //    }
    //}

    #endregion

    #region InsertIntoRateQuotes

    public static void InsertIntoRateQuotes(ref int newRateQuoteID, ref int SEGMENTID, string NetCharge, string Initials, double AESQuote)
    {
        try
        {

            #region InsertIntoRateQuotes

            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {

                string sql = string.Concat("INSERT INTO tbl_RATEQUOTES(SEGMENTID,NetCharge,Initials, AESQuote, DURRRDsell) ",
                                                   "OUTPUT INSERTED.RATEQUOTEID ",
                                                   "VALUES(",
                                                   SEGMENTID, ",", NetCharge, ",'", Initials, "',", AESQuote, ",",
                                                   Math.Round(AESQuote * 1.25, 2), ")");


                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    //command.ExecuteNonQuery();
                    newRateQuoteID = (int)command.ExecuteScalar();
                }

                HelperFuncs.writeToSiteErrors("InsertIntoRateQuotes newRateQuoteID, sql", newRateQuoteID.ToString() + ", " + sql);
            }

            #endregion
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("InsertIntoRateQuotes", e.ToString());
        }
    }

    #endregion

    #region InsertIntoRateQuotes

    public static void InsertIntoRateQuotes(ref int newRateQuoteID, ref int SEGMENTID, string NetCharge, string Initials, double AESQuote,
        ref double ourRate, string requester)
    {
        try
        {

            double DURRRDsell = 0.0;

            if (requester.Equals("NetNet"))
            {
                DURRRDsell = ourRate;
            }
            else
            {
                DURRRDsell = Math.Round(AESQuote * 1.25, 2);
            }

            #region InsertIntoRateQuotes

            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {

                string sql = string.Concat("INSERT INTO tbl_RATEQUOTES(SEGMENTID,NetCharge,Initials, AESQuote, DURRRDsell) ",
                                                   "OUTPUT INSERTED.RATEQUOTEID ",
                                                   "VALUES(",
                                                   SEGMENTID, ",", NetCharge, ",'", Initials, "',", AESQuote, ",",
                                                   DURRRDsell, ")");


                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    //command.ExecuteNonQuery();
                    newRateQuoteID = (int)command.ExecuteScalar();
                }

                HelperFuncs.writeToSiteErrors("InsertIntoRateQuotes newRateQuoteID, sql", newRateQuoteID.ToString() + ", " + sql);
            }

            #endregion
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("InsertIntoRateQuotes", e.ToString());
        }
    }

    #endregion

    #region GetAcctMgrID

    public static void GetAcctMgrID(string Username, out int AcctMgrID)
    {
        AcctMgrID = 0;
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                string sql = string.Concat("SELECT AcctMgrID ",
                    "FROM tbl_COMPANY ",
                    "LEFT JOIN tbl_LOGIN AS login ON login.AESCompID=tbl_COMPANY.CompID ",
                    "WHERE Username='", Username, "'");

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["AcctMgrID"] != DBNull.Value)
                            {
                                AcctMgrID = (int)reader["AcctMgrID"];
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("GetAcctMgrID", e.ToString());
        }
    }

    #endregion

    #region getStateByZip

    public static string getStateByZip(string zip)
    {
        //string State = "";
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
            {
                string sql = string.Concat("SELECT State FROM SQL_ZIPS WHERE Zip='", zip, "' AND Country='US'");

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["State"] != DBNull.Value)
                            {
                                //State = reader["State"].ToString();
                                return reader["State"].ToString();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("getStateByZip", e.ToString());
        }

        //return state;
        return string.Empty;
    }

    #endregion

    #region XmlEscape

    public static string XmlEscape(string unescaped)
    {
        XmlDocument doc = new XmlDocument();
        XmlNode node = doc.CreateElement("root");
        node.InnerText = unescaped;
        return node.InnerXml;
    }

    #endregion

    #region XmlUnescape

    public static string XmlUnescape(string escaped)
    {
        XmlDocument doc = new XmlDocument();
        XmlNode node = doc.CreateElement("root");
        node.InnerXml = escaped;
        return node.InnerText;
    }

    #endregion

    #region GetExternalComments

    public static void GetExternalComments(ref StringBuilder htmlTable, string ShipmentID)
    {
        try
        {
            byte counter = 0;
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                string sql = string.Concat("SELECT ID, Initials, DateStamp, TimeStamp, Comment FROM ExternalComments ",
                                           "WHERE ShipmentID=", ShipmentID, " ORDER BY DateStamp");

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            counter++;
                            HelperFuncs.writeToSiteErrors("GetExternalComments counter", counter.ToString());

                            htmlTable.Append("<tr>");

                            htmlTable.Append("<td>");
                            if (reader["DateStamp"] != DBNull.Value)
                            {
                                htmlTable.Append(((DateTime)reader["DateStamp"]).ToShortDateString());
                                //htmlTable.Append(string.Concat("<input type='text' name='firstname' value='", ((DateTime)reader["DateStamp"]).ToShortDateString(), "'>"));
                            }
                            if (reader["TimeStamp"] != DBNull.Value)
                            {
                                htmlTable.Append(string.Concat("&nbsp;", ((DateTime)reader["TimeStamp"]).ToShortTimeString().Replace(" ", "&nbsp;")));
                            }
                            htmlTable.Append("</td>");


                            htmlTable.Append(string.Concat("<td class='editClass' id='", reader["ID"].ToString(), "' onfocusout='commentBlur(", reader["ID"].ToString(), ",", counter, ");'>"));
                            if (reader["Comment"] != DBNull.Value)
                            {
                                int numOfWords = reader["Comment"].ToString().Split(' ').Length;
                               
                                //isSalesRep = (bool)reader["SalesRep"];
                                htmlTable.Append(string.Concat("<textarea disabled id='txtComment", counter, "' rows=\"", numOfWords, "\" cols=\"10\">", reader["Comment"].ToString(), "</textarea>"));
                            }
                            htmlTable.Append("</td>");

                            htmlTable.Append("<td>");
                            htmlTable.Append(string.Concat("<button type='button' onclick='editComment(", reader["ID"].ToString(), ",", counter, ");'>Edit</button>"));

                            htmlTable.Append("</td>");

                            htmlTable.Append("<td>");
                            htmlTable.Append(string.Concat("<button type='button' onclick='deleteComment(", reader["ID"].ToString(), ",", counter, ");'>Delete</button>"));

                            htmlTable.Append("</td>");

                            htmlTable.Append("<td>");
                            if (reader["Initials"] != DBNull.Value)
                            {
                                htmlTable.Append(reader["Initials"].ToString());
                            }
                            htmlTable.Append("</td>");

                            htmlTable.Append("</tr>");
                            
                            #region Not used

                            /*
                            htmlTable.Append("<li>");

                            //htmlTable.Append("<td>");
                            if (reader["DateStamp"] != DBNull.Value)
                            {
                                htmlTable.Append(((DateTime)reader["DateStamp"]).ToShortDateString());
                                //htmlTable.Append(string.Concat("<input type='text' name='firstname' value='", ((DateTime)reader["DateStamp"]).ToShortDateString(), "'>"));
                            }
                            if (reader["TimeStamp"] != DBNull.Value)
                            {
                                htmlTable.Append(string.Concat(" ", ((DateTime)reader["TimeStamp"]).ToShortTimeString()));
                            }
                            //htmlTable.Append("</td>");


                            //htmlTable.Append(string.Concat("<span class='editClass' id='", reader["ID"].ToString(), "' onfocusout='commentBlur(", reader["ID"].ToString(), ");'>"));
                            if (reader["Comment"] != DBNull.Value)
                            {
                                //isSalesRep = (bool)reader["SalesRep"];
                                htmlTable.Append(string.Concat("<textarea disabled id='txtComment", counter, "' rows=\"2\" cols=\"50\">", reader["Comment"].ToString(), "</textarea>"));
                            }
                           // htmlTable.Append("</span>");

                            //htmlTable.Append("<td>");
                            htmlTable.Append(string.Concat("<button type='button' onclick='editComment(", reader["ID"].ToString(), ");'>Edit</button>"));

                            //htmlTable.Append("</td>");


                            //htmlTable.Append("<td>");
                            if (reader["Initials"] != DBNull.Value)
                            {
                                htmlTable.Append(reader["Initials"].ToString());
                            }
                            //htmlTable.Append("</td>");

                            htmlTable.Append("</li>");
                            */

                            #endregion
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("GetExternalComments", e.ToString());
        }
    }

    #endregion

    #region GetExternalCommentsForTrackingPage

    public static void GetExternalCommentsForTrackingPage(ref StringBuilder htmlTable, string ShipmentID)
    {
        try
        {
            byte counter = 0;
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                string sql = string.Concat("SELECT ID, Initials, DateStamp, TimeStamp, Comment FROM ExternalComments ",
                                           "WHERE ShipmentID=", ShipmentID, " ORDER BY DateStamp");

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            counter++;

                            htmlTable.Append("<tr bgcolor='#FFFFCC'>");

                            htmlTable.Append("<td colspan='3' style='border-top: 0;'>");
                            if (reader["DateStamp"] != DBNull.Value)
                            {
                                htmlTable.Append(((DateTime)reader["DateStamp"]).ToShortDateString());
                                //htmlTable.Append("<br>");
                                //htmlTable.Append(string.Concat("<input type='text' name='firstname' value='", ((DateTime)reader["DateStamp"]).ToShortDateString(), "'>"));
                            }
                            if (reader["TimeStamp"] != DBNull.Value)
                            {
                                htmlTable.Append(string.Concat(" ", ((DateTime)reader["TimeStamp"]).ToShortTimeString()));
                            }
                            htmlTable.Append("</td>");

                            htmlTable.Append(string.Concat("<td class='editClass' colspan='7' style='border-top: 0; font-size: 75%;' id='", reader["ID"].ToString(), "' onfocusout='commentBlur(", reader["ID"].ToString(), ");'>"));
                            if (reader["Comment"] != DBNull.Value)
                            {
                                
                                //htmlTable.Append(string.Concat("<textarea disabled id='txtComment", counter, "' rows=\"2\" cols=\"50\">", reader["Comment"].ToString(), "</textarea>"));
                                htmlTable.Append(reader["Comment"].ToString());
                            }
                            htmlTable.Append("</td>");

                            //htmlTable.Append("<td>");
                            //htmlTable.Append(string.Concat("<button type='button' onclick='editComment(", reader["ID"].ToString(), ");'>Edit</button>"));

                            //htmlTable.Append("</td>");


                            htmlTable.Append("<td colspan='1' style='border-top: 0;'>");
                            if (reader["Initials"] != DBNull.Value)
                            {
                                htmlTable.Append(reader["Initials"].ToString());
                            }
                            htmlTable.Append("</td>");

                            htmlTable.Append("</tr>");
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("GetExternalComments", e.ToString());
        }
    }

    #endregion

    #region InTestSiteMode

    public static bool InTestSiteMode()
    {
        bool myBool = false;

        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringTestSite))
            {
                string sql = string.Concat("SELECT InTestSiteMode FROM Migration ",
                                           "WHERE ID=1");

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["InTestSiteMode"] != DBNull.Value)
                            {
                                myBool = (bool)reader["InTestSiteMode"];
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("InTestSiteMode", e.ToString());
        }

        return myBool;
    }

    #endregion
}
