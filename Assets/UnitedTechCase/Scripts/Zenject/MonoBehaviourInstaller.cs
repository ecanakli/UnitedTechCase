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
        private ScriptableObject gameData;

        public override void InstallBindings()
        {
            InstallInstanceManagers();
            InstallRuntimeScriptableObjects();
        }

        private void InstallInstanceManagers()
        {
            Container.Bind<ObjectPoolManager>().FromInstance(objectPoolManager).AsSingle().NonLazy();
            Container.Bind<GameManager>().FromInstance(gameManager).AsSingle().NonLazy();
            Container.Bind<UIManager>().FromInstance(uiManager).AsSingle().NonLazy();
            Container.Bind<SpecialPowerManager>().FromInstance(specialPowerManager).AsSingle().NonLazy();
        }

        private void InstallRuntimeScriptableObjects()
        {
            var runtimeGameData = Instantiate((GameData) gameData);
            runtimeGameData.Initialize();
            Container.Bind<GameData>().FromInstance(runtimeGameData).AsSingle().NonLazy();
        }
    }
}