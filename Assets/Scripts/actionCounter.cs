using UnityEngine;

public class ActionCounter : MonoBehaviour
{




    public zedcameraHandler zedCameraHandler;
    private int kickCounter = 0;
    private int notFightingCounter = 0;
    private int boxingCounter = 0;

    private float notFightingTimer = 0f; 
    private bool canProcessActions = false; 

    void Update()
    {
        if (zedCameraHandler == null)
        {
            return; 
        }
        if (zedCameraHandler.Action == "notFighting")
        {
            
            notFightingTimer += Time.deltaTime; // Increment timer 
            if (notFightingTimer >= 10f) // Check if "not fighting" has lasted for 10 seconds
            {
                canProcessActions = true;
            }

        }
        else
        {
            notFightingTimer = 0f; // Reset timer 
        }



        if (canProcessActions == true)
        {
            switch (zedCameraHandler.Action)
            {
                case "kick":
                    kickCounter++;
                    if (kickCounter % 10 == 0)
                    {
                        Debug.Log($"Kick occurred 10 times.");
                    }
                    break;

                case "notFighting":
                    notFightingCounter++;
                    if (notFightingCounter % 10 == 0)
                    {
                        Debug.Log($"Not fighting occurred 10 times.");
                    }
                    break;

                case "boxing":
                    boxingCounter++;
                    if (boxingCounter % 10 == 0)
                    {
                        Debug.Log($"Boxing occurred 10 times.");
                    }
                    break;
            }
        }
    }
}
