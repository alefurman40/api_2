#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;

#endregion

namespace gcmAPI.Models.Utilities
{
    public class Scraper
    {
        #region Variables

        public struct Scraping_info
        {
            public string url, referrer, method, content_type, accept, post_data, user_agent;
            public string[] header_names, header_values;
            public bool? allow_redirect;
            public CookieContainer cookie_container;
            public int time_out;
        }

        public Scraping_info info;

        #endregion

        #region Constructor

        public Scraper(Scraping_info info)
        {
            this.info = info;
            this.info.cookie_container = new CookieContainer();

            if(string.IsNullOrEmpty(this.info.user_agent))
            {
                this.info.user_agent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.83 Safari/535.11";               
            }
            else
            {
                // Do nothing
            }

            if (this.info.time_out.Equals(0))
            {
                this.info.time_out = 20000;
            }
            else
            {
                // Do nothing
            }

            if (this.info.allow_redirect.Equals(false))
            {
                // Do nothing
            }
            else
            {
                this.info.allow_redirect = true;
            }
        }

        #endregion

        #region Make_http_request

        public string Make_http_request()             
        {
            if (info.method.Equals("POST") && string.IsNullOrEmpty(info.content_type))
            {
                info.content_type = "application/x-www-form-urlencoded";
            }
            else
            {
                // Do nothing
            }

            #region BuildRequest

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(info.url);

            request.Referer = info.referrer;
            
            request.UserAgent = info.user_agent;

            request.KeepAlive = true;
            request.Method = info.method;
            request.ContentType = info.content_type;
            request.Accept = info.accept;
            request.CookieContainer = new CookieContainer();
            request.CookieContainer = info.cookie_container;
            request.Timeout = info.time_out;
            request.AllowAutoRedirect = (bool)info.allow_redirect;

            #endregion

            #region GetResponse

            if (info.method.Equals("POST"))
            {
                #region Write to data stream

                byte[] postData;
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                postData = encoding.GetBytes(info.post_data);
                request.ContentLength = postData.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(postData, 0, postData.Length);
                }

                #endregion
            }
            else
            {
                // Do nothing
            }

            StringBuilder sb = new StringBuilder();

            // Execute the request
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
             
                byte[] buf = new byte[8192];

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

                request.Abort();

                #region Return result

                response.Close();

                return sb.ToString();

                #region Not used

                //if (return_type == "string")
                //{


                //}
                //else if (return_type == "collection")
                //{
                //    CookieCollection collection = new CookieCollection();
                //    collection = response.Cookies;
                //    response.Close();
                //    return collection;
                //}
                //else if (return_type == "location")
                //{
                //    string s = response.Headers["Location"];
                //    response.Close();
                //    return s;
                //}
                //else if (return_type == "CollectionLocationDocument")
                //{
                //    CollectionLocationDoc collLocDoc = new CollectionLocationDoc();
                //    collLocDoc.coll = new CookieCollection();
                //    collLocDoc.coll = response.Cookies;
                //    collLocDoc.doc = sb.ToString();
                //    collLocDoc.location = response.Headers["Location"];
                //    response.Close();
                //    return collLocDoc;
                //}
                //else
                //{
                //    response.Close();
                //    return null;
                //}

                #endregion

                #endregion
            }

            #endregion
        }

        #endregion

    }
}