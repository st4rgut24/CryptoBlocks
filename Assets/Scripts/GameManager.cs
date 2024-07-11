using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : Singleton<GameManager>
{
    Score score;
    public float timer = 0;

    public int totalCoins = 50;

    public List<CoinFract> fracts = new List<CoinFract>()
    {
        new CoinFract(Coin.Bitcoin, .3f),
        new CoinFract(Coin.Ethereum, .2f),
        new CoinFract(Coin.Dodgecoin, .2f),
        new CoinFract(Coin.Litecoin, .2f),
        new CoinFract(Coin.Shiba, .1f),
    };

    public List<Coin> coins = new List<Coin>();

    public Coin GetRandomCoin()
    {
        Coin coin = Coin.Bitcoin;
        float cum = 0;

        float rand = UnityEngine.Random.Range(0f, 1f);
        Debug.Log("Rand value " + rand);
        for (int i=0;i<fracts.Count;i++)
        {
            CoinFract fract = fracts[i];

            cum += fract.fract;

            if (rand <= cum)
            {
                return fract.coin;
            }
        };

        return coin;
    }

    private void OnEnable()
    {

    }

    private void Awake()
    {
        for (int i = 0; i < totalCoins; i++)
        {
            coins.Add(GetRandomCoin());
        }
    }

    // Use this for initialization
    void Start()
	{
        score = GameObject.Find(Consts.ScoreGo).GetComponent<Score>();
    }

    // Update is called once per frame
    void Update()
	{
        timer += Time.deltaTime;
        score.SetScore(timer);
	}

    private void OnDisable()
    {
    }
}

