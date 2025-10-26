using System.Collections.Generic;

// worth noting that we don't need a constructor since contentmanager does it for us
public class Airline
{
    public string Name { get; init; } = "";
    public string Callsign { get; init; } = "";
    public string Code { get; init; } = "";
    public int Frequency { get; init; } = 0;
    public Dictionary<string, List<string>> Routes { get; init; } = new();
    public Dictionary<string, int> Fleet { get; init; } = new();
}
