using Config;
using Field.Model.Bomb;
using Field.Model.Cell;
using Signals;
using System;
using System.Collections.Generic;
using Zenject;
using System.Linq;

namespace Field.Model.Walker
{
    public class TargetEnemyLogic : IMapWalkerLogic
    {
        public event Action WalkerTargetUpdated;
        public E_WalkerType Type => E_WalkerType.TargetEnemy;

        [Inject] private IFieldModel _cellsHolder;
        [Inject] private IBombsManager _bombHolder;

        private readonly float _timeToTarget;
        private readonly SignalBus _signalBus;

        private int _targetIndex;

        public TargetEnemyLogic(FieldConfig config, SignalBus signalBus)
        {
            _signalBus = signalBus;
            _signalBus.Subscribe<PlayerTargetIndexUpdatedSignal>(UpdateTarget);
            _timeToTarget = config.WalkerTypes.FirstOrDefault(t => t.Type == Type).Speed;
        }

        public void GetNextTarget(int index, out int nextIndex, out float timeToTarget)
        {
            List<E_DirectionType> directions = _cellsHolder.GetDirectionsBetweenIndexes(index, _targetIndex);

            ICellModel neighbour;
            List<int> acceptableTargetIndexes = new List<int>();

            foreach (E_DirectionType direction in directions)
            {
                neighbour = _cellsHolder.GetNeighborByDirection(index, direction);

                if (neighbour.type == E_CellType.Regular)
                    acceptableTargetIndexes.Add(neighbour.index);
            }

            acceptableTargetIndexes.Add(index);

            if (acceptableTargetIndexes.Count > 0)
                nextIndex = acceptableTargetIndexes[0];
            else
                nextIndex = index;

            timeToTarget = _timeToTarget;
        }

        private void UpdateTarget(PlayerTargetIndexUpdatedSignal signal)
        {
            _targetIndex = signal.Index;
        }
    }
}
