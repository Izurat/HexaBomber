

namespace Signals
{
    public class BombExplodedSignal
    {
        public readonly int Index;
        public readonly UnicalID Id;

        public BombExplodedSignal(int index, UnicalID id)
        {
            Index = index;
            Id = id;
        }
    }
}