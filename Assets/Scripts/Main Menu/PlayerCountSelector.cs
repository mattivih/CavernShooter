using UnityEngine;
using UnityEngine.UI;

public class PlayerCountSelector : MonoBehaviour
{
	public static int PlayersSelected = 2;
	public Sprite Active, Unactive, ActiveHover, UnactiveHover;
	private Image _image;

	void Start()
	{
		_image = GetComponent<Image>();
	}

    public void OnEnable()
    {
        GetComponent<Collider2D>().enabled = true;
    }

    public void SetInteractable(bool interactable)
    {
        GetComponent<Collider2D>().enabled = interactable;
    }

    //Start hoovering
    void OnMouseEnter()
	{
		if (_image.sprite.name == "BTN-ship-UNACTIVE-01")
		{
			ChangeSprites(UnactiveHover);
		}
		else if (_image.sprite.name == "BTN-ship-ACTIVE-01")
		{
			ChangeSprites(ActiveHover);
		}
	}

	//End hoovering
	void OnMouseExit()
	{
		if (_image.sprite.name == "BTN-ship-UNACTIVE-HOVER-01")
		{
			ChangeSprites(Unactive);
		}
		else if (_image.sprite.name == "BTN-ship-ACTIVE-HOVER-01")
		{
			ChangeSprites(Active);
		}
	}

	//Select/unselecet
	void OnMouseDown()
	{
		//Select
		if (_image.sprite.name == "BTN-ship-UNACTIVE-HOVER-01" ||
		_image.sprite.name == "BTN-ship-UNACTIVE-01")
		{
            ChangeSprites(Active);
            if (gameObject.name == "Player 4")
            {
                PlayersSelected = 4;
                if (GameObject.Find("Player 3").GetComponent<Image>().sprite.name == "BTN-ship-UNACTIVE-01")
                    GameObject.Find("Player 3").GetComponent<Image>().sprite = Active;
                ButtonSoundPlayer.Instance.PlayNextSound();
            }               
            else           
                PlayersSelected++;                   
        }

		else if (_image.sprite.name == "BTN-ship-ACTIVE-HOVER-01" ||
	   _image.sprite.name == "BTN-ship-ACTIVE-01")
		{
            ChangeSprites(Unactive);
            ButtonSoundPlayer.Instance.PlayPrevSound();
            if (gameObject.name == "Player 3" && GameObject.Find("Player 4").GetComponent<Image>().sprite.name == "BTN-ship-ACTIVE-01")
            {
                GameObject.Find("Player 4").GetComponent<Image>().sprite = Unactive;
                PlayersSelected = 2;
            }
            else
                PlayersSelected--;  
        }
	}

	public void ChangeSprites(Sprite sprite)
	{
		_image.sprite = sprite;
	}

}
