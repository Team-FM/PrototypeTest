using UniRx;

public static class HealthModel
{
    public static readonly ReactiveProperty<int> MaxHealth = new();
    public static readonly ReactiveProperty<int> CurHealth = new();
    public static void SetMaxHealth(int maxHealth)
    {
        MaxHealth.Value = maxHealth;
    }
    public static void SetCurHealth(int curHealth)
    {
        CurHealth.Value = curHealth;
    }
}
