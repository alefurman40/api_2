using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;

namespace gcmAPI.Controllers.admin
{
    public class GetDocumentsController : ApiController
    {
        // GET api/getdocuments
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/getdocuments/5
        public string Get(int id)
        {
         
            int ind;
           
            string shipId = id.ToString(), retString = "";
          
            try
            {
                HelperFuncs.writeToSiteErrors("Get Documents", id.ToString());
                //if (!string.IsNullOrEmpty(Request.QueryString["shipId"]) && int.TryParse(Request.QueryString["shipId"], out intID))
                //{
                //    shipId = Request.QueryString["shipId"];
                //}

                List<string> data = new List<string>();
                List<string> aesBol = new List<string>();
                HelperFuncs.getDocs(ref data, ref shipId);

                #region get AES BOL

                DirectoryInfo di;
                di = new DirectoryInfo(string.Concat(AppCodeConstants.G_root, "\\bookrate\\BOLReports"));

                DirectoryInfo diAES;
                diAES = new DirectoryInfo(string.Concat(AppCodeConstants.G_root, "\\aes\\bookrate\\BOLReports")); //some of the BOL's are in the net-net directory          

                DirectoryInfo diBOLEditor;
                diBOLEditor = new DirectoryInfo(string.Concat(AppCodeConstants.G_root_demo, "\\admin\\BOLReports"));

                string fileName = "";
                foreach (FileInfo f in di.GetFiles("*_" + shipId + "_*")) //* stands for pattern
                {
                    fileName = AppCodeConstants.g_car_man_base_url + "/bookrate/BOLReports/" + f.Name;
                    aesBol.Add(fileName);
                }
                foreach (FileInfo f in diAES.GetFiles("*_" + shipId + "_*"))
                {
                    fileName = AppCodeConstants.g_car_man_base_url + "/aes/bookrate/BOLReports/" + f.Name;
                    aesBol.Add(fileName);
                }

                foreach (FileInfo f in diBOLEditor.GetFiles("*_" + shipId + "_*"))
                {
                    //HelperFuncs.writeToSiteErrors("fName", f.Name);
                    fileName = AppCodeConstants.g_car_man_base_url + "/admin/BOLReports/" + f.Name;
                    aesBol.Add(fileName);
                }

                #endregion

                for (int i = 0; i < aesBol.Count; i++)
                {
                    //ind = data[i].IndexOf("WebDocs");
                    retString += aesBol[i];

                    if (i != (aesBol.Count - 1))
                        retString += "^";
                }

                // Construct retString
                for (int i = 0; i < data.Count; i++)
                {
                    if (i.Equals(0) && aesBol.Count.Equals(0))
                    {
                    }
                    else
                    {
                        retString += "^";
                    }
                    ind = data[i].IndexOf("WebDocs");
                    retString += data[i].Substring(ind);

                    //if (i != (data.Count - 1))
                    //    retString += "^";
                }

                return retString;
            }
            catch (Exception cE)
            {
                if (!cE.Message.Contains("abort"))
                {
                    HelperFuncs.writeToSiteErrors("Get Documents", cE.ToString());
                }
                return "error";
            }            
        }

        // POST api/getdocuments
        public void Post([FromBody]string value)
        {
        }

        // PUT api/getdocuments/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/getdocuments/5
        public void Delete(int id)
        {
        }

    }
}
