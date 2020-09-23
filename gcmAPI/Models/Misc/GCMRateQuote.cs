using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for GCMRateQuote
/// </summary>
public class GCMRateQuote
{
    private double _TotalPrice;
    private string _DisplayName;
    private string _BookingKey;
    private string _CarrierKey;
    private string _Scac;
    private int _DeliveryDays;
    private double _GuaranteedRateAM = -1;
    private double _GuaranteedRatePM = -1;
    private string _Documentation;
    private double _OurRate;
    private double _OurRateGAM = -1;
    private double _OurRateGPM = -1;
    private double _CoverageCost;
    private double _TotalLineHaul;
    private string _RulesTarrif;
    private string _RateId;
    private int _NewLogId;
    private string _RateType;
    private string _OnTimePercent = "N/A";
    private int _ShipsBetweenStates = 0;
    private string _cost_breakdown;
    private double _base_rate;
    private string _CarrierQuoteID;
    private int _Elapsed_milliseconds;
    private string _BillTo;
    private string _API_BookingKey;

    public GCMRateQuote()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public double TotalPrice
    {
        get { return _TotalPrice; }
        set { _TotalPrice = value; }
    }
    public string DisplayName
    {
        get { return _DisplayName; }
        set { _DisplayName = value; }
    }
    public string BookingKey
    {
        get { return _BookingKey; }
        set { _BookingKey = value; }
    }
    public string CarrierKey
    {
        get { return _CarrierKey; }
        set { _CarrierKey = value; }
    }
    public string Scac
    {
        get { return _Scac; }
        set { _Scac = value; }
    }
    public int DeliveryDays
    {
        get { return _DeliveryDays; }
        set { _DeliveryDays = value; }
    }
    public double GuaranteedRateAM
    {
        get { return _GuaranteedRateAM; }
        set { _GuaranteedRateAM = value; }
    }
    public double GuaranteedRatePM
    {
        get { return _GuaranteedRatePM; }
        set { _GuaranteedRatePM = value; }
    }
    public string Documentation
    {
        get { return _Documentation; }
        set { _Documentation = value; }
    }
    public double OurRate
    {
        get { return _OurRate; }
        set { _OurRate = value; }
    }
    public double OurRateGAM
    {
        get { return _OurRateGAM; }
        set { _OurRateGAM = value; }
    }
    public double OurRateGPM
    {
        get { return _OurRateGPM; }
        set { _OurRateGPM = value; }
    }
    public double CoverageCost
    {
        get { return _CoverageCost; }
        set { _CoverageCost = value; }
    }
    public double TotalLineHaul
    {
        get { return _TotalLineHaul; }
        set { _TotalLineHaul = value; }
    }
    public string RulesTarrif
    {
        get { return _RulesTarrif; }
        set { _RulesTarrif = value; }
    }
    public string RateId
    {
        get { return _RateId; }
        set { _RateId = value; }
    }
    public int NewLogId
    {
        get { return _NewLogId; }
        set { _NewLogId = value; }
    }
    public string RateType
    {
        get { return _RateType; }
        set { _RateType = value; }
    }
    public string OnTimePercent
    {
        get { return _OnTimePercent; }
        set { _OnTimePercent = value; }
    }
    public int ShipsBetweenStates
    {
        get { return _ShipsBetweenStates; }
        set { _ShipsBetweenStates = value; }
    }
    public string Cost_breakdown
    {
        get { return _cost_breakdown; }
        set { _cost_breakdown = value; }
    }
    public double base_rate
    {
        get { return _base_rate; }
        set { _base_rate = value; }
    }
    public string CarrierQuoteID
    {
        get { return _CarrierQuoteID; }
        set { _CarrierQuoteID = value; }
    }
    public int Elapsed_milliseconds
    {
        get { return _Elapsed_milliseconds; }
        set { _Elapsed_milliseconds = value; }
    }
    public string BillTo
    {
        get { return _BillTo; }
        set { _BillTo = value; }
    }
    public string API_BookingKey
    {
        get { return _API_BookingKey; }
        set { _API_BookingKey = value; }
    }
}