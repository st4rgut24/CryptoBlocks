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
        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if reached the target position
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Get a new random target position
            targetPosition = GetRandomPosition();
        }
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

