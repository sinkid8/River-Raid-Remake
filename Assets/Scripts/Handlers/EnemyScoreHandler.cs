using UnityEngine;

// Attach this to enemy objects
public class EnemyScoreHandler : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private bool isDestroyed = false;
    
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
        }
        else
        {
            Debug.LogWarning("ScoreManager not found, could not award score!");
        }
    }
}