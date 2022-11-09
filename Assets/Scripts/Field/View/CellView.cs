
using Field.Model.Cell;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Field.View
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private Transform _wallHolder;

        [SerializeField] private List<CellTypeViewData> _typeViews;

        private ICellModel _cellModel;

        private GameObject _currentStateView;

        private Transform _transform;
        private Transform ContainerTransform
        {
            get
            {
                if (_transform == null)
                    _transform = transform;

                return _transform;
            }
        }

        [Serializable]
        public class CellTypeViewData
        {
            public E_CellType state;
            public GameObject gameObject;
        }

        internal int Index
        {
            get
            {
                if (_cellModel == null)
                    return -1;

                return _cellModel.index;
            }
        }

        internal Vector3 Position
        {
            get => ContainerTransform.position;
        }

        internal void Init(ICellModel cellModel)
        {
            _cellModel = cellModel;
            DisplayModelState(_cellModel.type);
            _cellModel.OnTypeChanged += DisplayModelState;
        }

        internal void SetFieldPosition(float coloumn, float row)
        {
            ContainerTransform.position = new Vector3(coloumn, 0, row);
        }

        private void DisplayModelState(E_CellType state)
        {
            if (_currentStateView != null)
            {
                Destroy(_currentStateView);
                _currentStateView = null;
            }

            CellTypeViewData stateView = _typeViews.Where(s => s.state == state).FirstOrDefault();

            if (stateView.gameObject != null)
                _currentStateView = Instantiate(stateView.gameObject, _wallHolder);
        }
    }
}
