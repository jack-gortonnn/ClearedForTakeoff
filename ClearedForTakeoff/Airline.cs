using System.Collections.Generic;

public class Airline
{
    public string Code { get; set; }
    public string Callsign { get; set; }
    public Dictionary<string, int> Aircraft { get; set; } = new();
    public Airline(string code, string callsign, Dictionary<string, int> aircraft)
    {
        Code = code;
        Callsign = callsign;
        Aircraft = aircraft;
    }
}