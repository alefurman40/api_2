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
/// Summary description for BookingAddress
/// </summary>
public class BookingAddress
{
    private string _Name = "";
    private string _Email = "";
    private string _Company = "";
    //
    private int _DispatchAddressesId = 0;
    private string _Address1 = "";
    private string _Address2 = "";
    private string _City = "";
    private string _State = "";
    private string _Zip = "";
    private string _Phone = "";
    private string _Fax = "";

	public BookingAddress()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string Name
    {
        get
        {
            return _Name;
        }
        set
        {
            _Name = value;
        }
    }
    public string Email
    {
        get
        {
            return _Email;
        }
        set
        {
            _Email = value;
        }
    }
    public string Company
    {
        get
        {
            return _Company;
        }
        set
        {
            _Company = value;
        }
    }
    public string Address1
    {
        get
        {
            return _Address1;
        }
        set
        {
            _Address1 = value;
        }
    }
    public string Address2
    {
        get
        {
            return _Address2;
        }
        set
        {
            _Address2 = value;
        }
    }
    public string City
    {
        get
        {
            return _City;
        }
        set
        {
            _City = value;
        }
    }
    public string State
    {
        get
        {
            return _State;
        }
        set
        {
            _State = value;
        }
    }
    public string Zip
    {
        get
        {
            return _Zip;
        }
        set
        {
            _Zip = value;
        }
    }
    public string Phone
    {
        get
        {
            return _Phone;
        }
        set
        {
            _Phone = value;
        }
    }
    public string Fax
    {
        get
        {
            return _Fax;
        }
        set
        {
            _Fax = value;
        }
    }
    public int DispatchAddressesId
    {
        get { return _DispatchAddressesId; }
        set { _DispatchAddressesId = value; }
    }
   
}
