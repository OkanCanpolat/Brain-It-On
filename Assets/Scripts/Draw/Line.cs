using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Line : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private DrawManager drawManager;
    private PolygonCollider2D polyCollider;
    private Rigidbody2D rb;
    private List<Vector2> points;
    private SignalBus signalBus;

    private float lineHalfWidth;

    public int PointCount => lineRenderer.positionCount;
    public LineRenderer LineRenderer => lineRenderer;

    [Inject]
    public void Construct(DrawManager drawManager, SignalBus signalBus)
    {
        this.drawManager = drawManager;
        this.signalBus = signalBus;
        signalBus.Subscribe<LevelFinishedSignal>(OnLevelFinished);
    }
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        polyCollider = GetComponent<PolygonCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        points = new List<Vector2>();

        lineHalfWidth = lineRenderer.startWidth / 2f;
    }

    public void AddPosition(Vector2 position)
    {
        Vector2 relativePos = position - (Vector2)transform.position;

        if (!CanAddPosition(relativePos)) return;

        points.Add(relativePos);

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, relativePos);

        if (lineRenderer.positionCount >= 2)
        {
            int pathNumber = lineRenderer.positionCount - 2;

            List<Vector2> currentPosition = new List<Vector2>
            {
                lineRenderer.GetPosition(lineRenderer.positionCount -2), lineRenderer.GetPosition(lineRenderer.positionCount -1)
            };

            List<Vector2> currentColliderPoints = CalculateColliderPoints(currentPosition);

            polyCollider.SetPath(pathNumber, currentColliderPoints);
            polyCollider.pathCount++;
        }
    }

    private bool CanAddPosition(Vector2 position)
    {
        if (lineRenderer.positionCount == 0) return true;

        return Vector3.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 1), position) > drawManager.MinDrawDistance;
    }

    public void ActivateRigidbody()
    {
        GameObject circleColliderHolder = new GameObject("CircleColliders");
        circleColliderHolder.transform.SetParent(transform, false);
        CircleCollider2D circleCollider = circleColliderHolder.AddComponent<CircleCollider2D>();
        circleCollider.radius = lineHalfWidth;
        circleCollider.offset = points[0];
        CircleCollider2D circleCollider1 = circleColliderHolder.AddComponent<CircleCollider2D>();
        circleCollider1.radius = lineHalfWidth;
        circleCollider1.offset = points[points.Count-1];

        rb.isKinematic = false;
    }
    private List<Vector2> CalculateColliderPoints(List<Vector2> points)
    {
        float xOffset = points[1].x - points[0].x;
        float YOffset = points[1].y - points[0].y;

        float m = YOffset / xOffset;
        float deltaX;

        if (xOffset == 0)
        {
            deltaX = lineHalfWidth;
        }
        else
        {
            deltaX = lineHalfWidth * (m / Mathf.Pow(m * m + 1f, 0.5f));
        }

        float deltaY = lineHalfWidth * (1f / Mathf.Pow(1f + m * m, 0.5f));

        Vector2[] offsets = new Vector2[2];

        offsets[0] = new Vector2(-deltaX, deltaY);
        offsets[1] = new Vector2(deltaX, -deltaY);

        List<Vector2> colliderPoints = new List<Vector2>
        {
            points[0] + offsets[0],
            points[1] + offsets[0],
            points[1] + offsets[1],
            points[0] + offsets[1]
        };
        return colliderPoints;
    }
    private void OnLevelFinished()
    {
        rb.simulated = false;
    }
    private void OnDestroy()
    {
        signalBus.TryUnsubscribe<LevelFinishedSignal>(OnLevelFinished);
    }
}

