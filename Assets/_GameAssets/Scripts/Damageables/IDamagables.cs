public interface IDamagables
{
    void Damage(PlayerVehicleController playerVehicleController);
    ulong GetKillerClientİd();
    int GetRespawnTimer();
}
