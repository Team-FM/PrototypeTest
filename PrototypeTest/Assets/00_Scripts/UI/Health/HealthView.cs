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
        Transform heatlhTextGroup = transform.Find("Health Text Group");
        CurHealthText = heatlhTextGroup.Find("Current Health Text").GetComponent<TMP_Text>();
        MaxHealthText = heatlhTextGroup.Find("Max Health Text").GetComponent<TMP_Text>();
    }
}