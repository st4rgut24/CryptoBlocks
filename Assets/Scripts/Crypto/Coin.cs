using UnityEngine;
using System.Collections;

public enum Coin
{
    Bitcoin,
    Ethereum,
    Litecoin,
    Dodgecoin,
    Shiba
}

public class CoinFract
{
    public Coin coin;
    public float fract;

    public CoinFract(Coin coin, float fract)
    {
        this.coin = coin;
        this.fract = fract;
    }
}
