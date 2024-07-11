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

    public void SetScore(float elapsedTime)
    {
        int minutes = (int)elapsedTime / 60;
        int seconds = (int)elapsedTime % 60;

        string minString = "";
        string secString = "";

        if (minutes < 10)
        {
            minString = "0" + minutes.ToString();
        }
        else
        {
            minString = minutes.ToString();
        }

        if (seconds < 10)
        {
            secString = "0" + seconds.ToString();
        }
        else
        {
            secString = seconds.ToString();
        }

        string scoreStr = minString + " : " + secString;
        scoreText.text = scoreStr;
    }
}

