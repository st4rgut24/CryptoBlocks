using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    //ImmuneCounter immuneCounter;
    Score score;

    //public const int ImmuneGoal = 5; // number of immune people needed to find a cure
    public int savePersonCount = 0;

    private void OnEnable()
    {
        ResearchCenterPrefab.ProcessSavedPerson += OnProcessSavedPerson;
    }

    // Use this for initialization
    void Start()
	{
        score = GameObject.Find(Consts.ScoreGo).GetComponent<Score>();
        //immuneCounter = GameObject.Find(Consts.ImmuneCountGo).GetComponent<ImmuneCounter>();
        //immuneCounter.SetImmuneCounter(immunizeCount, ImmuneGoal);
    }

    private void OnProcessSavedPerson()
    {
        savePersonCount++;
        score.SetScore(savePersonCount);
    }

    // Update is called once per frame
    void Update()
	{
			
	}

    private void OnDisable()
    {
        ResearchCenterPrefab.ProcessSavedPerson -= OnProcessSavedPerson;
    }
}

