using Signals;
using UnityEngine.SceneManagement;
using Zenject;

namespace Field.Model
{
    public class GameRestarter
    {
        public GameRestarter(SignalBus signalBus)
        {
            signalBus.Subscribe<GameFinishedSignal>(OnGameFinished);
        }

        private void OnGameFinished()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
