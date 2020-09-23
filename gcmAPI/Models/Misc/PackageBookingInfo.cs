using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for LTLPiece
/// </summary>
public class PackageBookingInfo
{
    public string user, pass, accessLicenseNum, accountNum, shipperNum, serviceCode, shipDescription, shipChargeType,
            shipperAddressLine1, shipperCity, shipperState, shipperPostalCode, shipperCountryCode,
            shipperName, shipperAttentionName, shipperPhone,
            fromAddressLine1, fromCity, fromState, fromPostalCode, fromCountryCode, fromName, fromAttentionName,
            toAddressLine1, toCity, toState, toPostalCode, toCountryCode, toName, toAttentionName, toPhone;

    //public upsPackageItem[] packageItems;
    public PackageItem[] packageItems;
    public string[] additionalInfo;
}
