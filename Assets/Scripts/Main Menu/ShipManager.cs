using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipManager : MonoBehaviour
{

	[Tooltip("Ensimmäinen strenght-palkki")]
	public GameObject FirstBar;


	public GameObject AttrOn, AttrOff;
	[Tooltip("The parent game object")]
	public Transform WireframeShips, Attributes;

	public int SelectedShip = 0;
	public Text ShipName;
	// ship attributes: strength, weight, acceleration, speed, turning, fire rate

	// TODO: Alusten atribuutit haetaan myohemmin alus-luokasta tjsp.
	private int[] _retro_atr = new int[] { 2, 2, 2, 2, 2, 2 };
	private int[] _ufo_atr = new int[] { 1, 1, 2, 2, 3, 3 };
	private int[] _untied_atr = new int[] { 1, 1, 3, 3, 3, 1 };
	private int[] _discovery_atr = new int[] { 3, 3, 1, 1, 1, 3 };
	private int[] _rossa_atr = new int[] { 2, 1, 3, 2, 2, 2 };
	private List<int[]> _attributeList = new List<int[]>();
	private List<GameObject> _deleteAttributes = new List<GameObject>();
	private List<GameObject> _wireframeShips, _attributeBars;

	void Start()
	{
		//TODO: refactor to fetch the attributes from the ship prefabs
		_attributeList.Add(_untied_atr);
		_attributeList.Add(_retro_atr);
		_attributeList.Add(_discovery_atr);
		_attributeList.Add(_ufo_atr);
		_attributeList.Add(_rossa_atr);

		_wireframeShips = new List<GameObject>();
		_attributeBars = new List<GameObject>();

		//Collects child objects of Ships to a list
		foreach (Transform child in WireframeShips)
		{
			_wireframeShips.Add(child.gameObject);
		}

		//Collects child objects  of Attributes to a list
		foreach (Transform child in Attributes)
		{
			_attributeBars.Add(child.gameObject);
		}
		ChangeShip(0);
	}

	void Update()
	{
		for (int i = 0; i < _wireframeShips.Count; i++)
		{
			_wireframeShips[i].transform.Rotate(0, 0, Time.deltaTime * -40);
		}
	}

	public void NextShip()
	{
		ChangeShip(+1);
	}

	public void PrevShip()
	{
		ChangeShip(-1);
	}

	private void ChangeShip(int direction)
	{
		DeleteAttributes();

		// Vaihtaa alusta suunnan perusteella aktivoimalla uuden objektin ja deaktivoimalla aikaisemman
		_wireframeShips[SelectedShip].SetActive(false);
		SelectedShip += direction;
		if (SelectedShip > _wireframeShips.Count - 1)
		{
			SelectedShip = 0;
		}
		else if (SelectedShip < 0)
		{
			SelectedShip = _wireframeShips.Count - 1;
		}
		_wireframeShips[SelectedShip].SetActive(true);

		// Vaihtaa aktiivisena olevan aluksen nimen otsikkoon
		ShipName.text = _wireframeShips[SelectedShip].name;
		UpdateAttributes();

	}

    public string GetSelectedShip()
    {
        return _wireframeShips[SelectedShip].name;
    }

    private void DeleteAttributes()
	{
		// Tuhoaa aikaisemman aluksen attribuutit uusien tielta
		for (int i = 0; i < _deleteAttributes.Count; i++)
		{
			Destroy(_deleteAttributes[i]);
		}
	}


	private void UpdateAttributes()
	{
		Vector3 _distance = new Vector3(0, 0, 0);

		// Maalataan alusten attribuutit sceneen
		for (int i = 0; i < _retro_atr.Length; i++)
		{
			int temp = _attributeList[SelectedShip][i] * 3;
			for (int j = 0; j < temp; j++)
			{
				GameObject attribute = Instantiate(AttrOn, _attributeBars[i].transform.position + _distance, Quaternion.identity, _attributeBars[i].transform);
				attribute.transform.localScale = FirstBar.transform.localScale;
				_deleteAttributes.Add(attribute);
				_distance += new Vector3(FirstBar.transform.position.x - _attributeBars[0].transform.position.x, 0, 0);

				if (j == temp - 1)
				{
					for (int k = 0; k < 9 - temp; k++)
					{
						attribute = Instantiate(AttrOff, _attributeBars[i].transform.position + _distance, Quaternion.identity, _attributeBars[i].transform);
						_deleteAttributes.Add(attribute);
						_distance += new Vector3(FirstBar.transform.position.x - _attributeBars[0].transform.position.x, 0, 0);
					}
					_distance = new Vector3(0, 0, 0);
				}
			}
		}
	}
}
