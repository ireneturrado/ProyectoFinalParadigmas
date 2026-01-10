using UnityEngine;
using TMPro;

public class DifficultySelector : MonoBehaviour
{
    public TMP_Text difficultyText;

    private int difficulty = 1; // 0 Fácil, 1 Normal, 2 Difícil

    void Start()
    {
        difficulty = PlayerPrefs.GetInt("Difficulty", 1);
        UpdateText();
    }

    public void NextDifficulty()
    {
        difficulty = (difficulty + 1) % 3;
        PlayerPrefs.SetInt("Difficulty", difficulty);
        UpdateText();
    }

    void UpdateText()
    {
        string name = difficulty switch
        {
            0 => "Fácil",
            1 => "Normal",
            2 => "Difícil",
            _ => "Normal"
        };

        difficultyText.text = "Dificultad: " + name;
    }
}
