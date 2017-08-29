using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// handles Lightning visuals
/// generates branches on left and right side of the lightning up to MaxBranches at random position along the parent branch
/// </summary>

[RequireComponent(typeof(LineRenderer))]
public class RecursiveLightning : NetworkBehaviour {
    [Tooltip("Lightning quality")]
    public int vertexCount = 17;
    public Vector3[] vertices;
    [HideInInspector]
    public Vector3 firstVertexPosition;
    [HideInInspector]
    public Vector3 lastVertexPosition;

    [HideInInspector]
    public bool stray = false;
    [HideInInspector]
    public bool mainBranch = false;
    [HideInInspector]
    public RecursiveLightning leftBranchScript = null;
    [HideInInspector]
    public RecursiveLightning rightBranchScript = null;
    LineRenderer lineRenderer;
    int leftBranchVertex = -1;
    int rightBranchVertex = -1;
    [Tooltip("Fade out lightning after a strike?")]
    public bool fadeOutAfterStrike = true;
    [Tooltip("time for a single lightning fade out")]
    public float fadeOutTime = 0.3f;

    [Tooltip("Branch randomness horizontally in local space")]
    public Vector2 BranchRandomRangeX;
    [HideInInspector]
    private Vector2 _branchRandomRangeX;

    [Tooltip("Branch randomness vertically in local space")]
    public Vector2 BranchRandomRangeY;
    [HideInInspector]
    private Vector2 _branchRandomRangeY;

    [Tooltip("Maximum amount of sub branches")]
    public int MaxBranches = 2;
    [HideInInspector]
    public int _maxBranches = 2;

    [HideInInspector]
    public int BranchDepth = 0;

    [Tooltip("Main Lightning randomness")]
    public float RandomRange = 0.01f;
    [HideInInspector]
    private float _randomRange = 0.01f;

    void Awake() {
        InitializeLineRenderer();
    }

    void InsertFirstAndLastNode() {
        InsertNodeInLineRenderer(0, firstVertexPosition);
        InsertNodeInLineRenderer(vertexCount - 1, lastVertexPosition);
    }

    void InsertVertexBetween(int start, int end) {
        int currentVertexNumber = (start + end) / 2;

        if (currentVertexNumber != start) {
            vertices[currentVertexNumber] = (vertices[start] + vertices[end]) / 2 + new Vector3(Random.Range(-_randomRange, _randomRange), Random.Range(-_randomRange, _randomRange));

            InsertNodeInLineRenderer(currentVertexNumber, vertices[currentVertexNumber]);

            InsertVertexBetween(start, currentVertexNumber);
            InsertVertexBetween(currentVertexNumber, end);
        }

        if (leftBranchVertex == currentVertexNumber) {
            ConfigureBranch(leftBranchScript, vertices[currentVertexNumber], vertices[currentVertexNumber] + new Vector3(Random.Range(_branchRandomRangeX.x, _branchRandomRangeX.y), _branchRandomRangeY.x, _branchRandomRangeY.y));
            leftBranchScript.StrikeLightning();
        }

        if (rightBranchVertex == currentVertexNumber) {
            ConfigureBranch(rightBranchScript, vertices[currentVertexNumber], vertices[currentVertexNumber] + new Vector3(Random.Range(_branchRandomRangeX.x, _branchRandomRangeX.y), _branchRandomRangeY.x, _branchRandomRangeY.y));
            rightBranchScript.StrikeLightning();
        }
    }

    void ConfigureBranch(RecursiveLightning branch, Vector3 firstVertexPosition, Vector3 lastVertexPosition) {
        branch.firstVertexPosition = firstVertexPosition;

        if (lastVertexPosition.y > vertices[vertexCount - 1].y)
            lastVertexPosition.y = vertices[vertexCount - 1].y;

        branch.lastVertexPosition = lastVertexPosition;

        branch.fadeOutTime = fadeOutTime;
        branch.fadeOutAfterStrike = fadeOutAfterStrike;
    }


    void InsertNodeInLineRenderer(int position, Vector3 vertex) {
        vertex = transform.TransformPoint(vertex);
        vertex.z = 4;
        lineRenderer.SetPosition(position, vertex);
    }

    void InitializeLineRenderer() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = vertexCount;

        lineRenderer.enabled = false;
    }

    public void StrikeLightning() {
        if (!lineRenderer.enabled) {
            if (stray) {
                _branchRandomRangeX = BranchRandomRangeX;
                _branchRandomRangeY = BranchRandomRangeY;
                _randomRange = RandomRange;
                _maxBranches = MaxBranches;
            } else {
                _branchRandomRangeX = new Vector2(0,0);
                _branchRandomRangeY = new Vector2(0, 0);
                _randomRange = RandomRange/10;
                _maxBranches = MaxBranches * 2;
                
            }
            if (mainBranch) {
                firstVertexPosition = transform.InverseTransformPoint(GetComponentInParent<Ship>().PowerUpPosition.transform.position);
            }

            if (BranchDepth < MaxBranches) {
                GameObject leftBranch = Instantiate(gameObject, transform.position, transform.rotation, transform);
                GameObject rightBranch = Instantiate(gameObject, transform.position, transform.rotation, transform);
                leftBranch.transform.parent = transform;
                rightBranch.transform.parent = transform;
                leftBranchScript = leftBranch.GetComponent<RecursiveLightning>();
                rightBranchScript = rightBranch.GetComponent<RecursiveLightning>();
                leftBranchScript.BranchDepth = BranchDepth + 1;
                rightBranchScript.BranchDepth = BranchDepth + 1;
            }

            if (leftBranchScript) {
                leftBranchVertex = Random.Range(0, vertexCount - 1);
            }

            if (rightBranchScript) {
                rightBranchVertex = Random.Range(0, vertexCount - 1);
            }

            vertices = new Vector3[vertexCount];

            vertices[0] = firstVertexPosition;
            vertices[vertexCount - 1] = lastVertexPosition;

            lineRenderer.enabled = true;
            InsertFirstAndLastNode();
            InsertVertexBetween(0, vertexCount - 1);

            if (fadeOutAfterStrike)
                FadeOut();
        }
    }

    public void FadeOut() {
        StartCoroutine("FadeOutQuickly");
    }

    IEnumerator FadeOutQuickly() {
        yield return new WaitForSeconds(fadeOutTime);
        lineRenderer.enabled = false;
        Destroy(gameObject, 1f);
    }
}
