using UnityEngine;
using UnityEngine.InputSystem;

public class PlinkoController : MonoBehaviour
{
    public static PlinkoController instance;

    public InputAction moveAction;
    public InputAction interactAction;

    public GameObject ballPrefab;
    public GameObject plinkoBoard;

    public Vector3 baseSpawnPosition = new Vector3(0.15f, 1.75f, 0);

    protected GameObject currentBall;

    protected Vector2 moveValue;
    protected int interactValue;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this);
                return;
            }
        }
        else
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateActionValues();

        UpdateMovement();
        UpdateInteract();

        SpawnBall();
    }

    void UpdateActionValues()
    {
        moveValue = moveAction.ReadValue<Vector2>();
        interactValue = interactAction.ReadValue<int>();
    }

    void UpdateMovement()
    {

    }

    void UpdateInteract()
    {

    }

    void SpawnBall()
    {
        if (currentBall == null)
        {
            currentBall = Instantiate(ballPrefab);

            currentBall.transform.SetParent(transform);
            currentBall.transform.position = baseSpawnPosition;
        }
    }
}
