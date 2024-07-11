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

	public int GetPopulation()
	{
		return population;
	}

    private void OnEnable()
    {
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

    // Use this for initialization
    void Start()
	{
        PersonsList = new List<GameObject>();

        Vector2 bufferVector = Vector2.one * buffer;
		Vector2 minWorldPoint = Camera.main.ScreenToWorldPoint(Vector2.zero + bufferVector);
		Vector2 maxWorldPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height) - bufferVector);

		InitPopulation(minWorldPoint, maxWorldPoint);
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
		GameManager.Instance.coins.ForEach((coin =>
		{
            float randWorldX = Random.Range(minWorldPoint.x, maxWorldPoint.x);
            float randWorldY = Random.Range(minWorldPoint.y, maxWorldPoint.y);

            GameObject PersonGo = Instantiate(PersonPrefabInst, PersonParentTransform);
            PersonPrefab person = PersonGo.GetComponent<PersonPrefab>();

            person.InitCoin(coin);

            PersonGo.transform.position = new Vector2(randWorldX, randWorldY);

            PersonsList.Add(PersonGo);
        }));
	}

    private void OnDisable()
    {
        Controller.DragEvent -= OnDrag;
    }
}

