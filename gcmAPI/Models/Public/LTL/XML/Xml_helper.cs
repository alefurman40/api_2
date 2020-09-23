#region Using

using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

#endregion

namespace gcmAPI.Models.Public.LTL.XML
{
    public class Xml_helper
    {
        #region Is_request_valid_XML
        public bool Is_request_valid_XML(ref string request)
        {
            try
            {
                //DB.Log("FU Public API request xml", request);
                //XDocument xd1 = new XDocument();
                //xd1 = XDocument.Load(request);
                XDocument doc;
                using (StringReader s = new StringReader(request))
                {
                    doc = XDocument.Load(s);
                }
                return true;
            }
            catch (XmlException e)
            {
                DB.Log("not valid XML ex", e.ToString());               
            }
            catch (Exception e)
            {
                DB.Log("not valid XML ex", e.ToString());
            }
            return false;
        }
        #endregion

        #region Build_response
        public string Build_response(ref GCMRateQuote[] totalQuotes, ref string request_format)
        {
            StringBuilder xml = new StringBuilder();
            xml.Append("<LTLRateReply>");

            xml.Append("<RateServiceNotification>");
            xml.Append("<Code>0");
            xml.Append("</Code>");
            xml.Append("<Message>Success");
            xml.Append("</Message>");
            xml.Append("</RateServiceNotification>");

            xml.Append("<LTLResult>");

            for (byte i = 0; i < totalQuotes.Length; i++)
            {
                if (i == 0)
                {
                    xml.Append(string.Concat("<QuoteId>", totalQuotes[i].NewLogId, "</QuoteId>"));
                }
                else
                {
                    // Do nothing
                }

                xml.Append("<GCMRateQuote>");

                xml.Append(
                    string.Concat("<TotalPrice>", totalQuotes[i].TotalPrice, "</TotalPrice>")
                    );

                xml.Append(
                    string.Concat("<DisplayName>", totalQuotes[i].DisplayName.Replace("%2C", ","), "</DisplayName>")
                    );

                xml.Append(
                    string.Concat("<DeliveryDays>", totalQuotes[i].DeliveryDays, "</DeliveryDays>")
                    );

                xml.Append(
                    string.Concat("<CoverageCost>", totalQuotes[i].CoverageCost, "</CoverageCost>")
                    );

                xml.Append(
                    string.Concat("<Documentation>", totalQuotes[i].Documentation, "</Documentation>")
                    );

                xml.Append(
                    string.Concat("<SCAC>", totalQuotes[i].Scac, "</SCAC>")
                    );

                xml.Append(
                    string.Concat("<RulesTarrif>", totalQuotes[i].RulesTarrif, "</RulesTarrif>")
                    );

                xml.Append(
                    string.Concat("<RateType>", totalQuotes[i].RateType, "</RateType>")
                    );

                xml.Append(
                    string.Concat("<RateId>", totalQuotes[i].RateId, "</RateId>")
                    );

                xml.Append(
                    string.Concat("<BookingKey>", totalQuotes[i].BookingKey, "</BookingKey>")
                    );

                xml.Append("</GCMRateQuote>");
            }

            xml.Append("</LTLResult>");

            xml.Append("</LTLRateReply>");

            return xml.ToString();

            //if(request_format=="JSON")
            //{
            //    var json = new JavaScriptSerializer().Serialize(xml);
            //    return json.ToString();
            //}
            //else
            //{
            //    return xml.ToString();
            //}

        }

        #endregion

        #region Build_error_response
        public string Build_error_response(string code, string message, ref string request_format)
        {
            string xml = string.Concat(
                        "<LTLRateReply>",

                        "<RateServiceNotification>",
                        "<Code>", code,
                        "</Code>",
                        "<Message>", message,
                        "</Message>",
                        "</RateServiceNotification>",

                        "<LTLResult>",
                        "</LTLResult>",

                        "</LTLRateReply>"

                        );

            if (request_format == "JSON")
            {
                var json = new JavaScriptSerializer().Serialize(xml);
                return json.ToString();
            }
            else
            {
                return xml;
            }
        }
        #endregion

    }
}