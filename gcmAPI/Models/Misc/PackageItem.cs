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
public class PackageItem
{
	public PackageItem()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    private int _Quantity = 0;
    private double _Weight = 0;    
    //private string _FreightClass = "-1";
    private string _ItemUnit = "";
    private bool _HazMat = false;
    private string _Tag = "";
    private double _Length = 0;
    private double _Width = 0;
    private double _Height = 0;
    private string _Commodity = "";
    
    private string _UnitOfMeasurement = "";

    private string _PackageType = "";
    public string[] additionalInfo;

    public string UnitOfMeasurement
    {
        get
        {
            return _UnitOfMeasurement;
        }
        set
        {
            _UnitOfMeasurement = value;
        }
    }

    public string PackageType
    {
        get
        {
            return _PackageType;
        }
        set
        {
            _PackageType = value;
        }
    }
    public int Quantity
    {
        get
        {
            return _Quantity;
        }
        set
        {
            _Quantity = value;
        }
    }
    public double Weight
    {
        get
        {
            return _Weight;
        }
        set
        {
            _Weight = value;
        }
    }
    public string ItemUnit
    {
        get
        {
            return _ItemUnit;
        }
        set
        {
            _ItemUnit = value;
        }
    }    
    //public string FreightClass
    //{
    //    get
    //    {
    //        return _FreightClass;
    //    }
    //    set
    //    {
    //        _FreightClass = value;
    //    }
    //}
    public bool HazMat
    {
        get
        {
            return _HazMat;
        }
        set
        {
            _HazMat = value;
        }
    }
    public string Tag
    {
        get
        {
            return _Tag;
        }
        set
        {
            _Tag = value;
        }
    }
    public double Length
    {
        get
        {
            return _Length;
        }
        set
        {
            _Length = value;
        }
    }
    public double Width
    {
        get
        {
            return _Width;
        }
        set
        {
            _Width = value;
        }
    }
    public double Height
    {
        get
        {
            return _Height;
        }
        set
        {
            _Height = value;
        }
    }
    public string Commodity
    {
        get
        {
            return _Commodity;
        }
        set
        {
            _Commodity = value;
        }
    }
}
