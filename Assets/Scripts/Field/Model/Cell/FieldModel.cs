using Config;
using Signals;
using System;
using System.Collections.Generic;
using Zenject;

namespace Field.Model.Cell
{
    public interface IFieldModel
    {
        int FieldWidth { get; }
        int FieldHeight { get; }
        IReadOnlyList<ICellModel> Cells { get; }
        ICellModel GetCellByIndex(int index);
        List<int> GetNeighborsIndexes(int index);
        void GetPositionByIndex(int index, out int coloumn, out int row, out bool isOdd);
        ICellModel GetNeighborByDirection(int index, E_DirectionType direction);
        IReadOnlyDictionary<E_DirectionType, ICellModel> GetNeighboursWithDirections(int index);

        List<E_DirectionType> GetDirectionsBetweenIndexes(int startIndex, int targetIndex);
    }

    public class FieldModel : IFieldModel
    {
        private SignalBus _signalBus;

        private Dictionary<int, CellModel> _cellsDictionary;

        private FieldConfig _config;

        public int FieldWidth => _config.FieldWidth;
        public int FieldHeight => _config.FieldHeight;
        public IReadOnlyList<ICellModel> Cells { get; private set; }

        public FieldModel(FieldConfig config, SignalBus signalBus)
        {
            _config = config;

            _signalBus = signalBus;

            int cellsCount = FieldWidth * FieldHeight;

            _cellsDictionary = new Dictionary<int, CellModel>();
            List<ICellModel> cellsList = new List<ICellModel>(cellsCount);

            CellModel currentCell;

            for (int i = 0; i < cellsCount; i++)
            {
                currentCell = CreateCell(E_CellType.Regular, i);
                cellsList.Add(currentCell);
                _cellsDictionary[i] = currentCell;
            }

            Cells = cellsList;

            signalBus.Subscribe<BombExplodedSignal>(OnBombExplode);

            foreach (WallData wall in _config.Walls)
                _cellsDictionary[wall.Index].SetType(wall.Type);

        }

        public ICellModel GetCellByIndex(int index)
        {
            return _cellsDictionary[index];
        }

        public List<int> GetNeighborsIndexes(int index)
        {
            GetPositionByIndex(index, out int coloumn, out int row, out bool isOdd);

            List<int> result = new List<int>();

            if (!isOdd)
            {
                TryAddNeighbour(coloumn - 1, row - 1, ref result);
                TryAddNeighbour(coloumn - 1, row + 1, ref result);
            }
            else
            {
                TryAddNeighbour(coloumn + 1, row - 1, ref result);
                TryAddNeighbour(coloumn + 1, row + 1, ref result);
            }

            TryAddNeighbour(coloumn + 1, row, ref result);
            TryAddNeighbour(coloumn - 1, row, ref result);
            TryAddNeighbour(coloumn, row + 1, ref result);
            TryAddNeighbour(coloumn, row - 1, ref result);

            result.Add(index);

            return result;
        }

        private void TryAddNeighbour(int coloumn, int row, ref List<int> indexes)
        {
            int currentNeighbourIndex = -1;

            if (TryGetIndex(coloumn, row, out currentNeighbourIndex))
                indexes.Add(currentNeighbourIndex);
        }

        public void GetPositionByIndex(int index, out int coloumn, out int row, out bool isOdd)
        {
            row = Math.DivRem(index, FieldWidth, out coloumn);

            int odd;
            Math.DivRem(row, 2, out odd);
            isOdd = !(odd > 0) || row == 0;
        }

        public IReadOnlyDictionary<E_DirectionType, ICellModel> GetNeighboursWithDirections(int index)
        {

            Dictionary<E_DirectionType, ICellModel> result = new Dictionary<E_DirectionType, ICellModel>();

            foreach (E_DirectionType direction in Enum.GetValues(typeof(E_DirectionType)))
            {
                ICellModel neighbour = GetCellByIndexAndDirection(index, direction);

                if (neighbour != null)
                    result[direction] = neighbour;
            }

            return result;

        }

