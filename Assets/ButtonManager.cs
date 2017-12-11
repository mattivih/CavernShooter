using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

    public Button Back, CenterButton;

    public Button GetBackButton() {
        return Back;
    }

    public Button GetCenterButton() {
        return CenterButton;
    }
}
