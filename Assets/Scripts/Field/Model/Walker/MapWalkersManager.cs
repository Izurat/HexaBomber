using Config;
using Field.Model.Cell;
using Signals;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Field.Model.Walker
{
    public interface IMapWalkersManager
    {
        event Action<IMapWalker> OnMapWalkerAdded;
        event Action<IMapWalker> OnMapWalkerRemoved;
        List<IMapWalker> Walkers { get; }
    }

    public class MapWalkersManager : IMapWalkersManager, ITickable
    {
        public event Action<IMapWalker> OnMapWalkerAdded;
        public event Action<IMapWalker> OnMapWalkerRemoved;

        private SignalBus _signaBus;
        private IFieldModel _cellsHolder;

        private List<MapWalkerModel> _mapWalkers;

        private Dictionary<E_WalkerType, IMapWalkerLogic> _waklersLogics;

        public MapWalkersManager(
            FieldConfig config,
            SignalBus signalBus,
            IFieldModel cellsHolder,
            RandomEnemyLogic randomEnemyLogic,
            UserGuidedLogic userGuidedLogic,
            TargetEnemyLogic targetEnemyLogic)
        {
            _signaBus = signalBus;
            _cellsHolder = cellsHolder;

            _waklersLogics = new Dictionary<E_WalkerType, IMapWalkerLogic>();
            _waklersLogics[randomEnemyLogic.Type] = randomEnemyLogic;
            _waklersLogics[userGuidedLogic.Type] = userGuidedLogic;
            _waklersLogics[targetEnemyLogic.Type] = targetEnemyLogic;

            _mapWalkers = new List<MapWalkerModel>();

            signalBus.Subscribe<BombExplodedSignal>(OnBombExplode);

            foreach (WalkerSpawnPointData spawnData in config.WalkerSpawnPoints)
                AddMapWalker(spawnData.Type, spawnData.StartIndex);
        }

        public void AddMapWalker(E_WalkerType type, int startIndex)
        {
            IMapWalkerLogic logic = _waklersLogics[type];
            MapWalkerModel newWalker = new MapWalkerModel(logic, startIndex);
            _mapWalkers.Add(newWalker);
            OnMapWalkerAdded?.Invoke(newWalker);
        }

        public void Tick()
        {
            foreach (ITimeUpdateableModel walker in _mapWalkers)
                walker.Update(Time.deltaTime);
        }

        public List<IMapWalker> Walkers
        {

            //!TODO refactor this
            get
            {
                List<IMapWalker> result = new List<IMapWalker>();

                foreach (IMapWalker currentWalker in _mapWalkers)
                    result.Add(currentWalker);

                return result;
            }
        }

        private void OnBombExplode(BombExplodedSignal signal)
        {
            List<int> affectedCellsIndexes = _cellsHolder.GetNeighborsIndexes(signal.Index);

            List<MapWalkerModel> affectedWalkers = new List<MapWalkerModel>();

            foreach (MapWalkerModel walker in _mapWalkers)
            {
                if (affectedCellsIndexes.IndexOf(walker.TargetIndex) >= 0)
                    affectedWalkers.Add(walker);
            }

            foreach (MapWalkerModel walker in affectedWalkers)
                RemoveWalker(walker);
        }

        private void RemoveWalker(MapWalkerModel mapWalker)
        {
            OnMapWalkerRemoved?.Invoke(mapWalker);

            mapWalker.Dispose();
            _mapWalkers.Remove(mapWalker);
        }
    }
}