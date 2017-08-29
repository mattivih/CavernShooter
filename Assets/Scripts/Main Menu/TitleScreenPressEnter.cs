using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenPressEnter : MonoBehaviour
{
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
            SceneManager.LoadScene(1);
		}
	}
}
