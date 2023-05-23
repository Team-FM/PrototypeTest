using UnityEngine;

public class test : MonoBehaviour
{
    private void Start()
    {
        health = 700;
        HealthModel.SetMaxHealth(health);
        HealthModel.SetCurHealth(health);
    }
    int health = 700;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            health -= 10;
            HealthModel.SetCurHealth(health);
        }
        if (Input.GetKeyDown (KeyCode.W))
        {
            health += 10;
            HealthModel.SetCurHealth(health);
        }
    }
}
