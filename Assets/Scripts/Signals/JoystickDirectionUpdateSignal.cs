
namespace Signals
{

    public class JoystickDirectionUpdateSignal
    {
        public readonly E_DirectionType Direction;

        public JoystickDirectionUpdateSignal(E_DirectionType direction)
        {
            Direction = direction;
        }
    }
}
