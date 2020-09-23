#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

#endregion

namespace gcmAPI.Models.Utilities
{
    public class Mail
    {
        EmailInfo emailInfo;

        public struct EmailInfo
        {
            public string to, fromName, fromAddress, body, subject, bcc;
            public string[] attachments;
        }

        public Mail(ref EmailInfo emailInfo)
        {
            this.emailInfo = emailInfo;
        }

        #region SendEmail

        public void SendEmail()
        {
            try
            {
                string[] forSplit;
                if (emailInfo.to.Contains(".") == false) // Validate email address
                {
                    throw new Exception("Invalid email address: " + emailInfo.to);
                }
               
                string mailServer = "", toAddressList = "";
                mailServer = AppCodeConstants.mail_server_ip;//"smtp.gmail.com"
                toAddressList = emailInfo.to;
              
                SmtpClient objClient = new SmtpClient(mailServer);

                MailMessage objMessage = new MailMessage();
               
                if(string.IsNullOrEmpty(emailInfo.bcc))
                {
                    // Do nothing
                }
                else
                {
                    HelperFuncs.writeToSiteErrors("bcc email", emailInfo.bcc);

                    forSplit = emailInfo.bcc.Split(' ');
                    for (int i = 0; i < forSplit.Length; i++)
                    {
                        objMessage.Bcc.Add(forSplit[i].Trim());
                        HelperFuncs.writeToSiteErrors("adding bcc", forSplit[i].Trim());
                    }
                }
                

                MailAddress objFrom = new System.Net.Mail.MailAddress(emailInfo.fromAddress, emailInfo.fromName);
                //MailAddress objTo = new System.Net.Mail.MailAddress(toAddressList, toAddressList);
               
                objMessage.From = objFrom;
              
                forSplit = emailInfo.to.Split(' ');
                for (int i = 0; i < forSplit.Length; i++)
                {
                    objMessage.To.Add(forSplit[i].Trim());
                }
                objMessage.Subject = emailInfo.subject;
                objMessage.IsBodyHtml = true;
                objMessage.Body = emailInfo.body;

                if(emailInfo.attachments == null)
                {
                    // Do nothing
                }
                else
                {
                    foreach (string img in emailInfo.attachments)
                    {

                        HelperFuncs.writeToSiteErrors("adding attachment", img);
                        try
                        {
                            Attachment attachment = new Attachment(new Uri(img).LocalPath);
                            objMessage.Attachments.Add(attachment);
                        }
                        catch (Exception e1)
                        {
                            HelperFuncs.writeToSiteErrors("error adding attachment " + img, e1.ToString());
                            //MailUser("", e1.ToString(), messageText, fromName, fromAddress, messageSubject, "");
                        }
                    }
                }
                

                objClient.Send(objMessage);
            }
            catch (Exception e)
            {
                HelperFuncs.writeToSiteErrors("sendEmail", e.ToString());
                //MailUser("", e.ToString(), messageText, fromName, fromAddress, messageSubject, "");
            }
        }

        #endregion

        #region sendEmailToCustomer

        //public void sendEmailToCustomer(string shipName, string consName, string pro, string ata, string shipID, string PO, string carName, string email, string mode)
        //{
        //    string fromName = "Global Cargo Manager";
        //    string fromAddress = "cs" + AppCodeConstants.email_domain;
        //    string messageSubject = "Shipment for \"" + (string)shipName + "\" has been picked up by \"" + carName + "\"";

        //    string emailBody = "";

        //    if (mode == "demo")
        //    {
        //        emailBody += "This is a test email. If the program was live, the email would have been sent to: " + email +
        //             " (if there's no email - there was no email address in the database)<br><br><br>";
        //    }

        //    emailBody += "Notice from Global Cargo Manager: Your shipment has been picked up and is in transit." + "<br>" +

        //    "Shipment ID: " + shipID.ToString() + "<br>" +
        //    "Shipper Name: " + (string)shipName + "<br>" +
        //    "Consignee: " + (string)consName + "<br>" +
        //    "PO/Ref #" + (string)PO + "<br>" +
        //    "Carrier: " + carName + "<br>" +
        //    "Pro Number: " + pro + "<br>" +
        //    "Estimated delivery date (as published on carrier's website): " + ata + " (Transit time is estimated unless your shipment was booked as guaranteed.)<br><br><br>" +
        //    "Call or email customer service with any questions 877-890-2295/cs" + AppCodeConstants.email_domain;
        //    //MailUser("", "", emailBody, fromName, fromAddress, messageSubject);
        //    //if (mode == "demo")
        //    //{
        //    //    MailUser("", "", emailBody, fromName, fromAddress, messageSubject, ""); // 
        //    //}
        //    //else if (mode == "live")
        //    //{
        //    //    MailUser(email, "", emailBody, fromName, fromAddress, messageSubject, "");  //           
        //    //}
        //}

        #endregion
  
        #region IsValidEmail

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}