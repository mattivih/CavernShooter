using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour {

    public GameObject Ship;
    public SpriteRenderer LevelSprite;
    public float Margin;
    public float YOffset {get;set;}

    private float _xMax, _xMin, _yMax, _yMin;
    private float _xRange, _yRange;
    private string _levelName;



    /// <summary>
    /// calculate map edges using level's outer edge and applies margin to those limits
    /// </summary>

    void Start() {
        float _xExt = LevelSprite.bounds.extents.x;
        float _yExt = LevelSprite.bounds.extents.y;
        Vector2 _center = LevelSprite.bounds.center;
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize;
        Vector2 cameraSize = new Vector2(cameraHeight * screenAspect, cameraHeight);

        _xMax = _center.x + _xExt - cameraSize.x;
        _xMin = _center.x - _xExt + cameraSize.x;
        _yMax = _center.y + _yExt - cameraSize.y;
        _yMin = _center.y - _yExt + cameraSize.y;

        _levelName = SceneManager.GetActiveScene().name;

    }

    /// <summary>
    /// Clamps camera movement between xRange and yRange.
    /// </summary>
    void FixedUpdate() {
        if (Ship != null) {
            _xRange = Mathf.Clamp(Ship.transform.position.x, _xMin, _xMax);
            if (_levelName != "3_Limbo")
            {
                _yRange = Mathf.Clamp(Ship.transform.position.y + YOffset, _yMin, _yMax);
            }
            else {
                _yRange = Ship.transform.position.y + YOffset;
            }
            transform.position = new Vector3(_xRange, _yRange, transform.position.z);
        } else {
            transform.position = new Vector3(0, 0, transform.position.z);
        }
    }
}
