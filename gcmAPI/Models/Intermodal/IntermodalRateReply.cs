#region Using

using System;

#endregion

/// <summary>
/// Summary description for IntermodalRateReply
/// </summary>
public class IntermodalRateReply
{
    private RateServiceNotification _Notification;
    private IntermodalResult[] _IntermodalRates;
    private int _QuoteID;
    public string[] additionalInfo;

    public int QuoteID
    {
        set
        {
            _QuoteID = value;
        }
        get
        {
            return _QuoteID;
        }
    }

    public RateServiceNotification Notification
    {
        set
        {
            _Notification = value;
        }
        get
        {
            return _Notification;
        }
    }
    
    public IntermodalResult[] IntermodalRates
    {
        get
        {
            return _IntermodalRates;
        }
        set
        {
            _IntermodalRates = value;
        }
    }
	
    public IntermodalRateReply()
	{
		//
		// TODO: Add constructor logic here
		//
	}
}
