using UnityEngine;
using System.Collections;
using System;

public class ResearchCenterPrefab : MonoBehaviour
{
    Box box;

	public Sprite skullSprite;
	public bool IsHealthCenter;

	public static Action ProcessSavedPerson;

	[SerializeField]
	private GameObject ResearchParticlePrefab; // 1 prefab per cell

    [SerializeField]
    private GameObject SickParticlesPrefab; // 1 prefab per cell

    private ParticleSystem ResearchParticle;

	private BoxCollider2D boxCollider2D;

	Vector2 boxColliderOffset = new Vector2(-.5f, -.5f);

    void OnTriggerEnter2D(Collider2D collider)
	{
		GameObject colliderGo = collider.gameObject;
		PersonPrefab person = colliderGo.GetComponent<PersonPrefab>();

		if (person != null)
		{
			if (!IsHealthCenter)
			{
                if (person.healthState == HealthState.Healthy)
                {
                    person.SetHealthState(HealthState.Sick);
                }
                else
                {
                    person.Die();
                }
            }
			else
			{
                if (person.healthState == HealthState.Immune || person.healthState == HealthState.Healthy)
                {
                    ProcessSavedPerson?.Invoke();
                }
                else if (person.healthState == HealthState.Sick)
                {
					IsHealthCenter = false;

                    GameObject.Destroy(ResearchParticle.gameObject);
                    CreateResearchParticles(this.box, SickParticlesPrefab);
                }

                // remove the entered person but dont hurt score
                GameObject.Destroy(person.gameObject);
				PersonPlotter.Instance.PersonsList.Remove(person.gameObject);
            }
        }
    }

	private void CreateResearchParticles(Box box, GameObject ResearchParticlePrefab)
	{
        GameObject ResearchGo = GameObject.Instantiate(ResearchParticlePrefab);

        ResearchParticle = ResearchGo.GetComponent<ParticleSystem>();

        ResearchGo.transform.position = box.bounds.center;


        var sh = ResearchParticle.shape;
		sh.scale = new Vector2(box.bounds.size.x, box.bounds.size.y);
	}

    public void InitSpace(Box box)
    {
        this.box = box;

		// set the bounds of the box collider
		boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
		boxCollider2D.isTrigger = true;
		// do something else
		boxCollider2D.size = (Vector2)box.bounds.size + boxColliderOffset;
		boxCollider2D.offset = box.bounds.center;

		CreateResearchParticles(box, ResearchParticlePrefab);
    }

    // Use this for initialization
    void Start()
	{
		IsHealthCenter = true;
    }
}

