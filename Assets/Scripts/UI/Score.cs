using UnityEngine;
using System.Collections;
using TMPro;
public class Score : MonoBehaviour
{
	TextMeshProUGUI scoreText;

    //private int alive;
    private int population;

    private void OnEnable()
    {
        //PersonPrefab.DieEvent += OnDie;
    }

    // Use this for initialization
    void Start()
	{
        scoreText = GetComponent<TextMeshProUGUI>();

        population = PersonPlotter.Instance.GetPopulation();
        SetScore(0);
	}

    private void OnDisable()
    {
        //PersonPrefab.DieEvent -= OnDie;
    }

    public void SetScore(int score)
    {
        string scoreStr = score.ToString() + "/" + population.ToString();
        scoreText.text = scoreStr;
    }
}

