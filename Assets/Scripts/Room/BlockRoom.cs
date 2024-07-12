using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BlockRoom : MonoBehaviour
{
    Box box;

    public static Action ProcessSavedPerson;

    [SerializeField]
    private GameObject RewardParticlePrefab; // 1 prefab per cell

    private ParticleSystem RewardParticle;

    private BoxCollider2D boxCollider2D;

    public void Init(Coin coin, Box box)
    {
        Color effectColor = GameManager.Instance.CoinToColor[coin];

        CreateRewardParticles(box, RewardParticlePrefab, effectColor);
    }

    private void CreateRewardParticles(Box box, GameObject RewardParticlePrefab, Color color)
    {
        GameObject RewardGo = GameObject.Instantiate(RewardParticlePrefab);

        RewardParticle = RewardGo.GetComponent<ParticleSystem>();

        RewardGo.transform.position = box.bounds.center;

        var sh = RewardParticle.shape;
        sh.scale = new Vector2(box.bounds.size.x, box.bounds.size.y);

        ParticleSystem.MainModule main = RewardParticle.main;
        main.startColor = color;
    }
}

