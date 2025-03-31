using UnityEngine;

// Attach this to enemy objects
public class EnemyScoreHandler : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;
    private bool isDestroyed = false;
    
    // Method to set the score value from another script
    public void SetScoreValue(int newValue)
    {
        scoreValue = newValue;
    }
    
    // This can be called when an enemy is destroyed
    public void AwardScore()
    {
        // Prevent multiple score awards
        if (isDestroyed)
            return;
            
        isDestroyed = true;
        
        // Award score points
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
            Debug.Log($"Awarded {scoreValue} points for destroying {gameObject.name}");
        }
        else
        {
            Debug.LogWarning("ScoreManager not found, could not award score!");
        }
    }
    
    // Getter for score value if needed
    public int GetScoreValue()
    {
        return scoreValue;
    }
}