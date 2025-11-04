using Microsoft.Xna.Framework.Input;

class RadioManager
{
    public static string CheckRadioCommands(Aircraft _selectedPlane, string radioMessage)
    {
        if (_selectedPlane == null)
            radioMessage = "";

        if (InputManager.Pressed(Keys.P))
        {
            if (_selectedPlane.State.CurrentState != AircraftState.AtGate)
            {
                radioMessage = "Can't tell a plane to pushback that isn't at a gate!";
            }
            else
            {
                _selectedPlane.State.SetState(AircraftState.PushingBack);
                radioMessage = $"{_selectedPlane.Identity.FlightNumber}, cleared for pushback, stand {_selectedPlane.Identity.AssignedGate}, tail {_selectedPlane.Movement.tailFacing}.";
            }
        }

        if (InputManager.Pressed(Keys.T))
        {
            if (_selectedPlane.State.CurrentState != AircraftState.HoldingPosition)
            {
                radioMessage = "Can't tell a plane to taxi that isn't holding position!";
            }
            else
            {
                _selectedPlane.State.SetState(AircraftState.Taxiing);
                radioMessage = $"{_selectedPlane.Identity.FlightNumber}, cleared for taxi.";
            }
        }

        return radioMessage;
    }
}
