using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PersonPlotter : Singleton<PersonPlotter>
{
	[SerializeField]
	private int population;

	[SerializeField]
	private int sickCount;

	[SerializeField]
	private GameObject PersonPrefabInst;

	[SerializeField]
	private Transform PersonParentTransform;

	public List<GameObject> PersonsList;

	private float buffer = 150; // pixels
	private int sickCounter = 0;

	public int GetPopulation()
	{
		return population;
	}

    private void OnEnable()
    {
		PersonPrefab.DieEvent += OnDie;
        Controller.DragEvent += OnDrag;
    }

    public void OnDrag(Vector3 dragStart, Vector3 dragEnd)
	{
		Vector2 dragStartWorld = Camera.main.ScreenToWorldPoint(dragStart);
        Vector2 dragEndWorld = Camera.main.ScreenToWorldPoint(dragEnd);

        List<GameObject> Swimmers = GetSplashRadiusPeople(Consts.CurrentRadius, dragStartWorld, false);

        Vector3 dir = (dragEndWorld - dragStartWorld).normalized;

		Swimmers.ForEach((Swimmer) =>
		{
			PersonPrefab swimmer = Swimmer.GetComponent<PersonPrefab>();
			Vector2 finalSwimPos = swimmer.transform.position + dir * Consts.CurrentDist;

			swimmer.SetTargetPosition(finalSwimPos);
		});
	}

	public void ImmunizeEveryone()
	{
		PersonsList.ForEach((Person) =>
		{
			PersonPrefab pp = Person.GetComponent<PersonPrefab>();
			pp.SetHealthState(HealthState.Immune);
		});
	}

	public bool IsPeopleInBounds(Bounds bounds)
	{
		for (int i = 0; i < PersonsList.Count; i++)
		{
			if (bounds.Contains(PersonsList[i].transform.position))
			{
				return true;
			}
		}
		return false;
	}

    public void OnDie(GameObject deadPerson, HealthState healthState)
	{
		if (healthState == HealthState.Sick)
		{
            List<GameObject> Victims = GetSplashRadiusPeople(Consts.BurstRadius, deadPerson.transform.position, true);
            // TODO: destroy walls within burst radius
            SpreadSickness(Victims);
        }

        PersonsList.Remove(deadPerson);
	}

    // Use this for initialization
    void Start()
	{
        PersonsList = new List<GameObject>();

        Vector2 bufferVector = Vector2.one * buffer;
		Vector2 minWorldPoint = Camera.main.ScreenToWorldPoint(Vector2.zero + bufferVector);
		Vector2 maxWorldPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height) - bufferVector);

		InitPopulation(minWorldPoint, maxWorldPoint);
	}

    private void SpreadSickness(List<GameObject> victims)
    {
        // sickness happens on explosion
        victims.ForEach((victim) =>
        {
            PersonPrefab victimPrefab = victim.GetComponent<PersonPrefab>();

            if (victimPrefab.healthState == HealthState.Healthy)
            {
                victimPrefab.SetHealthState(HealthState.Sick);
            }
        });
    }

    HealthState GetHealthState()
	{
		if (sickCounter < sickCount)
		{
			sickCounter++;
			return HealthState.Sick;
		}
		else
		{
            return HealthState.Healthy;
        }
    }

	/// <summary>
	/// Criteria for getting sick:
	/// Persons must be in splash radius AND an adjacent CONNECTED room
	/// </summary>
	/// <param name="radius"></param>
	/// <param name="center"></param>
	/// <returns></returns>
	public List<GameObject> GetSplashRadiusPeople(float radius, Vector3 center, bool IncludeAdjRoom)
	{
		RoomPrefab room = Map.Instance.FindRoomWorldCoords(center);

		List<GameObject> ProximalPersons = new List<GameObject>();

		PersonsList.ForEach((Person) =>
		{

			if (Vector2.Distance(Person.transform.position, center) < radius)
			{
				PersonPrefab otherPerson = Person.GetComponent<PersonPrefab>();
				RoomPrefab otherRoom = Map.Instance.FindRoomWorldCoords(otherPerson.transform.position);

				if (room == otherRoom)
					ProximalPersons.Add(Person);
				else if (IncludeAdjRoom && room.IsAdjacentConnectedRoom(otherRoom))
                    ProximalPersons.Add(Person);
            }
        });

		return ProximalPersons;
	}

	void InitPopulation(Vector2 minWorldPoint, Vector2 maxWorldPoint)
	{
		for (int i=0;i<population;i++)
		{
			float randWorldX = Random.Range(minWorldPoint.x, maxWorldPoint.x);
			float randWorldY = Random.Range(minWorldPoint.y, maxWorldPoint.y);

			GameObject PersonGo = Instantiate(PersonPrefabInst, PersonParentTransform);
			PersonGo.transform.position = new Vector2(randWorldX, randWorldY);

			HealthState healthState = GetHealthState();
			PersonGo.GetComponent<PersonPrefab>().SetHealthState(healthState);

			PersonsList.Add(PersonGo);
		}
	}

    private void OnDisable()
    {
        PersonPrefab.DieEvent -= OnDie;
        Controller.DragEvent -= OnDrag;
    }
}

