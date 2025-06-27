public interface IDamagables
{
    void Damage(PlayerVehicleController playerVehicleController, string playerName);
    ulong GetKillerClientİd();
    int GetRespawnTimer();
    int GetDamageAmount();
    string GetKillerName();
}
