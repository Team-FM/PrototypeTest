using UnityEngine;

public class Tmp : MonoBehaviour
{
    public GameObject OccuapationBar;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OccuapationBar.SetActive(true);
        }
    }
}