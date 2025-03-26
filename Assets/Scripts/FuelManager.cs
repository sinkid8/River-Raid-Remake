using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FuelManager : MonoBehaviour
{
    public Image batteryImage;
    public Sprite[] batterySprites;

    // Event for when fuel level changes
    public UnityEvent<int> OnFuelLevelChanged = new UnityEvent<int>();
    // Event specifically for when fuel is full
    public UnityEvent OnFuelFull = new UnityEvent();

    private int currentFuelLevel = 0;
    private int maxFuelLevel = 4;

    private void Start()
    {
        // Initialize battery UI
        UpdateBatteryUI();
    }

    public void AddFuel()
    {
        if (currentFuelLevel < maxFuelLevel)
        {
            currentFuelLevel++;
            UpdateBatteryUI();

            // Check if fuel is now full
            if (currentFuelLevel >= maxFuelLevel)
            {
                OnFuelFull.Invoke();
            }

            // Notify listeners about fuel level change
            OnFuelLevelChanged.Invoke(currentFuelLevel);
        }
    }

    // Method to check if fuel is full
    public bool IsFuelFull()
    {
        return currentFuelLevel >= maxFuelLevel;
    }

    // Method to use half of the fuel (for energy weapon)
    public bool UseHalfFuel()
    {
        if (currentFuelLevel >= maxFuelLevel)
        {
            // Halve the fuel level (integer division)
            currentFuelLevel = maxFuelLevel / 2;
            UpdateBatteryUI();
            OnFuelLevelChanged.Invoke(currentFuelLevel);
            return true;
        }
        return false;
    }

    private void UpdateBatteryUI()
    {
        // Update the battery image based on fuel level
        batteryImage.sprite = batterySprites[currentFuelLevel];
    }

    // Added getter for current fuel level
    public int GetCurrentFuelLevel()
    {
        return currentFuelLevel;
    }
}