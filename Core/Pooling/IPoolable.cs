namespace FCT.Gameplay
{
    /// <summary>
    /// Implement this interface on any component attached to a pooled object
    /// to reset its state automatically when spawned or despawned.
    /// </summary>
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
    }
}
