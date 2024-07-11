using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    Score score;

    private void OnEnable()
    {

    }

    // Use this for initialization
    void Start()
	{
        score = GameObject.Find(Consts.ScoreGo).GetComponent<Score>();
    }

    // Update is called once per frame
    void Update()
	{
			
	}

    private void OnDisable()
    {
    }
}

