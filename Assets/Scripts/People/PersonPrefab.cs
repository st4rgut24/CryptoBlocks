using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HealthState
{
    Sick,
    Healing,
    Healthy,
    Immune
}

public class PersonPrefab : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    public Sprite SickSprite;
    public Sprite HealthySprite;
    public Sprite ImmuneSprite;

    public GameObject DeathParticles;

    public static System.Action<GameObject, HealthState> DieEvent;

    public Color HealthyColor = Color.white;
    public Color InfectedColor = Color.red;

    public float moveSpeed = 2f;
    public float maxDistance = 2f;

    private Vector2 targetPosition;
    private Vector2 randomDirection;

    private bool isImmunizing = false;

    public HealthState healthState;

    float timeSick = 0;
    float timeImmune = 0;

    float sickSec;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start()
	{
        targetPosition = GetRandomPosition();
        sickSec = Random.Range(Consts.minSickSec, Consts.maxSickSec);
    }

	// Update is called once per frame
	void Update()
	{
        if (isImmunizing) // if immunizing another cell, attach to that cell and do not move self
            return;

        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if reached the target position
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Get a new random target position
            targetPosition = GetRandomPosition();
        }

        if (healthState == HealthState.Sick)
        {
            timeSick += Time.deltaTime;
            float t = timeSick / sickSec;
            spriteRenderer.color = Color.Lerp(HealthyColor, InfectedColor, t);

            if (timeSick >= sickSec)
                processSickOutcome();
        }
    }

    public void Die()
    {
        GameObject.Destroy(gameObject);
        DieEvent?.Invoke(gameObject, healthState);
    }

    private void processSickOutcome()
    {
        float chance = Random.Range(0f, 1f);

        if (chance < Consts.fatalityProb)
        {
            GameObject Splatter = GameObject.Instantiate(DeathParticles);
            Splatter.transform.position = transform.position;

            Die();
        }
        else
        {
            SetHealthState(HealthState.Immune);
            moveSpeed *= 2;
            maxDistance *= 2;
            // move faster and farther to aid in spreading immunity
        }
    }

    public void SetHealthState(HealthState healthState)
    {
        this.healthState = healthState;
        StyleByHealthState(healthState);
    }

    private void StyleByHealthState(HealthState healthState)
    {
        Color color;
        spriteRenderer.color = Color.white; // set default color of sprite

        switch (healthState)
        {
            case HealthState.Sick:
                spriteRenderer.sprite = SickSprite; 
                break;
            case HealthState.Healthy:
                spriteRenderer.sprite = HealthySprite;
                break;
            case HealthState.Immune:
                spriteRenderer.sprite = ImmuneSprite;
                break;
            default:
                color = Color.white;
                break;
        }
    }

    private bool IsImmuneEncounter(HealthState otherHealthState)
    {
        bool IsImmuneScenario = healthState == HealthState.Immune && otherHealthState == HealthState.Sick && !isImmunizing;

        bool ImmunityChance = Random.Range(0f, 1f) < Consts.immunityProb;

        return IsImmuneScenario && ImmunityChance;
    }


    private IEnumerator processImmuneEncounter(PersonPrefab ImmunePerson, PersonPrefab SickPerson)
    {
        Transform originalImmuneParent = ImmunePerson.transform.parent;

        isImmunizing = true;
        transform.SetParent(SickPerson.transform);

        SickPerson.healthState = HealthState.Healing; // dont die while healing

        yield return new WaitForSeconds(Consts.immunityCooldownSec);
        // add a cooldown period

        isImmunizing = false;
        transform.SetParent(originalImmuneParent);

        SickPerson.SetHealthState(HealthState.Immune);
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    Vector3 GetRandomPosition()
    {
        // Generate a random position within a maxDistance radius
        randomDirection = Random.insideUnitSphere * maxDistance;
        return randomDirection + (Vector2) transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Consts.PersonTag))
        {
            PersonPrefab otherPerson = collision.gameObject.GetComponent<PersonPrefab>();

            if (IsImmuneEncounter(otherPerson.healthState))
            {
                StartCoroutine(processImmuneEncounter(this, otherPerson));
            }
        }
        if (collision.gameObject.CompareTag(Consts.RoomTag))
        {
            Vector3 closestPoint = collision.ClosestPoint(transform.position);
            Vector2 oppositeDirection = transform.position - closestPoint; ;

            // Set the target position based on the opposite direction and move distance
            targetPosition = transform.position + new Vector3(oppositeDirection.x, oppositeDirection.y, 0f) * 5;
            Debug.DrawRay(closestPoint, oppositeDirection, Color.magenta, 1000);
        }
    }
}

