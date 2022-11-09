using Field.Model.Bomb;
using System;

namespace Field.Model.Walker
{
    public interface IMapWalker
    {
        event Action<int, float> OnTargetUpdated;
        event Action OnDisposed;
        UnicalID ID { get; }

        public int TargetIndex { get; }

        public int StartIndex { get; }

        public int CurrentIndex { get; }

        public E_WalkerType Type { get; }
    }

    public class MapWalkerModel : IMapWalker, ITimeUpdateableModel
    {
        public UnicalID ID { get; private set; }

        public event Action<int, float> OnTargetUpdated;
        public event Action OnDisposed;

        public float TimeToTarget { get; private set; }
        public int TargetIndex { get; private set; }
        public int StartIndex { get; private set; }

        public E_WalkerType Type => _logic.Type;

        public int CurrentIndex
        {
            get
            {
                if (_initialTimeToTarget / TimeToTarget >= 2)
                    return TargetIndex;

                return StartIndex;
            }
        }

        private readonly IMapWalkerLogic _logic;

        private float _initialTimeToTarget;

        public MapWalkerModel(IMapWalkerLogic logic, int startIndex)
        {
            _logic = logic;

            StartIndex = startIndex;
            TargetIndex = startIndex;

            ID = UnicalID.Create();
        }

        public void Update(float deltaTime)
        {
            TimeToTarget -= deltaTime;

            if (TimeToTarget <= 0)
            {
                StartIndex = TargetIndex;

                _logic.GetNextTarget(StartIndex, out int newIndex, out float timeToTarget);

                TargetIndex = newIndex;
                TimeToTarget = timeToTarget;
                _initialTimeToTarget = timeToTarget;

                OnTargetUpdated?.Invoke(newIndex, timeToTarget);
            }
        }

        public void Dispose()
        {
            OnTargetUpdated = null;
            OnDisposed?.Invoke();
            OnDisposed = null;
        }
    }
}
