using EndGame;
using Signals;
using UnityEngine;
using Zenject;

public class WindowsLogic : MonoBehaviour
{
    [SerializeField] private EndGameWindow _windowWin;
    [SerializeField] private EndGameWindow _windowLose;

    [Inject] private IEndGameHandler _endGameHandler;
    [Inject] private SignalBus _signalBus;

    private void Start() 
    {
        _windowLose.Hide();
        _windowWin.Hide();

        _windowWin.OnOkButton += OnOkButton;
        _windowLose.OnOkButton += OnOkButton;

        _endGameHandler.OnLose += ShowLose;
        _endGameHandler.OnWin += ShowWin;
    }

    private void ShowLose() 
    {
        _windowLose.Show();
    }

    private void ShowWin() 
    {
        _windowWin.Show();
    }

    private void OnOkButton()
    {
        _signalBus.Fire<GameFinishedSignal>(new GameFinishedSignal());
    }
}
