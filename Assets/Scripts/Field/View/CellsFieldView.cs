using Field.Model.Cell;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;


namespace Field.View
{
    public class CellsFieldView : MonoBehaviour
    {
        [SerializeField] private CellView _cellViewPrefab;
        [SerializeField] private float _cellHorizontalOffset;
        [SerializeField] private float _cellVerticalOffset;

        [Inject] private IFieldModel _cellsHolder;
        [Inject] private SignalBus _signalBus;
        [Inject] private DiContainer _diContainer;

        private List<CellView> _cells;

        private Transform _transform;

        private void Start()
        {
            _transform = GetComponent<Transform>();

            _cells = new List<CellView>(_cellsHolder.Cells.Count);

            foreach (ICellModel currentCellModel in _cellsHolder.Cells)
                CreateCell(currentCellModel);
        }

        public Vector3 GetPositionByIndex(int index)
        {
            return GetViewByIndex(index).Position;
        }

        private void CreateCell(ICellModel cell)
        {
            GameObject viewGO = _diContainer.InstantiatePrefab(_cellViewPrefab);
            viewGO.transform.parent = _transform;

            CellView view = viewGO.GetComponent<CellView>();
            view.Init(cell);

            _cells.Add(view);

            _cellsHolder.GetPositionByIndex(cell.index, out int coloumn, out int row, out bool isOdd);

            UpdateCellPosition(view, coloumn, row, isOdd);
        }

        private CellView GetViewByIndex(int index)
        {
            return _cells.FirstOrDefault(c => c.Index == index);
        }

        private void UpdateCellPosition(CellView cell, int coloumn, int row, bool isOdd)
        {
            float x = coloumn * _cellHorizontalOffset;
            float y = row * _cellVerticalOffset;

            if (isOdd)
                x += _cellHorizontalOffset / 2;

            cell.SetFieldPosition(x, y);
        }
    }
}
