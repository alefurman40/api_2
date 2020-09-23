#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using gcmAPI.Models.Utilities;
using static gcmAPI.Models.Utilities.Mail;

#endregion

namespace gcmAPI.Models.LTL
{
    public class Parser
    {
        #region Get_url_string_from_xml

        public string Get_url_string_from_xml(ref string doc)
        {
            StringBuilder data = new StringBuilder();

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(doc);
            }
            catch(Exception e_xml)
            {
                string str = e_xml.ToString();
                EmailInfo emailInfo = new EmailInfo()
                {
                    to = AppCodeConstants.Alex_email,
                    fromAddress = AppCodeConstants.Alex_email,
                    fromName = "GCM API XML parser",
                    subject = "Could not parse XML request",
                    body = doc
                };
                return "Could not parse XML";
            }

            #region Origin and Destination

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("originZip");
            if (nodeList[0] != null)
            {
                data.Append(string.Concat("&q_OPCode=", nodeList[0].InnerText.Trim()));
            }
            else
            {
                // Do nothing
            }

            nodeList = xmlDoc.GetElementsByTagName("originCity");
            if (nodeList[0] != null)
            {
                data.Append(string.Concat("&origCity=", nodeList[0].InnerText.Trim()));
            }
            else
            {
                // Do nothing
            }

            nodeList = xmlDoc.GetElementsByTagName("originState");
            if (nodeList[0] != null)
            {
                data.Append(string.Concat("&origState=", nodeList[0].InnerText.Trim()));
            }
            else
            {
                // Do nothing
            }

            nodeList = xmlDoc.GetElementsByTagName("destinationZip");
            if (nodeList[0] != null)
            {
                data.Append(string.Concat("&q_DPCode=", nodeList[0].InnerText.Trim()));
            }
            else
            {
                // Do nothing
            }

            nodeList = xmlDoc.GetElementsByTagName("destinationCity");
            if (nodeList[0] != null)
            {
                data.Append(string.Concat("&destCity=", nodeList[0].InnerText.Trim()));
            }
            else
            {
                // Do nothing
            }

            nodeList = xmlDoc.GetElementsByTagName("destinationState");
            if (nodeList[0] != null)
            {
                data.Append(string.Concat("&destState=", nodeList[0].InnerText.Trim()));
            }
            else
            {
                // Do nothing
            }

            #endregion

            #region Pickup date

            nodeList = xmlDoc.GetElementsByTagName("pickupDate");

            if (nodeList[0] != null)
            {
                DateTime pu_date;
                if (DateTime.TryParse(nodeList[0].InnerText.Trim(), out pu_date))
                {
                    data.Append(string.Concat("&q_pickupDate=", pu_date.ToShortDateString()));
                }
            }
            else
            {
                // Do nothing
            }

            #endregion

            double test_double;
            int test_int;
            bool test_bool, hazmat = false;

            #region totalCube

            nodeList = xmlDoc.GetElementsByTagName("totalCube");

            if (nodeList[0] != null)
            {
                //double lineal_feet;
                if (double.TryParse(nodeList[0].InnerText.Trim(), out test_double))
                {
                    data.Append(string.Concat("&totalCube=", test_double));
                }
            }
            else
            {
                // Do nothing
            }

            #endregion

            #region Lineal feet

            nodeList = xmlDoc.GetElementsByTagName("linealFeet");

            if (nodeList[0] != null)
            {
                //double lineal_feet;
                if (double.TryParse(nodeList[0].InnerText.Trim(), out test_double))
                {
                    data.Append(string.Concat("&linealFeet=", test_double));
                }
            }
            else
            {
                // Do nothing
            }

            #endregion

            #region Items

            nodeList = xmlDoc.GetElementsByTagName("item");

