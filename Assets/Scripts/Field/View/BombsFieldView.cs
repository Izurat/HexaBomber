using Field.Model.Bomb;
using Signals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Field.View
{
    public class BombsFieldView : MonoBehaviour
    {
        [SerializeField] private CellsPositionsWrapper _cellPositionHolder;
        [SerializeField] private BombView _bombViewPrefab;

        [Inject] private IBombsManager _bombHolder;
        [Inject] private SignalBus _signalBus;
        [Inject] private DiContainer _diContainer;

        private Transform _transform;
        private List<BombView> _bombViews;

        private void Start()
        {
            _transform = GetComponent<Transform>();
            _bombViews = new List<BombView>();

            //wait for cell views creation;
            StartCoroutine("InitBombsViews");
        }

        private IEnumerator InitBombsViews()
        {
            yield return new WaitForSeconds(0.2f);

            foreach (IBombModel bomb in _bombHolder.Bombs)
                CreateBomb(bomb);

            _signalBus.Subscribe<BombExplodedSignal>(OnBombExplode);
            _signalBus.Subscribe<BombAddedSignal>(OnBombAdded);
        }

        private void CreateBomb(IBombModel bomb)
        {
            GameObject viewGO = _diContainer.InstantiatePrefab(_bombViewPrefab);
            viewGO.transform.parent = _transform;

            BombView view = viewGO.GetComponent<BombView>();
            view.Init(bomb);

            _bombViews.Add(view);

            Vector3 pos = _cellPositionHolder.GetPositionByIndex(bomb.Index);

            view.transform.position = pos;
        }

        private void OnBombExplode(BombExplodedSignal signal)
        {
            RemoveBombViewByID(signal.Id);
        }

        private void RemoveBombViewByID(UnicalID id)
        {
            foreach (BombView bomb in _bombViews)
            {
                if (bomb.Id.Equals(id))
                {
                    _bombViews.Remove(bomb);
                    Destroy(bomb.gameObject);
                    return;
                }
            }
        }

        private void OnBombAdded(BombAddedSignal signal)
        {
            CreateBomb(signal.Bomb);
        }
    }
}
