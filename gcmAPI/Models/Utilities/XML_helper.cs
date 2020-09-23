using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace gcmAPI.Models.Utilities
{
    public class XML_helper
    {
        public static string Serialize<T>(T dataToSerialize)
        {
            try
            {
                var stringwriter = new System.IO.StringWriter();
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
            catch(Exception e)
            {
                DB.Log("XML_helper", e.ToString());
                return "could not serialize";
                //throw;
            }
        }

        public static T Deserialize<T>(string xmlText)
        {
            try
            {
                var stringReader = new System.IO.StringReader(xmlText);
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
            catch
            {
                throw;
            }
        }

        //public string ToXML()
        //{
        //    using (var stringwriter = new System.IO.StringWriter())
        //    {
        //        var serializer = new XmlSerializer(this.GetType());
        //        serializer.Serialize(stringwriter, this);
        //        return stringwriter.ToString();
        //    }
        //}

        //public static SharedLTL.CarriersResult LoadFromXMLString(string xmlText)
        //{
        //    using (var stringReader = new System.IO.StringReader(xmlText))
        //    {
        //        var serializer = new XmlSerializer(typeof(SharedLTL.CarriersResult));
        //        return serializer.Deserialize(stringReader) as SharedLTL.CarriersResult;
        //    }
        //}

        #region Add_one_node
        //public string Get_one_node_to_(ref StringBuilder data, ref XmlDocument xmlDoc, 
        //    string tag_name, string url_name)
        //{
        //    XmlNodeList nodeList = xmlDoc.GetElementsByTagName(tag_name);

        //    if (nodeList[0] != null)
        //    {
        //        data.Append(string.Concat(url_name, nodeList[0]));
        //    }
        //    else
        //    {
        //        // Do nothing
        //    }
        //}

        #endregion

    }
}