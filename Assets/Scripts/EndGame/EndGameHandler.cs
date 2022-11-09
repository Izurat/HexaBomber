using Field.Model.Walker;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EndGame
{
    public interface IEndGameHandler
    {
        event Action OnWin;
        event Action OnLose;
    }
    public class EndGameHandler : IEndGameHandler
    {
        public event Action OnWin;
        public event Action OnLose;

        private IMapWalkersManager _walkersHolder;

        public EndGameHandler(IMapWalkersManager walkersHolder, UserGuidedLogic userLogic, RandomEnemyLogic randomLogic)
        {
            _walkersHolder = walkersHolder;

            userLogic.WalkerTargetUpdated += OnTargetUpdated;
            randomLogic.WalkerTargetUpdated += OnTargetUpdated;

            walkersHolder.OnMapWalkerRemoved += OnMapWalkerRemoved;
        }

        private void OnTargetUpdated()
        {
            List<IMapWalker> walkers = _walkersHolder.Walkers;

            IMapWalker player = walkers.Where(w => w.Type == E_WalkerType.Player).FirstOrDefault();

            if (player == null)
            {
                OnLose?.Invoke();
                return;
            }

            int enemiesCounter = 0;

            walkers.Where(w => w.Type == E_WalkerType.RandomEnemy || w.Type == E_WalkerType.TargetEnemy).ToList().ForEach(
                w =>
                {
                    enemiesCounter++;

                    if (w.TargetIndex == player.TargetIndex)
                    {
                        OnLose?.Invoke();
                        return;
                    }
                });

            if (enemiesCounter == 0)
                OnWin.Invoke();
        }

        private void OnMapWalkerRemoved(IMapWalker removedWalker)
        {
            if (removedWalker.Type == E_WalkerType.Player)
            {
                OnLose?.Invoke();
                return;
            }

            int enemiesCounter = _walkersHolder.Walkers.Where(w => w.Type == E_WalkerType.RandomEnemy || w.Type == E_WalkerType.TargetEnemy).ToList().Count;

            if (enemiesCounter <= 0)
                OnLose.Invoke();
        }
    }
}
