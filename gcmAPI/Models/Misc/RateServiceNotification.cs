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
/// Summary description for RateServiceNotification
/// </summary>
public class RateServiceNotification
{
    private string _Code = "";
    private string _Message = "";
    public string Code
    {
        set
        {
            _Code = value;
        }
        get
        {
            return _Code;
        }
    }
    public string Message
    {
        set
        {
            _Message = value;
        }
        get
        {
            return _Message;
        }
    }
	public RateServiceNotification()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public RateServiceNotification(string code,string message)
    {
        //
        // TODO: Add constructor logic here
        //
        this._Code = code;
        this._Message = message;
    }
}
