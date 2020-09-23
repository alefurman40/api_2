#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using gcmAPI.Models.Utilities;
using gcmAPI.RateServiceWebReference;

#endregion

namespace gcmAPI.Models.Carriers.FedEx
{
    public class Fedex_rate
    {

        #region CreateRateRequest 

        public RateRequest CreateRateRequest()
        {
            // Build a RateRequest
            RateRequest request = new RateRequest();
            //
            request.WebAuthenticationDetail = new WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();
            request.WebAuthenticationDetail.UserCredential.Key = AppCodeConstants.fedex_freight_key; // Replace "XXX" with the Key
            request.WebAuthenticationDetail.UserCredential.Password = AppCodeConstants.fedex_freight_pwd; // Replace "XXX" with the Password
            //request.WebAuthenticationDetail.ParentCredential = new WebAuthenticationCredential();
            //request.WebAuthenticationDetail.ParentCredential.Key = "XXX"; // Replace "XXX" with the Key
            //request.WebAuthenticationDetail.ParentCredential.Password = ""; // Replace "XXX"
            //if (usePropertyFile()) //Set values from a file for testing purposes
            //{
            //    request.WebAuthenticationDetail.UserCredential.Key = getProperty("key");
            //    request.WebAuthenticationDetail.UserCredential.Password = getProperty("password");
            //    request.WebAuthenticationDetail.ParentCredential.Key = getProperty("parentkey");
            //    request.WebAuthenticationDetail.ParentCredential.Password = getProperty("parentpassword");
            //}
            //
            request.ClientDetail = new ClientDetail();
            request.ClientDetail.AccountNumber = AppCodeConstants.fedex_freight_acct; // Replace "XXX" with the client's account number
            request.ClientDetail.MeterNumber = AppCodeConstants.fedex_freight_meter; // Replace "XXX" with the client's meter number
            //if (usePropertyFile()) //Set values from a file for testing purposes
            //{
            //    request.ClientDetail.AccountNumber = getProperty("accountnumber");
            //    request.ClientDetail.MeterNumber = getProperty("meternumber");
            //}
            //
            request.TransactionDetail = new TransactionDetail();
            request.TransactionDetail.CustomerTransactionId = "***Freight Rate Request using VC#***"; // This is a reference field for the customer.  Any value can be used and will be provided in the response.
            //
            request.Version = new VersionId();
            //
            request.ReturnTransitAndCommit = true;
            request.ReturnTransitAndCommitSpecified = true;
            request.CarrierCodes = new CarrierCodeType[1];
            request.CarrierCodes[0] = CarrierCodeType.FXFR;
            //
            SetShipmentDetails(request);
            //
            return request;
        }

        #endregion

        #region SetShipmentDetails

        private static void SetShipmentDetails(RateRequest request)
        {
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.ShipTimestamp = DateTime.Now; // Shipping date and time
            request.RequestedShipment.ShipTimestampSpecified = true;
            request.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP; //Drop off types are BUSINESS_SERVICE_CENTER, DROP_BOX, REGULAR_PICKUP, REQUEST_COURIER, STATION
            // If ServiceType is omitted, all applicable ServiceTypes are returned.
            //request.RequestedShipment.ServiceType = ServiceType.FEDEX_FREIGHT_PRIORITY; // Service types are STANDARD_OVERNIGHT, PRIORITY_OVERNIGHT, FEDEX_GROUND ...
            //request.RequestedShipment.ServiceTypeSpecified = true;
            request.RequestedShipment.PackagingType = "YOUR_PACKAGING"; // Packaging type FEDEX_BOK, FEDEX_PAK, FEDEX_TUBE, YOUR_PACKAGING, ...
                                                                        //  request.RequestedShipment.PackagingTypeSpecified = true;
                                                                        //
            SetSender(request);
            //
            SetRecipient(request);
            //
            SetPayment(request);
            //
            SetFreightShipmentDetail(request);
            //
            request.RequestedShipment.PackageCount = "1";
        }

        #endregion

        #region SetSender

        private static void SetSender(RateRequest request)
        {
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Address = new Address();
            request.RequestedShipment.Shipper.Address.StreetLines = new string[1] { "SHIPPER ADDRESS LINE 1" };
            // Replace "XXX" with sender address
            request.RequestedShipment.Shipper.Address.City = "Harrison";
            request.RequestedShipment.Shipper.Address.StateOrProvinceCode = "AR";
            request.RequestedShipment.Shipper.Address.PostalCode = "72601";
            request.RequestedShipment.Shipper.Address.CountryCode = "US";
        }

