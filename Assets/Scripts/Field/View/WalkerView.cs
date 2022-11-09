using DG.Tweening;
using Field.Model.Walker;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Field.View
{
    public class WalkerView : MonoBehaviour
    {
        public event Action<WalkerView> Disposed;

        [SerializeField] private List<WalkerTypeViewData> _typeViews;

        private IMapWalker _walker;

        private CellsPositionsWrapper _positionsHolder;

        private GameObject _typeView;
        private Transform _transform;

        private Tweener _currentMover;
        private Tweener _currentRotator;

        [Serializable]
        public class WalkerTypeViewData
        {
            public E_WalkerType type;
            public GameObject gameObject;
        }

        public void Init(IMapWalker walker, CellsPositionsWrapper positionsHolder)
        {
            _transform = transform;
            _walker = walker;
            _positionsHolder = positionsHolder;

            DisplayModelType(_walker.Type);

            SetPositionByIndex(walker.CurrentIndex);

            _walker.OnTargetUpdated += OnTargetUpdated;

            _walker.OnDisposed += OnDisposed;
        }

        private void OnTargetUpdated(int newTarget, float timeToTarget)
        {
            MoveToIndex(newTarget, timeToTarget);
        }

        private void SetPositionByIndex(int index)
        {
            _transform.position = _positionsHolder.GetPositionByIndex(index);
        }

        private void MoveToIndex(int index, float time)
        {
            if (_currentMover != null)
                _currentMover.Kill();

            if (_currentRotator != null)
                _currentRotator.Kill();

            _currentMover = _transform.DOMove(_positionsHolder.GetPositionByIndex(index), time);
            _currentRotator = _transform.DOLookAt(_positionsHolder.GetPositionByIndex(index), 0.1f);
        }

        private void DisplayModelType(E_WalkerType type)
        {
            if (_typeView != null)
            {
                Destroy(_typeView);
                _typeView = null;
            }

            foreach (WalkerTypeViewData view in _typeViews)
            {
                if (view.type == type)
                {
                    if (view.gameObject != null)
                        _typeView = Instantiate(view.gameObject, _transform);

                    break;
                }
            }
        }

        private void OnDisposed()
        {
            Destroy(_typeView);
            _typeView = null;

            _currentMover.Kill();
            _currentRotator.Kill();

            Disposed.Invoke(this);
            Disposed = null;
        }
    }
}
