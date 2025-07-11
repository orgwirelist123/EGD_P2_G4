using UnityEngine;

public class PlinkoGoal : MonoBehaviour
{
    public float prizeMoney = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            StageManager.instance.AddMoney(prizeMoney);

            Destroy(other.gameObject);
        }
    }
}
