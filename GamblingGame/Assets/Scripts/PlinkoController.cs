using UnityEngine;
using UnityEngine.InputSystem;

public class PlinkoController : MonoBehaviour
{
    public static PlinkoController instance;

    public InputAction moveAction;
    public InputAction interactAction;

    public GameObject ballPrefab;
    public GameObject plinkoBoard;

    public GameObject leftBound;
    public GameObject rightBound;
    public GameObject spawnPosition;

    public float moveSpeed = 0.5f;

    public Vector3 baseSpawnPosition = new Vector3(0.15f, 1.75f, 0);

    protected GameObject currentBall;

    protected Vector2 moveValue;
    protected float interactValue;

    protected float lastBallSpawn = 0;
    public float ballCooldown = 0.25f;
    protected bool canSpawnBall = false;

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
        interactValue = interactAction.ReadValue<float>();
    }

    void UpdateMovement()
    {
        if (currentBall == null) { return; }
        float min = Mathf.Min(leftBound.transform.position.z, rightBound.transform.position.z);
        float max = Mathf.Max(leftBound.transform.position.z, rightBound.transform.position.z);
        Vector3 newPosition = new Vector3(
            currentBall.transform.position.x,
            currentBall.transform.position.y,
            Mathf.Clamp(currentBall.transform.position.z + moveValue.x * moveSpeed, min, max)
        );
        currentBall.transform.position = newPosition;
    }

    void UpdateInteract()
    {
        if (interactValue == 0)
        {
            canSpawnBall = true;
        }

        if (currentBall != null && interactValue >= 1 && canSpawnBall)
        {
            canSpawnBall = false;

            currentBall.GetComponent<Rigidbody>().useGravity = true;
            currentBall = null;
            lastBallSpawn = Time.time;
        }
    }

    void SpawnBall()
    {
        if (currentBall == null && Time.time > lastBallSpawn + ballCooldown)
        {
            currentBall = Instantiate(ballPrefab);

            currentBall.transform.SetParent(transform);

            if (spawnPosition != null)
            {
                currentBall.transform.position = spawnPosition.transform.position;
            }
            else
            {
                currentBall.transform.localPosition = baseSpawnPosition;
            }

            currentBall.GetComponent<Rigidbody>().useGravity = false;
        }
    }
}
