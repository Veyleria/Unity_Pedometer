using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Pedometer : MonoBehaviour
{
    [Header("UI")]
    public Text stepsText;
    public Text walkingText;
    public Text TimeElapsedWalkingText;
    public Text TimeElapsedStandingStillText;

    [Header("Pedometer")]
    public float lowLimit = 0.005F; // Level to fall to the low state. 
    public float highLimit = 0.1F; // Level to go to high state (and detect steps).
    private bool stateHigh = false; // Comparator state.

    public float filterHigh = 10.0F; // Noise filter control. Reduces frequencies above filterHigh private . 
    public float filterLow = 0.1F; // Average gravity filter control. Time constant about 1/filterLow.
    public float currentAcceleration = 0F; // Noise filter.
    float averageAcceleration = 0F;

    public int steps = 0; // Step counter. Counts when comparator state goes to high.
    private int oldSteps;
    public float waitCounter = 0F;
    public float timeElapsedWalking = 0F;
    public float timeElapsedStandingStill = 0F;
    public bool isWalking = false;
    private bool startWaitCounter = false;

    void Awake()
    {
        averageAcceleration = Input.acceleration.magnitude; // Initialize average filter.
        oldSteps = steps;
    }

    void Update()
    {
        UpdateElapsedWalkingTime(); // Updates the time you spend while walking.
        WalkingCheck(); // Checks if you are walking or not.

        // Updates the isWalking Text.
        if (isWalking)
        {
            walkingText.text = ("Good job! You're walking! (づ｡◕‿‿◕｡)づ");
        }

        else if (!isWalking)
        {
            walkingText.text = ("Y U NO WALK?! (づ｡ᗒ ᗩᗕ｡)づ");
        }
    }

    void FixedUpdate()
    { 
        // Filter Input.acceleration using Math.Lerp.
        currentAcceleration = Mathf.Lerp(currentAcceleration, Input.acceleration.magnitude, Time.deltaTime * filterHigh);
        averageAcceleration = Mathf.Lerp(averageAcceleration, Input.acceleration.magnitude, Time.deltaTime * filterLow);

        float delta = currentAcceleration - averageAcceleration; // Gets the acceleration pulses.

        if (!stateHigh)
        { 
            // If the state is low.
            if (delta > highLimit)
            { 
                // Only goes to high, if the Input is higher than the highLimit.
                stateHigh = true;
                steps++; // Counts the steps when the comparator goes to high.
                stepsText.text = "Steps: " + steps;
            }
        }
        else
        {
            if (delta < lowLimit)
            { 
                // Only goes to low, if the Input is lower than the lowLimit.
                stateHigh = false;
            }
        }
    }

    // Checks if you are walking or not.
    private void WalkingCheck()
    {
        if (steps != oldSteps)
        {
            startWaitCounter = true;
            waitCounter = 0F;
        }

        if (startWaitCounter)
        {
            waitCounter += Time.deltaTime;

            if (waitCounter != 0)
            {
                isWalking = true;
            }
            if (waitCounter > 2.5)
            {
                waitCounter = 0F;
                startWaitCounter = false;
            }
        }
        else if (!startWaitCounter)
        {
            isWalking = false;
        }
        oldSteps = steps;
    }

    // Updates the time you spend while walking.
    private void UpdateElapsedWalkingTime()
    {
        int secondsWalk = (int)(timeElapsedWalking % 60);
        int minutesWalk = (int)(timeElapsedWalking / 60) % 60;
        int hourWalk = (int)(timeElapsedWalking / 3600) % 24;

        int secondsStill = (int)(timeElapsedStandingStill % 60);
        int minutesStill = (int)(timeElapsedStandingStill / 60) % 60;
        int hoursStill = (int)(timeElapsedStandingStill / 3600) % 24;

        string timeElapsedWalkingString = string.Format("{0:0}:{1:00}:{2:00}", hourWalk, minutesWalk, secondsWalk);
        string timeElapsedStandingStillString = string.Format("{0:0}:{1:00}:{2:00}", hoursStill, minutesStill, secondsStill);

        TimeElapsedWalkingText.text = "Time spend walking: " + timeElapsedWalkingString;
        TimeElapsedStandingStillText.text = "Time spend standing still: " + timeElapsedStandingStillString;

        if (isWalking)
        {
            timeElapsedWalking += Time.deltaTime;
        }
        else if (!isWalking)
        {
            timeElapsedStandingStill += Time.deltaTime;
        }
    }
}