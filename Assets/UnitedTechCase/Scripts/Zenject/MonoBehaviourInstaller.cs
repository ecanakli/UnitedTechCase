using UnitedTechCase.Scripts.Managers;
using UnityEngine;
using Zenject;

namespace UnitedTechCase.Scripts.Zenject
{
    public class MonoBehaviourInstaller : MonoInstaller
    {
        [Header("Managers")]
        [SerializeField]
        private ObjectPoolManager objectPoolManager;

        [SerializeField]
        private GameManager gameManager;

        [SerializeField]
        private UIManager uiManager;

        [SerializeField]
        private SpecialPowerManager specialPowerManager;

        [Header("Scriptable Objects")]
        [SerializeField]
        private ScriptableObject bulletData;

        [SerializeField]
        private ScriptableObject gameData;

        public override void InstallBindings()
        {
            InstallInstanceManagers();
            InstallScriptableObjects();
        }

        private void InstallInstanceManagers()
        {
            Container.Bind<ObjectPoolManager>().FromInstance(objectPoolManager).AsSingle().NonLazy();
            Container.Bind<GameManager>().FromInstance(gameManager).AsSingle().NonLazy();
            Container.Bind<UIManager>().FromInstance(uiManager).AsSingle().NonLazy();
            Container.Bind<SpecialPowerManager>().FromInstance(specialPowerManager).AsSingle().NonLazy();
        }

        private void InstallScriptableObjects()
        {
            Container.Bind<BulletData>().FromScriptableObject((BulletData) bulletData).AsSingle().NonLazy();
            Container.Bind<GameData>().FromScriptableObject((GameData) gameData).AsSingle().NonLazy();
        }
    }
}