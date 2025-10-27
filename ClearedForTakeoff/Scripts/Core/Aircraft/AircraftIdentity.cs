public class AircraftIdentity
{
    public string AircraftType { get; set; }
    public string DisplayName { get; set; }
    public string AirlineCode { get; set; }
    public string AirlineName { get; set; }
    public string Callsign { get; set; }
    public string FlightNumber { get; set; }
    public string Destination { get; set; }
    public string AssignedGate { get; set; }
    public AircraftIdentity(string aircraftType, string displayName)
    {
        AircraftType = aircraftType;
        DisplayName = displayName;
    }
}