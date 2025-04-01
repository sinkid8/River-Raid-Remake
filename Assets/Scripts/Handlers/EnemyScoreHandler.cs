using UnityEngine;

public class EnemyScoreHandler : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;
    private bool isDestroyed = false;
    
    public void SetScoreValue(int newValue)
    {
        scoreValue = newValue;
    }

    public void AwardScore()
    {
        if (isDestroyed)
            return;
            
        isDestroyed = true;
        
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
    
    public int GetScoreValue()
    {
        return scoreValue;
    }
}