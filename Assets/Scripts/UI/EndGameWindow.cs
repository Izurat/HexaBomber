using System;
using UnityEngine;
using UnityEngine.UI;

public class EndGameWindow : MonoBehaviour
{
    [SerializeField] private Button _okButton;
    [SerializeField] private GameObject _window;

    public event Action OnOkButton;

    public void Show() 
    {
        _window.SetActive(true);
    }

    public void Hide() 
    {
        _window.SetActive(false);
    }

    private void Start()
    {
        _okButton.onClick.AddListener(OnButton);
    }

    private void OnButton()
    {
        OnOkButton.Invoke();
    }
}
