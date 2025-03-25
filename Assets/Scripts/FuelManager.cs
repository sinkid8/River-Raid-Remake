using UnityEngine;
using UnityEngine.UI;

public class FuelManager : MonoBehaviour
{
    public Image batteryImage;  
    public Sprite[] batterySprites;  

    private int currentFuelLevel = 0;  
    private int maxFuelLevel = 4;  

    
    public void AddFuel()
    {
        if (currentFuelLevel < maxFuelLevel)
        {
            currentFuelLevel++;
            UpdateBatteryUI();
        }
    }

    private void UpdateBatteryUI()
    {
        // Update the battery image based on fuel level
        batteryImage.sprite = batterySprites[currentFuelLevel];
    }
}
