using UnityEngine.UI;

public class CharHeadHealthView : View
{
    public Image HealthImage { get; private set; }
    public Image InstanceHealthImage { get; private set; }
    private void Awake()
    {
        HealthImage = transform.Find("Health Gauge").GetComponentAssert<Image>();
        InstanceHealthImage = transform.Find("Instance Health Gauge").GetComponentAssert<Image>();
    }
}
