using Config;
using Field.Model.Bomb;
using Field.Model.Cell;
using System;
using System.Collections.Generic;
using Zenject;
using System.Linq;

namespace Field.Model.Walker
{
    public class RandomEnemyLogic : IMapWalkerLogic
    {
        public event Action WalkerTargetUpdated;
        public E_WalkerType Type => E_WalkerType.RandomEnemy;

        [Inject] private IFieldModel _cellsHolder;
        [Inject] private IBombsManager _bombHolder;

        private readonly float _timeToTarget = 0.5f;

        public RandomEnemyLogic(FieldConfig config)
        {
            _timeToTarget = config.WalkerTypes.FirstOrDefault(t => t.Type == Type).Speed;
        }

        public void GetNextTarget(int index, out int nextIndex, out float timeToTarget)
        {
            List<int> safeWalkableNeighbors = GetSafeWalkableNeighboursIndexes(index);

            timeToTarget = _timeToTarget;

            if (safeWalkableNeighbors.Count > 0)
                nextIndex = safeWalkableNeighbors[UnityEngine.Random.Range(0, safeWalkableNeighbors.Count)];
            else
                nextIndex = index;

            WalkerTargetUpdated.Invoke();
        }

        private List<int> GetSafeWalkableNeighboursIndexes(int centerIndex)
        {
            List<int> neighboursIndexes = _cellsHolder.GetNeighborsIndexes(centerIndex);
            neighboursIndexes.Remove(centerIndex);

            ICellModel currentNeighbour;
            List<int> result = new List<int>();

            foreach (int index in neighboursIndexes)
            {
                currentNeighbour = _cellsHolder.GetCellByIndex(index);

                if (currentNeighbour.type == E_CellType.Regular)
                {
                    if (!IsBombNearCell(index))
                        result.Add(index);
                }
            }

            return result;
        }

        private bool IsBombNearCell(int cellIndex)
        {
            List<int> neighboursIndexes = _cellsHolder.GetNeighborsIndexes(cellIndex);

            foreach (int index in neighboursIndexes)
            {
                if (_bombHolder.HasBomb(index))
                    return true;
            }

            return false;
        }
    }
}
