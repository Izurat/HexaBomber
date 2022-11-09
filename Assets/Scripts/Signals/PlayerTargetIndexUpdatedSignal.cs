namespace Signals
{
    public class PlayerTargetIndexUpdatedSignal
    {
        public readonly int Index;

        public PlayerTargetIndexUpdatedSignal(int index)
        {
            Index = index;
        }
    }
}
