using Config;
using Field.Model.Cell;
using Signals;
using System;
using Zenject;
using System.Linq;

namespace Field.Model.Walker
{
    public class UserGuidedLogic : IMapWalkerLogic
    {
        public event Action WalkerTargetUpdated;
        public E_WalkerType Type => E_WalkerType.Player;

        private IFieldModel _cellsHolder;
        private SignalBus _signalBus;

        private E_DirectionType _currentDirection = E_DirectionType.None;

        private int _lastReceivedIndex;

        private readonly float _timeToTarget;

        public UserGuidedLogic(FieldConfig config, SignalBus signalBus, IFieldModel cellsHolder)
        {
            _cellsHolder = cellsHolder;
            _signalBus = signalBus;

            _signalBus.Subscribe<JoystickDirectionUpdateSignal>(OnDirectionUpdate);
            _signalBus.Subscribe<SetBombInputSignal>(OnBombInCurrentPositionInput);

            _timeToTarget = config.WalkerTypes.FirstOrDefault(t => t.Type == Type).Speed;
        }

        public void GetNextTarget(int currentIndex, out int nextIndex, out float timeToTarget)
        {
            ICellModel nextCell = _cellsHolder.GetNeighborByDirection(currentIndex, _currentDirection);

            if (nextCell.type != E_CellType.Regular)
                nextIndex = currentIndex;
            else
                nextIndex = nextCell.index;
            //nextIndex = _cellsHolder.GetNextIndexByDirection(currentIndex, _currentDirection);
            timeToTarget = _timeToTarget;
            _lastReceivedIndex = nextIndex;

            WalkerTargetUpdated?.Invoke();

            _signalBus.Fire<PlayerTargetIndexUpdatedSignal>(new PlayerTargetIndexUpdatedSignal(nextIndex));
        }

        private void OnDirectionUpdate(JoystickDirectionUpdateSignal signal)
        {
            _currentDirection = signal.Direction;
        }

        private void OnBombInCurrentPositionInput()
        {
            _signalBus.Fire<AddBombSignal>(new AddBombSignal(_lastReceivedIndex));
        }
    }
}
