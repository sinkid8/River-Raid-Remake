using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FuelManager : MonoBehaviour
{
    public Image batteryImage;
    public Sprite[] batterySprites;

    public UnityEvent<int> OnFuelLevelChanged = new UnityEvent<int>();
    public UnityEvent OnFuelFull = new UnityEvent();

    private int currentFuelLevel = 0;
    private int maxFuelLevel = 4;
    
    // Default starting fuel level
    [SerializeField] private int defaultStartingFuel = 0;

    private void Start()
    {
        ResetFuel();
        UpdateBatteryUI();
    }

    public void AddFuel()
    {
        if (currentFuelLevel < maxFuelLevel)
        {
            currentFuelLevel++;
            UpdateBatteryUI();

            if (currentFuelLevel >= maxFuelLevel)
            {
                OnFuelFull.Invoke();
            }
            OnFuelLevelChanged.Invoke(currentFuelLevel);
        }
    }

    public bool IsFuelFull()
    {
        return currentFuelLevel >= maxFuelLevel;
    }

    public bool UseHalfFuel()
    {
        if (currentFuelLevel >= 2)
        {
            currentFuelLevel -= 2;
            UpdateBatteryUI();
            OnFuelLevelChanged.Invoke(currentFuelLevel);
            return true;
        }
        return false;
    }

    public bool UseExactFuel(int amount)
    {
        if (currentFuelLevel >= amount)
        {
            currentFuelLevel -= amount;
            UpdateBatteryUI();
            OnFuelLevelChanged.Invoke(currentFuelLevel);
            return true;
        }
        return false;
    }

    private void UpdateBatteryUI()
    {
        if (batteryImage != null && batterySprites != null && currentFuelLevel >= 0 && currentFuelLevel < batterySprites.Length)
        {
            batteryImage.sprite = batterySprites[currentFuelLevel];
        }
    }

    public int GetCurrentFuelLevel()
    {
        return currentFuelLevel;
    }
    
    public void ResetFuel()
    {
        // Reset fuel to default starting value
        currentFuelLevel = defaultStartingFuel;
        UpdateBatteryUI();
        OnFuelLevelChanged.Invoke(currentFuelLevel);
        
        // Check if we need to trigger the full fuel event
        if (currentFuelLevel >= maxFuelLevel)
        {
            OnFuelFull.Invoke();
        }
    }
}