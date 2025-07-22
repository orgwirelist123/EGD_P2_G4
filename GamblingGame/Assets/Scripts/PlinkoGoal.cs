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

            if (prizeMoney > 0)
            {
                PlinkoController.instance.successes++;
                float pitch = 1 + PlinkoController.instance.successes * PlinkoController.instance.successMultiplier;
                AudioManager.instance.PlayAudioOneShot("MetalSheet", 0.5f, pitch);
            } 
            else
            {
                PlinkoController.instance.successes = 0;
                AudioManager.instance.PlayAudioOneShot("Thud", 2);
            }

            Destroy(other.gameObject);
        }
    }
}
