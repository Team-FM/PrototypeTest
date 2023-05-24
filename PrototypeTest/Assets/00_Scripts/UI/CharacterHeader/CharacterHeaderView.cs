using TMPro;
using UnityEngine.UI;

public class CharacterHeaderView : View
{
    public Image InstanceHealthImage { get; private set; }
    public Image HealthImage { get; private set; }
    public TMP_Text LevelText { get; private set; }
    public TMP_Text NickNameText { get; private set; }
    private void Awake()
    {
        InstanceHealthImage = transform.Find("Instance Health Gauge").GetComponentAssert<Image>();
        HealthImage = transform.Find("Health Gauge").GetComponentAssert<Image>();
        LevelText = transform.Find("Level").Find("Level Text").GetComponentAssert<TMP_Text>();
        NickNameText = transform.Find("Nick Name Text").GetComponentAssert<TMP_Text>();
    }
}
