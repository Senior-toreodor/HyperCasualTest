using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static int score = 0;
    public TextMeshProUGUI scoreText;

    void Update()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public static void IncreaseScore(int amount)
    {
        score += amount;
    }

    public static void DecreaseScore(int amount)
    {
        score -= amount;
    }
}
