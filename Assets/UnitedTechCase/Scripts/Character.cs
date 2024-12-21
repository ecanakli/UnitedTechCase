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
            CheckExtraBulletSpecialPower();
        }

        private void CheckExtraBulletSpecialPower()
        {
            if (_gameData.BulletData.ExtraBullets <= 0)
            {
                return;
            }

            var totalExtraBullets = _gameData.BulletData.ExtraBullets;
            var angleSpread = 90f;
            var startAngle = -angleSpread / 2f;

            for (var i = 0; i < totalExtraBullets; i++)
            {
                var angle = startAngle + (i * angleSpread / (totalExtraBullets - 1));
                var rotation = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0, angle, 0));
                CreateBullet(firePoint.position, rotation);
            }
        }


        private void CreateBullet(Vector3 position, Quaternion rotation)
        {
            if (_gameData.DoubleShotEnabled)
            {
                DoubleShotBullet(position, rotation, _fireCancellationTokenSource.Token).Forget();
            }
            else
            {
                var bullet = ObjectPoolManager.Spawn<Bullet>(position, rotation);
                bullet.Initialize(_gameData.BulletData, rotation);
            }
        }

        private async UniTask DoubleShotBullet(Vector3 position, Quaternion rotation,
            CancellationToken cancellationToken)
        {
            for (var i = 0; i < 2; i++)
            {
                var bullet = ObjectPoolManager.Spawn<Bullet>(position, rotation);
                bullet.Initialize(_gameData.BulletData, rotation);
                await UniTask.Delay(100, cancellationToken: cancellationToken);
            }
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