            for (byte i = 1; i <= nodeList.Count; i++)
            {
                //if (double.TryParse(nodeList[0].InnerText.Trim(), out test_double))
                //{
                //    data.Append(string.Concat("&linealFeet=", test_double));
                //}

                //if(nodeList[i - 1]["type"] != null)
                //{
                //    data.Append(string.Concat("&q_Class", i, "=", test_double));
                //}

                if (nodeList[i - 1]["commodity"] != null)
                {
                    data.Append(string.Concat("&q_Commodity", i, "=", nodeList[i - 1]["commodity"].InnerText.Trim()));
                }
                else
                {

                }

                #region Weight Class

                if (double.TryParse(nodeList[i-1]["freightClass"].InnerText.Trim(), out test_double))
                {
                    data.Append(string.Concat("&q_Class", i, "=", test_double));
                }
                else
                {

                }

                if (double.TryParse(nodeList[i-1]["weight"].InnerText.Trim(), out test_double))
                {
                    data.Append(string.Concat("&q_Weight", i, "=", test_double));
                }
                else
                {

                }

                #endregion

                #region Units pieces

                if (int.TryParse(nodeList[i - 1]["units"].InnerText.Trim(), out test_int))
                {
                    data.Append(string.Concat("&commodity_unit", i, "=", test_int));
                }
                else
                {

                }

                if (int.TryParse(nodeList[i - 1]["pieces"].InnerText.Trim(), out test_int))
                {
                    data.Append(string.Concat("&q_Piece", i, "=", test_int));
                }
                else
                {

                }

                #endregion

                if (bool.TryParse(nodeList[i - 1]["hazmat"].InnerText.Trim(), out test_bool))
                {
                    hazmat = true;
                    //data.Append(string.Concat("&q_Class", i, "=", test_double));
                }
                else
                {

                }

                #region Dimensions

                if (int.TryParse(nodeList[i - 1]["length"].InnerText.Trim(), out test_int))
                {
                    data.Append(string.Concat("&q_Length", i, "=", test_int));
                }
                else
                {

                }

                if (int.TryParse(nodeList[i - 1]["width"].InnerText.Trim(), out test_int))
                {
                    data.Append(string.Concat("&q_Width", i, "=", test_int));
                }
                else
                {

                }

                if (int.TryParse(nodeList[i - 1]["height"].InnerText.Trim(), out test_int))
                {
                    data.Append(string.Concat("&q_Height", i, "=", test_int));
                }
                else
                {

                }

                #endregion
            }

            #endregion

            #region Accessorials

            nodeList = xmlDoc.GetElementsByTagName("service");
            for (byte i = 0; i < nodeList.Count; i++)
            {
                if(nodeList[i].InnerText == "RSP")
                {
                    data.Append("&q_ResPick=true&q_TailPick=true&q_AppPick=true");
                }
                else if (nodeList[i].InnerText == "RSD")
                {
                    data.Append("&q_ResDel=true&q_TailDel=true&q_AppDel=true");
                }
                else if (nodeList[i].InnerText == "CSP")
                {
                    data.Append("&q_ConstPick=true");
                }
                else if (nodeList[i].InnerText == "CSD")
                {
                    data.Append("&q_ConstDel=true");
                }
                else if (nodeList[i].InnerText == "TGP")
                {
                    data.Append("&q_TailPick=true");
                }
                else if (nodeList[i].InnerText == "TGD")
                {
                    data.Append("&q_TailDel=true");
                }
                else if (nodeList[i].InnerText == "AMP")
                {
                    data.Append("&q_AppPick=true");
                }
                else if (nodeList[i].InnerText == "AMD")
                {
                    data.Append("&q_TailDel=true");
                }
                else if (nodeList[i].InnerText == "TSP")
                {
                    data.Append("&q_TradePick=true");
                }
                else if (nodeList[i].InnerText == "TSD")
                {
                    data.Append("&q_TradeDel=true");
                }
                else if (nodeList[i].InnerText == "ISD")
                {
                    data.Append("&q_InsDel=true");
                }
                else
                {
                    return string.Concat("Unrecognized accessorial: ", nodeList[i].InnerText);
                }

                //DB.Log("additionalService", nodeList[i].InnerText);
            }

            if (hazmat == true)
            {
                data.Append(string.Concat("&isHazMat=True"));
            }
            else
            {
                data.Append(string.Concat("&isHazMat=False"));
            }

            #endregion

            return data.ToString();
        }

        #endregion

        #region Get_url_string_from_json

        public string Get_url_string_from_json(ref string doc)
        {
            return "test Get_url_string_from_json";
        }

        #endregion

        #region Add_one_node
        private void Add_one_node(ref StringBuilder data, ref XmlDocument xmlDoc, string tag_name, string url_name)
        {
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName(tag_name);

            if (nodeList[0] != null)
            {
                data.Append(string.Concat(url_name, nodeList[0]));
            }
            else
            {
                // Do nothing
            }
        }

        #endregion

    }
}