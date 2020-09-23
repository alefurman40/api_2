#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;
using gcmAPI.NewPennRateService;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class NewPenn
    {
        QuoteData quoteData;

        #region Constructor
        // Constructor
        public NewPenn(ref QuoteData quoteData)
        {

            this.quoteData = quoteData;

        }

        #endregion

        #region Get_rates

        public void Get_rates(out GCMRateQuote NewPenn_Quote_Genera)
        {
            try
            {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                #region Not used

                //RateQuote rq = new RateQuote();

                //RateQuote[] res = request.quote("", "", "", "GA","Atlanta","30303","WA","Seattle","98144",
                //    "QuoteTerms","50","","","","","500","","","","","test po","quoteCashOnDelivery","accessorial1", "accessorial2",
                //    "accessorial3", "accessorial4", "accessorial5", "accessorial6","quoteHazard","quoteFreeze","GuaranteedDelivery",
                //    "OverLength","OverWidth","Skids","Drums");
                

                #endregion

                #region Weight and Class

                string weight1 = "", weight2 = "", weight3 = "", weight4 = "";
                string fClass1 = "", fClass2 = "", fClass3 = "", fClass4 = "";

                for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
                {
                    if (quoteData.m_lPiece[i].Weight > 0)
                    {
                        if (i == 0)
                        {
                            weight1 = quoteData.m_lPiece[i].Weight.ToString();
                            fClass1 = quoteData.m_lPiece[i].FreightClass;

                        }
                        else if (i == 1)
                        {
                            weight2 = quoteData.m_lPiece[i].Weight.ToString();
                            fClass2 = quoteData.m_lPiece[i].FreightClass;
                        }
                        else if (i == 2)
                        {
                            weight3 = quoteData.m_lPiece[i].Weight.ToString();
                            fClass3 = quoteData.m_lPiece[i].FreightClass;
                        }
                        else if (i == 3)
                        {
                            weight4 = quoteData.m_lPiece[i].Weight.ToString();
                            fClass4 = quoteData.m_lPiece[i].FreightClass;
                        }
                    }
                }

                if (fClass1 == "92.5")
                {
                    fClass1 = "925";
                }
                else if (fClass1 == "77.5")
                {
                    fClass1 = "775";
                }

                if (fClass2 == "92.5")
                {
                    fClass2 = "925";
                }
                else if (fClass2 == "77.5")
                {
                    fClass2 = "775";
                }

                if (fClass3 == "92.5")
                {
                    fClass3 = "925";
                }
                else if (fClass3 == "77.5")
                {
                    fClass3 = "775";
                }

                if (fClass4 == "92.5")
                {
                    fClass4 = "925";
                }
                else if (fClass4 == "77.5")
                {
                    fClass4 = "775";
                }

                #endregion

                #region Accessorials

                List<string> accessorials = new List<string>();
                Get_accessorials(ref accessorials);

                if (accessorials.Count > 6)
                {
                    throw new Exception("Too many accessorials");
                }
                string accessorial1 = "", accessorial2 = "", accessorial3 = "", accessorial4 = "", accessorial5 = "", accessorial6 = "";

                for(byte i=0;i< accessorials.Count;i++)
                {
                    if(i==0)
                    {
                        accessorial1 = accessorials[i];
                    }
                    else if (i == 1)
                    {
                        accessorial2 = accessorials[i];
                    }
                    else if (i == 2)
                    {
                        accessorial3 = accessorials[i];
                    }
                    else if (i == 3)
                    {
                        accessorial4 = accessorials[i];
                    }
                    else if (i == 4)
                    {
                        accessorial5 = accessorials[i];
                    }
                    else if (i == 5)
                    {
                        accessorial6 = accessorials[i];
                    }
                }
               
                string hazMat = "";
                if (quoteData.isHazmat)
                {
                    hazMat = "HAZ";
                }

                #endregion

                RequestRateQuoteV6Service request = new RequestRateQuoteV6Service();
                RateQuote[] res = request.quote(AppCodeConstants.new_penn_genera_un, AppCodeConstants.new_penn_genera_pwd,
                    AppCodeConstants.new_penn_genera_acct, "NJ", "MONROE TOWNSHIP", "08831",

                    //"VT", "BURLINGTON", "05401",
                    quoteData.destState, quoteData.destCity, quoteData.destZip,
                    "PPD",

                      //"50", "", "", "", "", 
                      //"500", "", "", "", "", 

                      fClass1, fClass2, fClass3, fClass4, "",
                    weight1, weight2, weight3, weight4, "",

                    "test po", "", accessorial1, accessorial2,
                    accessorial3, accessorial4, accessorial5, accessorial6, hazMat, "", "",
                    "", "", "", "");

                //RateQuote[] res = request.quote("", "", "", 
                //"GA","Atlanta","30303",
                //"WA","Seattle","98144",
                //    "QuoteTerms","50","","","","","500","","","","","test po","quoteCashOnDelivery","accessorial1", "accessorial2",
                //    "accessorial3", "accessorial4", "accessorial5", "accessorial6","quoteHazard","quoteFreeze","GuaranteedDelivery",
                //    "OverLength","OverWidth","Skids","Drums");
                //HAZ
                string quoteNetCharges = res[0].quoteNetCharges;

                string quoteTransitDays = res[0].quoteTransitDays;

                NewPenn_Quote_Genera = new GCMRateQuote();
                double TotalPrice;
                int DeliveryDays;
                if (double.TryParse(quoteNetCharges, out TotalPrice))
                {
                    NewPenn_Quote_Genera.TotalPrice = TotalPrice;
                    if (int.TryParse(quoteNetCharges, out DeliveryDays))
                    {
                        NewPenn_Quote_Genera.DeliveryDays = DeliveryDays;
                    }
                    else
                    {
                        NewPenn_Quote_Genera.DeliveryDays = 5;
                    }

                    NewPenn_Quote_Genera.DisplayName = "New Penn - Genera";
                    NewPenn_Quote_Genera.CarrierKey = "UPS";
                    NewPenn_Quote_Genera.BookingKey = "#1#";
                    NewPenn_Quote_Genera.Scac = "NPME";
                }

                //DB.Log("New Penn", "got result");
            }
            catch (Exception e)
            {
                NewPenn_Quote_Genera = new GCMRateQuote();
                DB.Log("New Penn", e.ToString());
            }
        }

        #endregion

        #region Get_accessorials
        private void Get_accessorials(ref List<string> accessorials)
        {
            if (quoteData.hasAccessorials == false && quoteData.isHazmat == false)
            {
                return;
            }


            /*
                SSM -- Single Shipment,
NCM – Delivery Appointment
GDM -- NYC Garment Area
HYM – Liftgate Pickup
IPM – Inside Pickup
PRU – Residential Pickup
LAP – Limited Access Pickup
HTM – Liftgate Delivery
IDM -- Inside Delivery
PRD – Residential Delivery
LAD – Limited Access Delivery
                */

            if (quoteData.AccessorialsObj.RESPU)
            {
                accessorials.Add("PRU");
            }
            if (quoteData.AccessorialsObj.RESDEL)
            {
                accessorials.Add("PRD");
            }
            if (quoteData.AccessorialsObj.LGPU)
            {
                accessorials.Add("HYM");
            }
            if (quoteData.AccessorialsObj.LGDEL)
            {
                accessorials.Add("HTM");
            }
            if (quoteData.AccessorialsObj.CONPU)
            {
                accessorials.Add("LAP");
            }
            if (quoteData.AccessorialsObj.CONDEL)
            {
                accessorials.Add("LAD");
            }
            if (quoteData.AccessorialsObj.INSDEL)
            {
                accessorials.Add("IDM");
            }
            if (quoteData.AccessorialsObj.APTDEL)
            {
                accessorials.Add("NCM");
            }

            return;

        }

        #endregion
    }
}