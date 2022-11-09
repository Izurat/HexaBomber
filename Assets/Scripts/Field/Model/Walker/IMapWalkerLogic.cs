using System;

namespace Field.Model.Walker
{
    public interface IMapWalkerLogic
    {
        event Action WalkerTargetUpdated;
        E_WalkerType Type { get; }
        void GetNextTarget(int currentIndex, out int nextIndex, out float timeToTarget);
    }
}