        #endregion

        #region SetRecipient

        private static void SetRecipient(RateRequest request)
        {
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Address = new Address();
            request.RequestedShipment.Recipient.Address.StreetLines = new string[1] { "RECIPIENT ADDRESS LINE 1" };
            request.RequestedShipment.Recipient.Address.City = "COLORADO SPRINGS";
            request.RequestedShipment.Recipient.Address.StateOrProvinceCode = "CO";
            request.RequestedShipment.Recipient.Address.PostalCode = "80915";
            request.RequestedShipment.Recipient.Address.CountryCode = "US";
        }

        #endregion

        #region SetPayment

        private static void SetPayment(RateRequest request)
        {
            request.RequestedShipment.ShippingChargesPayment = new Payment();
            request.RequestedShipment.ShippingChargesPayment.PaymentType = PaymentType.SENDER;
            request.RequestedShipment.ShippingChargesPayment.PaymentTypeSpecified = true;
            request.RequestedShipment.ShippingChargesPayment.Payor = new Payor();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty = new Party();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.AccountNumber = "XXX"; // Replace "XXX" with client's account number
            if (usePropertyFile()) //Set values from a file for testing purposes
            {
                request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.AccountNumber = getProperty("payoraccount");
            }
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.Contact = new Contact();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.Address = new Address();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.Address.CountryCode = "US";
        }

        #endregion

        #region SetFreightShipmentDetail

        private static void SetFreightShipmentDetail(RateRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail = new FreightShipmentDetail();
            request.RequestedShipment.FreightShipmentDetail.FedExFreightAccountNumber = "XXX"; // Replace "XXX" with the client's account number
            if (usePropertyFile()) //Set values from a file for testing purposes
            {
                request.RequestedShipment.FreightShipmentDetail.FedExFreightAccountNumber = getProperty("freightaccount");
            }
            SetFreightBillingContactAddress(request);
            request.RequestedShipment.FreightShipmentDetail.Role = FreightShipmentRoleType.SHIPPER;
            request.RequestedShipment.FreightShipmentDetail.RoleSpecified = true;
            SetFreightDeclaredValue(request);
            SetFreightLiabilityCoverageDetail(request);
            request.RequestedShipment.FreightShipmentDetail.TotalHandlingUnits = "15";
            SetFreightShipmentDimensions(request);
            SetFreightShipmentLineItems(request);
        }

        #endregion

        #region SetFreightBillingContactAddress

        private static void SetFreightBillingContactAddress(RateRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress = new ContactAndAddress();
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Contact = new Contact();
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Contact.PersonName = "Freight Billing Contact";
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Contact.CompanyName = "Freight Billing Company";
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Contact.PhoneNumber = "1234567890";
            //
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address = new Address();
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.StreetLines = new string[1] { "FREIGHT BILLING ADDRESS LINE 1" };
            // Replace "XXX" with Freight billing address
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.City = "Harrison";
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.StateOrProvinceCode = "AR";
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.PostalCode = "72601";
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.CountryCode = "US";
        }

        #endregion

        #region SetFreightDeclaredValue

        private static void SetFreightDeclaredValue(RateRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail.DeclaredValuePerUnit = new Money();
            request.RequestedShipment.FreightShipmentDetail.DeclaredValuePerUnit.Currency = "USD";
            request.RequestedShipment.FreightShipmentDetail.DeclaredValuePerUnit.Amount = 50.0M;
            request.RequestedShipment.FreightShipmentDetail.DeclaredValuePerUnit.AmountSpecified = true;
        }

        #endregion

        #region SetFreightLiabilityCoverageDetail

        private static void SetFreightLiabilityCoverageDetail(RateRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail.LiabilityCoverageDetail = new LiabilityCoverageDetail();
            request.RequestedShipment.FreightShipmentDetail.LiabilityCoverageDetail.CoverageType = LiabilityCoverageType.NEW;
            request.RequestedShipment.FreightShipmentDetail.LiabilityCoverageDetail.CoverageTypeSpecified = true;
            request.RequestedShipment.FreightShipmentDetail.LiabilityCoverageDetail.CoverageAmount = new Money();
            request.RequestedShipment.FreightShipmentDetail.LiabilityCoverageDetail.CoverageAmount.Currency = "USD";
            request.RequestedShipment.FreightShipmentDetail.LiabilityCoverageDetail.CoverageAmount.Amount = 50.0M;
            request.RequestedShipment.FreightShipmentDetail.LiabilityCoverageDetail.CoverageAmount.AmountSpecified = true;
        }

