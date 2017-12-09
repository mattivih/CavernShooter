using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningController : Photon.PunBehaviour {

    public List<Transform> targets;
    public int LightningCount = 3;
    private int lightningsDone = 0;
    public int StrayLightningAmount = 3;
    public float Delay = 0.1f;
    public float Interval = 0.08f;
    public GameObject LightningPrefab;
    public GameObject Illuminator;
    public LayerMask LayerMaskPlayer;
    public LayerMask LayerMaskEnemy;
    public LayerMask LayerMask;

    public float DistanceRandomnessPercentageOrig;
    float DistanceRandomnessPercentage;

    Vector3 leftPos, rightPos;
    PolygonCollider2D polycol;

    float maxY = float.MinValue;

    Vector3 center;
    float xRad, yRad;
    float OriginalDistance;

    bool drawDebugPoints = false;
    List<pointPair> debugPoints;
    
    struct pointPair {
        public Vector3 start;
        public Vector3 end;

        public pointPair(Vector3 start, Vector3 end) {
            this.start = start;
            this.end = end;
        }
    }

    public AudioClip clipFire;
    private AudioSource audioFire;

    public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip;
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    void Awake() {
        audioFire = GetComponent<AudioSource>();
        debugPoints = new List<pointPair>();
        targets = new List<Transform>();
        polycol = GetComponent<PolygonCollider2D>(); 
        polycol.enabled = false;
    }

    void Start() {
        DistanceRandomnessPercentage = DistanceRandomnessPercentageOrig;

        //DoLightning();
    }

    void Update() {
        if (drawDebugPoints) {
            foreach(pointPair p in debugPoints) {
                Debug.DrawLine(p.start, p.end, Color.red);
            }
        }
    }

    void InitializeLightning() {
        GetLightningBounds();
        Vector3 center = new Vector3((leftPos.x + rightPos.x) / 2, (leftPos.y + rightPos.y) / 2);
        yRad = Mathf.Abs(maxY - center.y);
        xRad = Vector3.Distance(leftPos, rightPos);
        OriginalDistance = center.magnitude*2;
     //   Debug.Log("dist:" + OriginalDistance + ", center: " + center);
    }

    public void StartLightning() {
        targets.Clear();
        InitializeLightning();
        InvokeRepeating("DoLightning", Delay, Interval);
        polycol.enabled = true;
        Illuminator.transform.position = polycol.bounds.center;
        Illuminator.SetActive(true);
    }

    void DoLightning()
    {
        ShowLightning();
    }

    void ShowLightning() {
        if (lightningsDone >= LightningCount) {
            CancelInvoke("DoLightning");
            lightningsDone = 0;
            polycol.enabled = false;
            Illuminator.SetActive(false);
            return;
        }
        if (targets.Count == 0) {
            audioFire.Play();
            DistanceRandomnessPercentage = DistanceRandomnessPercentageOrig;
            for (int i = 0; i < StrayLightningAmount; i++) {
                float angle = Mathf.Deg2Rad * (Random.Range(-180,0));
                Vector3 ellipsePoint = GetEllipsePoint(angle, xRad, yRad);

                Vector3 origin = GetComponentInParent<Ship>().PowerUpPosition.transform.position;
                Vector3 end = transform.TransformPoint(ellipsePoint + Vector3.up * OriginalDistance);

                float distance = OriginalDistance + Random.Range(OriginalDistance * DistanceRandomnessPercentage, -OriginalDistance * DistanceRandomnessPercentage);

                RaycastHit2D hit = Physics2D.Linecast(origin, end, LayerMask);
                if (hit.collider != null) {
                    distance = 0;
                    ellipsePoint = transform.InverseTransformPoint(hit.point);
                    debugPoints.Add(new pointPair(origin, hit.point));
                }
                

                //Debug.DrawLine(transform.TransformPoint(ellipsePoints[i]), transform.TransformPoint(ellipsePoints[i + 1]), Color.red);
                SummonLightning(ellipsePoint + Vector3.up * distance, true);
            }
        } else {
            audioFire.Play();
            DistanceRandomnessPercentage = 0;
            foreach (Transform target in targets) {
                SummonLightning(transform.InverseTransformPoint(target.position), false);
            }
        }
        lightningsDone++;
    }

    void SummonLightning(Vector3 target, bool stray) {
        GameObject newLightning = Instantiate(LightningPrefab, transform);
        RecursiveLightning newLightningScript = newLightning.GetComponent<RecursiveLightning>();
        newLightning.transform.parent = transform;

        newLightningScript.lastVertexPosition = target;
        newLightningScript.mainBranch = true;
        newLightningScript.StrikeLightning();

    }

    void GetLightningBounds() {
        float minX = float.MaxValue, maxX = float.MinValue;
        Vector2[] points = polycol.points;
        //TODO: use bounds instead?
        for (int i = 0; i < points.Length - 1; i++) {
            //Debug.Log(transform.TransformPoint(points[i]));
            //points[i] = transform.TransformPoint(points[i]);
            if (points[i].x < minX) {
                leftPos = points[i];
                minX = points[i].x;
            }
            if (points[i].x > maxX) {
                rightPos = points[i];
                maxX = points[i].x;
            }
            if (points[i].y > maxY) {
                maxY = points[i].y;
            }
        }
    }

    Vector3 GetEllipsePoint(float angle, float a, float b) {
        //Vector3 point;
        Vector3 point2;
        a /= 2;
        b /= 2;

        //float tan = Mathf.Tan(angle);
        //float x = (a * b) / Mathf.Sqrt(b * b + a * a * tan * tan);
        //float y = (a * b * tan) / Mathf.Sqrt(b * b + a * a * tan * tan);
        ////Debug.Log(x+ ","+y+ ", tan: " + tan + ", a: " + a + ", b:" + b);
        //point = new Vector3(x, y);


        //float tan = Mathf.Tan(angle);
        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);
        float bcos2 = Mathf.Pow(b * cos, 2);
        float asin2 = Mathf.Pow(a * sin, 2);
        float x2 = -(a * b * cos) / Mathf.Sqrt(bcos2 + asin2);
        float y2 = -(a * b * sin) / Mathf.Sqrt(bcos2 + asin2);
        //Debug.Log(x+ ","+y+ ", tan: " + tan + ", a: " + a + ", b:" + b);
        point2 = new Vector3(x2, y2);

        //Debug.Log("point1: " + point.x + "," + point.y + "\npoint2: " + point2.x + "," + point2.y);

        return point2;
        //formula source: http://math.stackexchange.com/a/1760296

    }

    public void SetParent(Transform parent) {
        transform.parent = parent;
        transform.localScale = Vector3.one * 10;
    }

    /// <summary>
    /// tests line of sight to target - cant hit lightning through walls
    /// </summary>
    /// <param name="target"></param>

    public bool TryAddTarget(Transform target) {
        Vector3 start = GetComponentInParent<Ship>().PowerUpPosition.transform.position;
        RaycastHit2D[] hits = Physics2D.LinecastAll(start, target.position);
        bool hitbeforewall = false;
        for(int i=0; i < hits.Length; i++) {
            string tag = hits[i].transform.tag;
            if (hits[i].transform == target) {
                hitbeforewall = true;
                break;
            }
            if (tag != "Enemy" && tag != "Player" && hits[i].transform!=target) { //hit wall before enemy;
                break;
            }
        }
        if (hitbeforewall) {
            targets.Add(target);
            return true;
        }
        return false;
    }
}
