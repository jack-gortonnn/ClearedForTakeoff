using System.Collections.Generic;

public class Airline
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Callsign { get; set; }
    public int Frequency { get; set; }
    public Dictionary<string, int> Fleet { get; set; } = new();
    public Dictionary<string, List<string>> Routes { get; set; } = new();
    public Airline(string name, string code, string callsign, int frequency, Dictionary<string, int> fleet, Dictionary<string,List<string>> routes)
    {
        Name = name;
        Code = code;
        Callsign = callsign;
        Fleet = fleet;
        Frequency = frequency;
        Routes = routes;
    }
}