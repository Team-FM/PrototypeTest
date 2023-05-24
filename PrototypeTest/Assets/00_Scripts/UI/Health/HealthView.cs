using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : View
{
    public Image HealthImage { get; private set; }
    public TMP_Text CurHealthText { get; private set; }
    public TMP_Text MaxHealthText { get; private set; }
    private void Awake()
    {
        HealthImage = transform.Find("Health Gauge").GetComponentAssert<Image>();
        CurHealthText = transform.Find("Health Text Group").Find("Current Health Text").GetComponent<TMP_Text>();
        MaxHealthText = transform.Find("Health Text Group").Find("Max Health Text").GetComponent<TMP_Text>();
    }
}