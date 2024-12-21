using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnitedTechCase.Scripts.Managers;
using UnityEngine;
using Zenject;

namespace UnitedTechCase.Scripts
{
    public class Bullet : PooledObject
    {
        private float _speed;
        private Vector3 _direction;
        private CancellationTokenSource _bulletMoveCancellationTokenSource;

        [Inject]
        private GameManager _gameManager;

        public void Initialize(BulletData bulletData, Quaternion rotation)
        {
            _speed = bulletData.Speed;
            _direction = rotation * bulletData.Direction.normalized;
            StartMovement().Forget();
        }

        private async UniTaskVoid StartMovement()
        {
            _bulletMoveCancellationTokenSource?.Cancel();
            _bulletMoveCancellationTokenSource = new CancellationTokenSource();

            try
            {
                while (!_bulletMoveCancellationTokenSource.Token.IsCancellationRequested)
                {
                    transform.position += _direction * _speed * Time.deltaTime;

                    if (IsOffScreen())
                    {
                        ReturnToPool();
                        return;
                    }

                    await UniTask.Yield(_bulletMoveCancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Bullet movement cancelled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error in bullet movement: {ex}");
            }
        }

        private bool IsOffScreen()
        {
            if (Camera.main == null)
            {
                return false;
            }

            var screenPosition = Camera.main.WorldToViewportPoint(transform.position);
            return screenPosition.x < 0 || screenPosition.x > 1 || screenPosition.y < 0 || screenPosition.y > 1;
        }

        public override void OnSpawned()
        {
            base.OnSpawned();
            _gameManager.RegisterBullet(this);
        }

        public override void OnDeSpawned()
        {
            base.OnDeSpawned();
            _bulletMoveCancellationTokenSource?.Cancel();
        }

        private void OnDestroy()
        {
            _bulletMoveCancellationTokenSource?.Cancel();
            _bulletMoveCancellationTokenSource?.Dispose();
        }
    }
}