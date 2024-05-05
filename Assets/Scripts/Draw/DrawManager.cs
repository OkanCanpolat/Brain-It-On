using UnityEngine;
using Zenject;

public class DrawManager : MonoBehaviour
{
    public DrawStateMachine StateMachine;
    [SerializeField] private GameObject oneShotCircle;
    [SerializeField] private float minDrawDistance;
    [SerializeField] private LayerMask cantDrawLayer;
    [SerializeField] private LayerMask canDrawLayer;
    [SerializeField] private float lineWidth;

    private Camera mainCam;
    private Line currentLine;

    private int cantDrawLayerIndex;
    private int canDrawLayerIndex;
    public float MinDrawDistance => minDrawDistance;
    public float LineWidth => lineWidth;
    public Line CurrentLine { get => currentLine; set => currentLine = value; }
    public LayerMask CanDrawLayer => canDrawLayer;
    public LayerMask CantDrawLayer => cantDrawLayer;
    public int CantDrawLayerIndex => cantDrawLayerIndex;
    public int CanDrawLayerIndex => canDrawLayerIndex;

    [Inject]
    public void Construct(DrawStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }
    private void Awake()
    {
        mainCam = Camera.main;
        cantDrawLayerIndex = LayerMask.NameToLayer("CantDraw");
        canDrawLayerIndex = LayerMask.NameToLayer("CanDraw");
    }

    private void Update()
    {
        StateMachine.CurrentState.OnUpdate();
    }
    public void EndDraw()
    {
        if (currentLine != null)
        {
            if (currentLine.PointCount < 2)
            {
                Destroy(currentLine.gameObject);
                Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                GameObject circle = Instantiate(oneShotCircle, new Vector3(mousePos.x, mousePos.y, mainCam.transform.position.z + 1f), Quaternion.identity);
                circle.layer = cantDrawLayerIndex;
                circle.GetComponent<Rigidbody2D>().isKinematic = false;
                currentLine = null;
            }

            else
            {
                currentLine.ActivateRigidbody();
                currentLine.gameObject.layer = cantDrawLayerIndex;
                currentLine = null;
            }
        }
    }
}

public class OnDrawSignal { }
