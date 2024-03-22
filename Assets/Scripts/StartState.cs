using UnityEngine;

public class StartState : IState
{
    private float notFightingTimer = 0f;
    private readonly StateMachine stateMachine;
    private readonly zedcameraHandler zedCameraHandler;

    public StartState(StateMachine stateMachine, zedcameraHandler zedCameraHandler)
    {
        this.stateMachine = stateMachine;
        this.zedCameraHandler = zedCameraHandler;
    }

    public void Enter()
    {
        Debug.Log("Entered StartState: Waiting for 10 seconds of non-fighting");
        notFightingTimer = 0f;
    }
        
    public void Execute()
    {
        if (zedCameraHandler.Action == "notFighting")
        {
            notFightingTimer += Time.deltaTime;
            if (notFightingTimer >= 10f)
            {
                if (zedCameraHandler.Action == "kick")
                {
                    stateMachine.ChangeState(new CountingKicksState(stateMachine, zedCameraHandler));
                }
                else if (zedCameraHandler.Action == "boxing")
                {
                    stateMachine.ChangeState(new CountingBoxingState(stateMachine, zedCameraHandler));
                }
                else
                {
                    // If no specific action is detected, transition to NoFightingState 
                    stateMachine.ChangeState(new NoFightingState(stateMachine, zedCameraHandler));
                }
            }
            else
            {
                float timeLeft = 10f - notFightingTimer;
                Debug.Log($"Time left in notFighting state: {timeLeft} seconds");
            }
        }
        else
        {
            notFightingTimer = 0f; // Reset timer if the action changes
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting StartState");
    }
}

/// <summary>
/// Represents the state where kicks are being counted.
/// </summary>
public class CountingKicksState : IState
{
    private readonly StateMachine stateMachine;
    private readonly zedcameraHandler zedCameraHandler;


    public CountingKicksState(StateMachine stateMachine, zedcameraHandler zedCameraHandler)
    {
        this.stateMachine = stateMachine;
        this.zedCameraHandler = zedCameraHandler;
    }


    public void Enter()
    {
        Debug.Log("Entered CountingKicksState: Starting to count kicks");
    }


    /// Called every frame while in the CountingKicksState.
    public void Execute()
    {
        if (zedCameraHandler.Action == "notFighting")
                {
                    stateMachine.ChangeState(new NoFightingState(stateMachine, zedCameraHandler));
                }
        else if (zedCameraHandler.Action == "boxing")
        {
            stateMachine.ChangeState(new CountingBoxingState(stateMachine, zedCameraHandler));
        }
    }


    /// Called when exiting the CountingKicksState.
    public void Exit()
    {
        stateMachine.KickCounter ++;
        Debug.Log($"Kick Count: {stateMachine.KickCounter}");
        Debug.Log("Exiting CountingKicksState");
    }
}


/// <summary>
/// Represents the state where the system is counting boxing actions.
/// </summary>
public class CountingBoxingState : IState
{
    
    private readonly StateMachine stateMachine;
    private readonly zedcameraHandler zedCameraHandler;

    public CountingBoxingState(StateMachine stateMachine, zedcameraHandler zedCameraHandler)
    {
        this.stateMachine = stateMachine;
        this.zedCameraHandler = zedCameraHandler;
    }

    public void Enter()
    {
        Debug.Log("Entered CountingKicksState: Starting to count kicks");
    }

    public void Execute()
    {
        if (zedCameraHandler.Action == "notFighting")
        {
            stateMachine.ChangeState(new NoFightingState(stateMachine, zedCameraHandler));
        }
        else if (zedCameraHandler.Action == "kick")
        {
            stateMachine.ChangeState(new CountingKicksState(stateMachine, zedCameraHandler));
        }
    }

    public void Exit()
    {
        stateMachine.BoxingCounter++;
        Debug.Log($"Box Count: {stateMachine.BoxingCounter}");
        Debug.Log("Exiting CountingBoxingState");
    }
}



/// <summary>
/// Represents the state where no fighting action is detected.
/// </summary>
public class NoFightingState : IState
{
    private readonly StateMachine stateMachine;
    private readonly zedcameraHandler zedCameraHandler;

    public NoFightingState(StateMachine stateMachine, zedcameraHandler zedCameraHandler)
    {
        this.stateMachine = stateMachine;
        this.zedCameraHandler = zedCameraHandler;
    }

    public void Enter()
    {
        Debug.Log("Entered NoFightingState: Monitoring for activities.");
    }

    public void Execute()
    {
        if (zedCameraHandler.Action == "kick")
        {
            stateMachine.ChangeState(new CountingKicksState(stateMachine, zedCameraHandler));
        }
        else if (zedCameraHandler.Action == "boxing")
        {
            stateMachine.ChangeState(new CountingBoxingState(stateMachine, zedCameraHandler));
        }
        else
        {
            
        }
    }
    public void Exit()
    {
        Debug.Log("Exiting NoFightingState");
    }
}
