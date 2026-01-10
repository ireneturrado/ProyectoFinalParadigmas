using UnityEngine;
using TMPro;

public class DifficultyDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    void Start()
    {
        int saved = PlayerPrefs.GetInt("Difficulty", 1);
        dropdown.value = saved;
    }

    public void OnDifficultyChanged(int value)
    {
        PlayerPrefs.SetInt("Difficulty", value);
    }
}
