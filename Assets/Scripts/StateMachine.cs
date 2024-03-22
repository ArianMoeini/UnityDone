using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StateMachine : MonoBehaviour
{
    public zedcameraHandler ZedCameraHandlerInstance;
    public int KickCounter { get; set; } = 0;
    public int BoxingCounter { get; set; } = 0;

    public TextMeshProUGUI kickCounterText; // Reference to the UI element for kick counter
    public TextMeshProUGUI boxingCounterText; // Reference to the UI element for boxing counter

    public void Start()
    {
        ChangeState(new StartState(this, ZedCameraHandlerInstance));
    }

    private IState currentState;

    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Execute();
      
        if (kickCounterText != null) kickCounterText.text = $"Kick Count: {KickCounter}";
        if (boxingCounterText != null) boxingCounterText.text = $"Box Count: {BoxingCounter}";
    }
}
