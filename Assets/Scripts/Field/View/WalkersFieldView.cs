using Field.Model.Walker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace Field.View
{
    public class WalkersFieldView : MonoBehaviour
    {
        [SerializeField] private CellsPositionsWrapper _cellPositionHolder;
        [SerializeField] private Transform _containerTransform;
        [SerializeField] private WalkerView _walkerPrefab;

        [Inject] private IMapWalkersManager _holder;
        [Inject] private SignalBus _signalBus;
        [Inject] private DiContainer _diContainer;

        private List<WalkerView> _walkers;

        private Transform _transform;
        private Transform Transform
        {
            get
            {
                if (_transform == null)
                    _transform = transform;

                return _transform;
            }
        }

        private void Start()
        {
            _walkers = new List<WalkerView>();

            //wait for cell views creation;
            StartCoroutine("InitWalkersViews");
        }

        private IEnumerator InitWalkersViews()
        {
            yield return new WaitForSeconds(0.2f);

            List<IMapWalker> walkers = _holder.Walkers;

            foreach (IMapWalker walker in walkers)
                CreateWalkerView(walker);
        }

        private void CreateWalkerView(IMapWalker walker)
        {
            GameObject viewGO = _diContainer.InstantiatePrefab(_walkerPrefab);
            viewGO.transform.parent = Transform;

            WalkerView view = viewGO.GetComponent<WalkerView>();
            view.Init(walker, _cellPositionHolder);

            view.Disposed += WalkerDisposed;

            _walkers.Add(view);
        }

        private void WalkerDisposed(WalkerView walker)
        {
            _walkers.Remove(walker);
            Destroy(walker.gameObject);
        }
    }
}