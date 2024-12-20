using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnitedTechCase.Scripts
{
    public class Bullet : PooledObject
    {
        private float _speed;
        private Vector3 _direction;
        private CancellationTokenSource _cancellationTokenSource;

        public void Initialize(BulletData bulletData)
        {
            _speed = bulletData.Speed;
            _direction = bulletData.Direction.normalized;
            StartMovement().Forget();
        }

        private async UniTaskVoid StartMovement()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    transform.position += _direction * _speed * Time.deltaTime;
                    await UniTask.Yield(_cancellationTokenSource.Token);
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

        public override void OnDeSpawned()
        {
            base.OnDeSpawned();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}