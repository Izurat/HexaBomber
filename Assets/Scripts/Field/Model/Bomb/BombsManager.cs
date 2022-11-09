using Config;
using Signals;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Field.Model.Bomb
{
    public interface IBombsManager
    {
        bool HasBomb(int cellIndex);
        List<IBombModel> Bombs { get; }
        List<int> GetBombIndexes();
    }

    public class BombsManager : IBombsManager, ITickable
    {
        private readonly SignalBus _signalBus;

        private List<BombModel> _bombs;

        private readonly float _bombDelay;

        public BombsManager(FieldConfig config, SignalBus signalBus)
        {
            _signalBus = signalBus;
            _bombs = new List<BombModel>();

            _signalBus.Subscribe<AddBombSignal>(AddBomb);

            _bombDelay = config.BombDelay;
        }

        private void AddBomb(AddBombSignal signal)
        {
            SetBomb(signal.Index, _bombDelay);
        }

        public void SetBomb(int index, float time)
        {
            BombModel newBomb = new BombModel(index, time);
            _bombs.Add(newBomb);

            _signalBus.Fire(new BombAddedSignal(newBomb));
        }

        public void Tick()
        {
            List<BombModel> bombsExplodeOnTick = new List<BombModel>(); ;

            foreach (BombModel bomb in _bombs)
            {
                bomb.Update(Time.deltaTime);

                if (bomb.TimeLeft <= 0)
                    bombsExplodeOnTick.Add(bomb);
            }

            foreach (BombModel bomb in bombsExplodeOnTick)
            {
                _bombs.Remove(bomb);
                _signalBus.Fire(new BombExplodedSignal(bomb.Index, bomb.Id));
            }
        }

        public bool HasBomb(int cellIndex)
        {
            return _bombs.Where(b => b.Index == cellIndex).FirstOrDefault() != null;
        }

        public List<int> GetBombIndexes() => _bombs.Select(b => b.Index).ToList();

        public List<IBombModel> Bombs
        {
            //TODO refactor this
            get
            {
                List<IBombModel> bombs = new List<IBombModel>();

                foreach (BombModel bomb in _bombs)
                    bombs.Add(bomb);

                return bombs;
            }
        }
    }
}