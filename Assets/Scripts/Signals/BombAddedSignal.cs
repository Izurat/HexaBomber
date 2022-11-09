

using Field.Model.Bomb;

namespace Signals
{
    public class BombAddedSignal
    {
        public IBombModel Bomb { get; private set; }

        public BombAddedSignal(IBombModel bomb)
        {
            Bomb = bomb;
        }
    }
}
