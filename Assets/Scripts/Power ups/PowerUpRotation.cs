using UnityEngine;

public class PowerUpRotation : MonoBehaviour {
    public Vector3 speed;

    // Update is called once per frame
    void Update() {
        transform.Rotate(speed* Time.deltaTime);
    }
}