        public ICellModel GetNeighborByDirection(int index, E_DirectionType direction)
        {
            ICellModel neighbour = GetCellByIndexAndDirection(index, direction);

            if (neighbour == null)
                neighbour = GetCellByIndex(index);

            return neighbour;
        }

        public List<E_DirectionType> GetDirectionsBetweenIndexes(int startIndex, int targetIndex)
        {
            List<E_DirectionType> result = new List<E_DirectionType>();

            if (startIndex == targetIndex)
                result.Add(E_DirectionType.None);
            else
            {
                GetPositionByIndex(startIndex, out int startColoumn, out int startRow, out bool isStartOdd);
                GetPositionByIndex(targetIndex, out int targetColoumn, out int targetRow, out bool isTargetOdd);

                if (startRow == targetRow)
                {
                    if (startColoumn < targetColoumn)
                    {
                        result.Add(E_DirectionType.Right);
                        result.Add(E_DirectionType.UpRight);
                        result.Add(E_DirectionType.DownRight);
                    }
                    else
                    {
                        result.Add(E_DirectionType.Left);
                        result.Add(E_DirectionType.UpLeft);
                        result.Add(E_DirectionType.DownLeft);
                    }
                }
                else if (startRow < targetRow)
                {
                    if (startColoumn < targetColoumn)
                    {
                        result.Add(E_DirectionType.UpRight);
                        result.Add(E_DirectionType.Right);

                    }
                    else
                    {
                        result.Add(E_DirectionType.UpLeft);
                        result.Add(E_DirectionType.Left);
                    }
                }
                else
                {
                    if (startColoumn < targetColoumn)
                    {
                        result.Add(E_DirectionType.DownRight);
                        result.Add(E_DirectionType.Right);

                    }
                    else
                    {
                        result.Add(E_DirectionType.DownLeft);
                        result.Add(E_DirectionType.Left);
                    }
                }
            }

            return result;
        }

        private ICellModel GetCellByIndexAndDirection(int index, E_DirectionType direction)
        {
            GetPositionByIndex(index, out int coloumn, out int row, out bool isOdd);

            int nextColoumn = coloumn;
            int nextRow = row;

            switch (direction)
            {
                case E_DirectionType.None:
                    return GetCellByIndex(index);
                case E_DirectionType.Left:
                    nextColoumn -= 1;
                    break;
                case E_DirectionType.Right:
                    nextColoumn += 1;
                    break;
                case E_DirectionType.UpRight:
                    nextRow += 1;
                    if (isOdd)
                        nextColoumn += 1;
                    break;
                case E_DirectionType.UpLeft:
                    nextRow += 1;
                    if (!isOdd)
                        nextColoumn -= 1;
                    break;
                case E_DirectionType.DownRight:
                    nextRow -= 1;
                    if (isOdd)
                        nextColoumn += 1;
                    break;
                case E_DirectionType.DownLeft:
                    nextRow -= 1;
                    if (!isOdd)
                        nextColoumn -= 1;
                    break;

                default:
                    break;
            }

            int resultIndex = 0;

            if (TryGetIndex(nextColoumn, nextRow, out resultIndex))
                return GetCellByIndex(resultIndex);

            return null;
        }

        private bool TryGetIndex(int coloumn, int row, out int index)
        {
            index = FieldWidth * row + coloumn;

            return coloumn >= 0 && coloumn < FieldWidth && row >= 0 && row < FieldHeight;
        }

        private void OnBombExplode(BombExplodedSignal signal)
        {
            List<int> affectedCellsIndexes = GetNeighborsIndexes(signal.Index);

            foreach (int index in affectedCellsIndexes)
                ExplodeCell(index);
        }

        private void ExplodeCell(int cellIndex)
        {
            CellModel cell = _cellsDictionary[cellIndex];

            if (cell.type == E_CellType.BreakableWall)
                cell.SetType(E_CellType.Regular);
        }

        private CellModel CreateCell(E_CellType type, int index)
        {
            CellModel result = new CellModel(type, index);
            return result;
        }
    }
}
