#region Using

using System;

#endregion

/// <summary>
/// Summary description for IntermodalResult
/// </summary>
public class IntermodalResult
{
    //private string _BookingKey = "";

    private string _FirstAvailablePickupDate = "";
    private string _EstimatedDeliveryDate = "";
    private string _RateValidThrough = "";
    
    private string _EstimatedTransitTime = "";   
    private double _TotalCharge = 0;

    private string _OriginCityState = "";  
    private string _OriginCountry = "";
    private string _DestinationCityState = "";
    private string _DestinationCountry = "";

    private string _ContainerSize = "";
    private string _Carrier = "";
    //private string _ConfidenceRate = "";
    //private string _ConfidenceGroup = "";

    public string[] additionalInfo;

	public IntermodalResult()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    //public string ConfidenceGroup
    //{
    //    set
    //    {
    //        _ConfidenceGroup = value;
    //    }
    //    get
    //    {
    //        return _ConfidenceGroup;
    //    }
    //}

    //public string ConfidenceRate
    //{
    //    set
    //    {
    //        _ConfidenceRate = value;
    //    }
    //    get
    //    {
    //        return _ConfidenceRate;
    //    }
    //}

    public string Carrier
    {
        set
        {
            _Carrier = value;
        }
        get
        {
            return _Carrier;
        }
    }

    public string EstimatedTransitTime
    {
        set
        {
            _EstimatedTransitTime = value;
        }
        get
        {
            return _EstimatedTransitTime;
        }
    }

    public string OriginCityState
    {
        set
        {
            _OriginCityState = value;
        }
        get
        {
            return _OriginCityState;
        }
    }
  
    public string OriginCountry
    {
        set
        {
            _OriginCountry = value;
        }
        get
        {
            return _OriginCountry;
        }
    }

    public string DestinationCityState
    {
        set
        {
            _DestinationCityState = value;
        }
        get
        {
            return _DestinationCityState;
        }
    }

    public string DestinationCountry
    {
        set
        {
            _DestinationCountry = value;
        }
        get
        {
            return _DestinationCountry;
        }
    }

    public double TotalCharge
    {
        set
        {
            _TotalCharge = value;
        }
        get
        {
            return _TotalCharge;
        }
    }

    public string FirstAvailablePickupDate
    {
        set
        {
            _FirstAvailablePickupDate = value;
        }
        get
        {
            return _FirstAvailablePickupDate;
        }
    }

    public string EstimatedDeliveryDate
    {
        set
        {
            _EstimatedDeliveryDate = value;
        }
        get
        {
            return _EstimatedDeliveryDate;
        }
    }

    public string RateValidThrough
    {
        set
        {
            _RateValidThrough = value;
        }
        get
        {
            return _RateValidThrough;
        }
    }

    public string ContainerSize
    {
        set
        {
            _ContainerSize = value;
        }
        get
        {
            return _ContainerSize;
        }
    }

}
