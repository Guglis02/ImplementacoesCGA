using TMPro;
using UnityEngine;

/// <summary>
/// Classe responsável pelo painel de marcações de tempo.
/// </summary>
public class TimeTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bruteForceTimeLabel;
    [SerializeField] private TextMeshProUGUI spacialHashingTimeLabel;

    public float BruteForceTime => bruteForceTime;
    public float SpacialHashingTime => spacialHashingTime;

    private float bruteForceTime = 0;
    private float spacialHashingTime = 0;

    private void Start()
    {
        bruteForceTimeLabel.text = "Brute Force: 0s";
        spacialHashingTimeLabel.text = "Spacial Hashing: 0s";
    }

    private void UpdateColors()
    {
        if (bruteForceTime == spacialHashingTime
            || bruteForceTime == 0
            || spacialHashingTime == 0)
        {
            bruteForceTimeLabel.color = Color.white;
            spacialHashingTimeLabel.color = Color.white;
        }
        else if (bruteForceTime > spacialHashingTime)
        {
            bruteForceTimeLabel.color = Color.red;
            spacialHashingTimeLabel.color = Color.green;
        }
        else
        {
            bruteForceTimeLabel.color = Color.green;
            spacialHashingTimeLabel.color = Color.red;
        }
    }

    public void SetBruteForceTime(float time)
    {
        bruteForceTime = time;
        bruteForceTimeLabel.text = "Brute Force: " + time + "s";
        UpdateColors();
    }

    public void SetSpacialHashingTime(float time)
    {
        spacialHashingTime = time;
        spacialHashingTimeLabel.text = "Spacial Hashing: " + time + "s";
        UpdateColors();
    }

    public void AddSpatialHashTime(float time)
    {
        spacialHashingTime += time;
        spacialHashingTimeLabel.text = "Spacial Hashing: " + spacialHashingTime + "s";
        UpdateColors();
    }

    public void Reset()
    {
        bruteForceTime = 0;
        spacialHashingTime = 0;
        bruteForceTimeLabel.text = "Brute Force: 0s";
        spacialHashingTimeLabel.text = "Spacial Hashing: 0s";
        UpdateColors();
    }
}
