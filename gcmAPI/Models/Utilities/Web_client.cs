#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

#endregion

namespace gcmAPI.Models.Utilities
{
    public class Web_client
    {
        #region Variables

        public string url, referrer, method, content_type, accept, post_data, user_agent, response_string;
        public string[] header_names, header_values;
        public bool? allow_redirect;
        public CookieContainer cookie_container;
        public int time_out;

        #endregion

        #region Constructor

        public Web_client()
        {

            //this.info = new Scraping_info();
            cookie_container = new CookieContainer();

            user_agent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36";
            time_out = 60000;
            allow_redirect = true;

        }

        #endregion

        #region Make_http_request

        public string Make_http_request()
        {
            if (method.Equals("POST") && string.IsNullOrEmpty(content_type))
            {
                content_type = "application/x-www-form-urlencoded";
            }
            else
            {
                // Do nothing
            }

            #region BuildRequest

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Referer = referrer;

            request.UserAgent = user_agent;
            //request.
            request.KeepAlive = true;
            request.Method = method;
            request.ContentType = content_type;
            request.Accept = accept;
            //request.CookieContainer = new CookieContainer();
            request.CookieContainer = cookie_container;
            request.Timeout = time_out;
            request.AllowAutoRedirect = (bool)allow_redirect;
            
            if (header_names != null)
            {
                for (byte i = 0; i < header_names.Length; i++)
                {
                    request.Headers[header_names[i]] = header_values[i];
                }
            }

            #endregion

            #region GetResponse

            if (method.Equals("POST"))
            {
                #region Write to data stream

                byte[] postData;
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                postData = encoding.GetBytes(post_data);
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

                response_string = sb.ToString();

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