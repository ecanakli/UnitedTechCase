using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace UnitedTechCase.Scripts
{
    public class Character : PooledObject
    {
        [SerializeField]
        private Transform firePoint;

        private GameData _gameData;

        private CancellationTokenSource _moveCancellationTokenSource;
        private CancellationTokenSource _fireCancellationTokenSource;

        public void Initialize(GameData gameData)
        {
            _gameData = gameData;
        }

        public void StartFiring()
        {
            _fireCancellationTokenSource?.Cancel();
            _fireCancellationTokenSource = new CancellationTokenSource();
            FireContinuously(_fireCancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid FireContinuously(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Fire();
                await UniTask.Delay(Mathf.RoundToInt(_gameData.BaseFireRate * 1000), cancellationToken: token);
            }
        }

        private void Fire()
        {
            CreateBullet(firePoint.position, firePoint.rotation);
            CheckAndApplySpecialPowers();
        }

        private void CheckAndApplySpecialPowers()
        {
            CheckExtraBulletSpecialPower();
            CheckDoubleShotSpecialPower();
        }

        private void CheckExtraBulletSpecialPower()
        {
            if (_gameData.BulletData.ExtraBullets <= 0)
            {
                return;
            }

            for (var i = 0; i < _gameData.BulletData.ExtraBullets; i++)
            {
                var angle = (i == 0) ? -45f : 45f;
                var rotation = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0, angle, 0));
                CreateBullet(firePoint.position, rotation);
            }
        }

        private void CheckDoubleShotSpecialPower()
        {
            if (!_gameData.DoubleShotEnabled)
            {
                return;
            }

            DoubleShot().Forget();
        }

        private async UniTaskVoid DoubleShot()
        {
            await UniTask.Delay(100);
            CreateBullet(firePoint.position, firePoint.rotation);
        }

        private void CreateBullet(Vector3 position, Quaternion rotation)
        {
            var bullet = ObjectPoolManager.Spawn<Bullet>(position, rotation);
            bullet.Initialize(_gameData.BulletData);
        }

        public async void OnGameEnd(Vector3 movePosition)
        {
            await Move(movePosition);
            ReturnToPool();
        }

        public async UniTask Move(Vector3 movePosition)
        {
            _moveCancellationTokenSource?.Cancel();
            _moveCancellationTokenSource = new CancellationTokenSource();
            await transform.DOMove(movePosition, 0.5f)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(_moveCancellationTokenSource.Token);
        }


        public override void OnDeSpawned()
        {
            base.OnDeSpawned();
            _moveCancellationTokenSource?.Cancel();
            _fireCancellationTokenSource?.Cancel();
        }

        private void OnDestroy()
        {
            _moveCancellationTokenSource?.Cancel();
            _fireCancellationTokenSource?.Cancel();
            _moveCancellationTokenSource?.Dispose();
            _fireCancellationTokenSource?.Dispose();
        }
    }
}