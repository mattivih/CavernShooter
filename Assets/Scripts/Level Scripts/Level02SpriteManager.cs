using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level02SpriteManager : MonoBehaviour {
    public Transform LevelLeft, LevelRight;

    public List<Transform> Middle;
    public List<Transform> Up;
    public List<Transform> Down;

    float levelHeight;

    public float[] scales;

    Vector3 oldPos;
    bool playerFound;

    /// <summary>
    /// Single parallax layer
    /// </summary>
    public class ParallaxLayer {
        public int layer;
        public float scale;
        public List<Transform> sprites;
        public int middleIndex;

        public ParallaxLayer(List<Transform> objects, int layer, float scale, int middleIndex) {
            sprites = objects;
            this.layer = layer;
            this.scale = scale;
            this.middleIndex = middleIndex;
        }
        /// <summary>
        /// assign new mid index when looping sprites
        /// </summary>
        /// <param name="newIndex"></param>
        public void newMid(int newIndex) {
            middleIndex = newIndex;
            //Debug.Log("new middle for layer " + layer + ": " + sprites[middleIndex].name);
        }

        /// <summary>
        /// check if new index is wrapping around available list indices
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int indexWrap(int i) {
            if (i >= sprites.Count) {
                i = 0;
            } else if (i < 0) {
                i = sprites.Count - 1;
            }
            return i;
        }
    }

    List<ParallaxLayer> paraLayers;

    /// <summary>
    /// assign sprites to their parallax layers
    /// </summary>
    void Awake() {
        paraLayers = new List<ParallaxLayer>();
        for (int i = 0; i < Middle.Count; i++) {
            List<Transform> sprites = new List<Transform>();
            sprites.Add(Up[i]);
            sprites.Add(Middle[i]);
            sprites.Add(Down[i]);
            ParallaxLayer newLayer = new ParallaxLayer(sprites, i, scales[i], 1);
            paraLayers.Add(newLayer);
        }
    }

    /// <summary>
    /// align sprites with each other perfectly and instantiate fake walls at top and bottom of level
    /// </summary>
    void Start() {

        SpriteRenderer sr = Middle[0].GetComponent<SpriteRenderer>();

        levelHeight = sr.bounds.size.y;

        foreach (Transform t in Middle) {
            t.position = new Vector3(0, 0, t.position.z);
            t.localScale = Middle[0].localScale;
        }
        foreach (Transform t in Up) {
            t.position = new Vector3(0, levelHeight, t.position.z);
            t.localScale = Middle[0].localScale;
        }
        foreach (Transform t in Down) {
            t.position = new Vector3(0, -levelHeight, t.position.z);
            t.localScale = Middle[0].localScale;
        }

        for(int i = 0; i < 2; i++) {
            float yPos = 0;
            yPos = i == 0 ? yPos + levelHeight : yPos - levelHeight;
            GameObject left = Instantiate(LevelLeft.gameObject, new Vector3(LevelLeft.position.x, LevelLeft.position.y+yPos, LevelLeft.position.z), LevelLeft.rotation);
            Destroy(left.GetComponent<PolygonCollider2D>());
            GameObject right = Instantiate(LevelRight.gameObject, new Vector3(LevelRight.position.x, LevelRight.position.y + yPos, LevelRight.position.z), LevelLeft.rotation);
            Destroy(right.GetComponent<PolygonCollider2D>());
        }

    }

    /// <summary>
    /// Handle parallax and parallax layer looping
    /// </summary>

    void Update() {
        if (GameManager.Instance.Player) {
            if (!playerFound) {
                oldPos = Camera.main.transform.position;
                playerFound = true;
            } else {
                Vector3 diff = oldPos - Camera.main.transform.position;
                foreach (ParallaxLayer l in paraLayers) {
                    foreach (Transform t in l.sprites) {
                        t.position += diff * l.scale;
                    }
                }
                oldPos = Camera.main.transform.position;
            }

            foreach (ParallaxLayer l in paraLayers) {
                float middlePos = l.sprites[l.middleIndex].position.y;
                if (GameManager.Instance.Player.transform.position.y > middlePos + levelHeight / 2) {
                    int ind = l.indexWrap(l.middleIndex + 1);
                    Transform last = l.sprites[ind];
                    last.position = new Vector3(last.position.x, last.position.y + levelHeight * 3, last.position.z);
                    //Debug.Log("moved up " + last.name);
                    int newMidIndex = l.indexWrap(l.middleIndex - 1);
                    l.newMid(newMidIndex);
                    
                } else if (GameManager.Instance.Player.transform.position.y < middlePos - levelHeight / 2) {
                    int ind = l.indexWrap(l.middleIndex - 1);
                    Transform first = l.sprites[ind];
                    first.position = new Vector3(first.position.x, first.position.y - levelHeight * 3, first.position.z);
                    //Debug.Log("moved down " + first.name);
                    int newMidIndex = l.indexWrap(l.middleIndex + 1);
                    l.newMid(newMidIndex);
                }
            }
        }
    }

    /// <summary>
    /// Counter-measure for parallax moving sprites on teleport (up/down)
    /// </summary>

    public void MoveSpritesUp() {
        foreach (ParallaxLayer l in paraLayers) {
            foreach (Transform t in l.sprites) {
                t.position = new Vector3(t.position.x, t.position.y + levelHeight*l.scale, t.position.z);
            }
        }
    }
    public void MoveSpritesDown() {
        foreach (ParallaxLayer l in paraLayers) {
            foreach (Transform t in l.sprites) {
                t.position = new Vector3(t.position.x, t.position.y - levelHeight*l.scale, t.position.z);
            }
            
        }
    }
}