        #endregion

        #region SetFreightShipmentDimensions

        private static void SetFreightShipmentDimensions(RateRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail.ShipmentDimensions = new Dimensions();
            request.RequestedShipment.FreightShipmentDetail.ShipmentDimensions.Length = "90";
            request.RequestedShipment.FreightShipmentDetail.ShipmentDimensions.Width = "60";
            request.RequestedShipment.FreightShipmentDetail.ShipmentDimensions.Height = "50";
            request.RequestedShipment.FreightShipmentDetail.ShipmentDimensions.Units = LinearUnits.IN;
            request.RequestedShipment.FreightShipmentDetail.ShipmentDimensions.UnitsSpecified = true;
        }

        #endregion

        #region SetFreightShipmentLineItems

        private static void SetFreightShipmentLineItems(RateRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail.LineItems = new FreightShipmentLineItem[1];
            request.RequestedShipment.FreightShipmentDetail.LineItems[0] = new FreightShipmentLineItem();
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].FreightClass = FreightClassType.CLASS_050;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].FreightClassSpecified = true;
            //
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Packaging = PhysicalPackagingType.BOX;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].PackagingSpecified = true;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Description = "Freight line item description";
            //
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Weight = new Weight();
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Weight.Units = WeightUnits.LB;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Weight.UnitsSpecified = true;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Weight.Value = 1000.0M;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Weight.ValueSpecified = true;
            //
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions = new Dimensions();
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.Length = "90";
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.Width = "60";
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.Height = "50";
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.Units = LinearUnits.IN;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.UnitsSpecified = true;
            //
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Volume = new Volume();
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Volume.Units = VolumeUnits.CUBIC_FT;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Volume.UnitsSpecified = true;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Volume.Value = 30M;
            request.RequestedShipment.FreightShipmentDetail.LineItems[0].Volume.ValueSpecified = true;
        }

        #endregion

        #region ShowRateReply

        public void ShowRateReply(RateReply reply)
        {
            DB.LogGenera("FedEx","","RateReply details:");
            foreach (RateReplyDetail rateDetail in reply.RateReplyDetails)
            {
                DB.LogGenera("FedEx","","ServiceType: {0}" +  rateDetail.ServiceType);
                //DB.LogGenera("FedEx","",);
                foreach (RatedShipmentDetail shipmentDetail in rateDetail.RatedShipmentDetails)
                {
                    ShowShipmentRateDetails(shipmentDetail);
                }
                ShowDeliveryDetails(rateDetail);
                DB.LogGenera("FedEx","","**********************************************************");
            }
            ShowNotifications(reply);
        }

        #endregion

        #region ShowShipmentRateDetails

        public void ShowShipmentRateDetails(RatedShipmentDetail shipmentDetail)
        {
            if (shipmentDetail == null) return;
            if (shipmentDetail.ShipmentRateDetail == null) return;
            ShipmentRateDetail rateDetail = shipmentDetail.ShipmentRateDetail;
            //
            DB.LogGenera("FedEx","","RateType: {0}" + rateDetail.RateType);
            if (rateDetail.TotalBillingWeight != null)
                DB.LogGenera("FedEx","",string.Concat("Total Billing Weight: {0} {1}", rateDetail.TotalBillingWeight.Value, shipmentDetail.ShipmentRateDetail.TotalBillingWeight.Units));
            if (rateDetail.TotalBaseCharge != null)
                DB.LogGenera("FedEx","", string.Concat("Total Base Charge: {0} {1}", rateDetail.TotalBaseCharge.Amount, rateDetail.TotalBaseCharge.Currency));
            if (rateDetail.TotalFreightDiscounts != null)
                DB.LogGenera("FedEx","", string.Concat("Total Freight Discounts: {0} {1}", rateDetail.TotalFreightDiscounts.Amount, rateDetail.TotalFreightDiscounts.Currency));
            if (rateDetail.TotalSurcharges != null)
                DB.LogGenera("FedEx","", string.Concat("Total Surcharges: {0} {1}", rateDetail.TotalSurcharges.Amount, rateDetail.TotalSurcharges.Currency));
            if (rateDetail.Surcharges != null)
            {
                // Individual surcharge for each package
                foreach (Surcharge surcharge in rateDetail.Surcharges)
                    DB.LogGenera("FedEx","", string.Concat(" {0} surcharge {1} {2}", surcharge.SurchargeType, surcharge.Amount.Amount, surcharge.Amount.Currency));
            }
            if (rateDetail.TotalNetCharge != null) DB.LogGenera("FedEx","", string.Concat("Total Net Charge: {0} {1}", rateDetail.TotalNetCharge.Amount, rateDetail.TotalNetCharge.Currency));
            ShowFreightRateDetail(rateDetail.FreightRateDetail);
        }

        #endregion

        #region ShowFreightRateDetail

        private static void ShowFreightRateDetail(FreightRateDetail freightRateDetail)
        {
            if (freightRateDetail == null) return;
            //DB.LogGenera("FedEx","",);
            DB.LogGenera("FedEx","","Freight Rate details");
            if (freightRateDetail.QuoteNumber != null)
                DB.LogGenera("FedEx","", string.Concat("Quote number {0} ", freightRateDetail.QuoteNumber));
            // Individual FreightBaseCharge for each shipment
            foreach (FreightBaseCharge freightBaseCharge in freightRateDetail.BaseCharges)
            {
                if (freightBaseCharge.Description != null)
                    DB.LogGenera("FedEx","","Description " + freightBaseCharge.Description);
                if (freightBaseCharge.Weight != null)
                    DB.LogGenera("FedEx","", string.Concat("Weight {0} {1} ", freightBaseCharge.Weight.Value, freightBaseCharge.Weight.Units));
                if (freightBaseCharge.ChargeRate != null)
                    DB.LogGenera("FedEx","", string.Concat("Charge rate {0} {1} ", freightBaseCharge.ChargeRate.Amount, freightBaseCharge.ChargeRate.Currency));
                if (freightBaseCharge.ExtendedAmount != null)
                    DB.LogGenera("FedEx","", string.Concat("Extended amount {0} {1} ", freightBaseCharge.ExtendedAmount.Amount, freightBaseCharge.ExtendedAmount.Currency));
                //DB.LogGenera("FedEx","",);
            }
        }

        #endregion

        #region ShowDeliveryDetails

        private static void ShowDeliveryDetails(RateReplyDetail rateDetail)
        {
            if (rateDetail.DeliveryTimestampSpecified)
                DB.LogGenera("FedEx","","Delivery timestamp: " + rateDetail.DeliveryTimestamp.ToString());
            if (rateDetail.TransitTimeSpecified)
                DB.LogGenera("FedEx","","Transit Time: " + rateDetail.TransitTime);
        }

        #endregion

        #region ShowNotifications

        public void ShowNotifications(RateReply reply)
        {
            DB.LogGenera("FedEx","","Notifications");
            for (int i = 0; i < reply.Notifications.Length; i++)
            {
                Notification notification = reply.Notifications[i];
                DB.LogGenera("FedEx","", string.Concat("Notification no. {0}", i));
                if (notification.SeveritySpecified)
                    DB.LogGenera("FedEx","", string.Concat(" Severity: {0}", notification.Severity));
                DB.LogGenera("FedEx","", string.Concat(" Code: {0}", notification.Code));
                DB.LogGenera("FedEx","", string.Concat(" Message: {0}", notification.Message));
                DB.LogGenera("FedEx","", string.Concat(" Source: {0}", notification.Source));
            }
        }

        #endregion

        #region usePropertyFile

        private static bool usePropertyFile() //Set to true for common properties to be set with getProperty function.
        {
            return getProperty("usefile").Equals("True");
        }

        #endregion

        #region getProperty

        private static String getProperty(String propertyname) //Sets common properties for testing purposes.
        {
            try
            {
                String filename = "C:\\Users\\ai953503\\Desktop\\SampleCode-Details\\Jul19 Sample code\\C#\\CS_WSGW_Properties.txt";
                if (System.IO.File.Exists(filename))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(filename);
                    do
                    {
                        String[] parts = sr.ReadLine().Split(',');
                        if (parts[0].Equals(propertyname) && parts.Length == 2)
                        {
                            return parts[1];
                        }
                    }
                    while (!sr.EndOfStream);
                }
                DB.LogGenera("FedEx","", string.Concat("Property {0} set to default 'XXX'", propertyname));
                return "XXX";
            }
            catch (Exception e)
            {
                DB.LogGenera("FedEx","", string.Concat("Property {0} set to default 'XXX'", propertyname));
                return "XXX";
            }
        }

        #endregion
    }
}