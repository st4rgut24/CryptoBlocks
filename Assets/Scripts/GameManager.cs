using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : Singleton<GameManager>
{
    Score score;
    public float timer = 0;

    public int totalCoins = 50;

    public Dictionary<Coin, int> CoinTracker = new Dictionary<Coin, int>();

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

    public int GetCoinCount(Coin coin)
    {
        return CoinTracker[coin];
    }

    private void Awake()
    {
        for (int i = 0; i < totalCoins; i++)
        {
            Coin coin = GetRandomCoin();

            if (!CoinTracker.ContainsKey(coin))
            {
                CoinTracker.Add(coin, 0);
            }

            CoinTracker[coin]++;

            coins.Add(coin);
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

