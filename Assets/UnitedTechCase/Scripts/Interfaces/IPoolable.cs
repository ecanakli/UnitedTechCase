namespace UnitedTechCase.Scripts.Interfaces
{
    public interface IPoolable
    {
        void OnSpawned();
        void OnDeSpawned();
        void ReturnToPool();
    }
}