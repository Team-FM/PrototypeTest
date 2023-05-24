using UniRx;

public static class CharacterStatusModel
{
    public static readonly ReactiveProperty<int> MaxHealth = new();
    public static readonly ReactiveProperty<int> CurHealth = new();
    public static readonly ReactiveProperty<int> CurLevel = new();
    public static readonly ReactiveProperty<string> NickName = new();

    public static void SetMaxHealth(int maxHealth)
    {
        MaxHealth.Value = maxHealth;
    }
    public static void SetCurHealth(int curHealth)
    {
        CurHealth.Value = curHealth;
    }
    public static void SetCurLevel(int curLevel)
    {
        CurLevel.Value = curLevel;
    }
    public static void SetNickName(string nickName)
    {
        NickName.Value = nickName;
    }
}
