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
/// Summary description for LTLResult
/// </summary>
public class LTLResult
{
    private string _BookingKey = "";
    private string _CarrierKey = "";
    private string _BusinessDays = "";
    private string _CarrierDisplayName = "";
    private double _Rate = 0;
    private double _CoverageCost = 0;
	private double _BuyRate = 0;
    

	public LTLResult()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string BusinessDays
    {
        set
        {
            _BusinessDays = value;
        }
        get
        {
            return _BusinessDays;
        }
    }
    public string CarrierDisplayName
    {
        set
        {
            _CarrierDisplayName = value;
        }
        get
        {
            return _CarrierDisplayName;
        }
    }
    public double Rate
    {
        set
        {
            _Rate = value;
        }
        get
        {
            return _Rate;
        }
    }
	public double BuyRate
    {
        set
        {
            _BuyRate = value;
        }
        get
        {
            return _BuyRate;
        }
    }
    public string BookingKey
    {
        get
        {
            return _BookingKey;
        }
        set
        {
            _BookingKey = value;
        }
    }
    public string CarrierKey
    {
        get
        {
            return _CarrierKey;
        }
        set
        {
            _CarrierKey = value;
        }
    }
    public double CoverageCost
    {
        set
        {
            _CoverageCost = value;
        }
        get
        {
            return _CoverageCost;
        }
    }
}
