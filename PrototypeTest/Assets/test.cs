using UnityEngine;

public class test : MonoBehaviour
{
    int health;
    private void Start()
    {
        health = 500;
        HealthModel.SetMaxHealth(health);
        HealthModel.SetCurHealth(health);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            health -= 50;
            HealthModel.SetCurHealth(health);
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            health += 50;
            HealthModel.SetCurHealth(health);
        }
    }
}
