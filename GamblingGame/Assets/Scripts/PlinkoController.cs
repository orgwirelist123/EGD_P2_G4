using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    public float wanderDuration = 1f;
    public float wanderRange = 1f;

    public Vector3 baseSpawnPosition = new Vector3(0.15f, 1.75f, 0);

    protected GameObject currentBall;

    protected Vector2 moveValue;
    protected float interactValue;

    protected float lastBallSpawn = 0;
    public float ballCooldown = 0.25f;
    protected bool canSpawnBall = false;

    protected float currentWander = 0;
    protected float wanderMultiplier = 1;
    protected Vector3 lastWanderVector = Vector3.zero;

    public int successes = 0;
    public float successMultiplier = 0.1f;

    public int failures = 0;
    public int failureLimit = 5;

    public bool canAct = true;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(gameObject);
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
        if (!canAct)
        {
            return;
        }

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

        currentWander += Time.deltaTime * wanderMultiplier;
        if (currentWander > wanderDuration || currentWander < 0)
        {
            wanderMultiplier *= -1;
        }
        currentWander = Mathf.Clamp(currentWander, 0, wanderDuration);

        float wanderOffset = wanderRange * (currentWander / wanderDuration);

        Vector3 wanderVector = new Vector3(
            0,
            0,
            wanderOffset - wanderRange / 2
        );
        currentBall.transform.position = newPosition + wanderVector - lastWanderVector;

        lastWanderVector = wanderVector;
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
            currentBall.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            currentBall = null;
            lastBallSpawn = Time.time;

            AudioManager.instance.PlayAudioOneShot("Drop", 1);
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
            currentBall.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void ResetFailures()
    {
        failures = 0;
    }

    public void AddFailure()
    {
        // Check if we're moving to the last skybox; if so, then we proceed, as we are at the last stage
        bool atLastStage = StageManager.instance.moneyCounter >= StageManager.instance.skyboxThresholds[StageManager.instance.skyboxThresholds.Count - 1];
        if (!atLastStage) { return; }

        failures++;

        if (failures >= failureLimit)
        {
            AudioManager.instance.PlayAudioOneShot("Gunshot", 0.25f);
            failures = 0;
            canAct = false;
            StageManager.instance.ExecutePlayer();
        }
    }
}
