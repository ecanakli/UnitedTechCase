using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace UnitedTechCase.Scripts
{
    public class Character : PooledObject
    {
        [SerializeField]
        private GameObject[] characterModels;

        [SerializeField]
        private Transform firePoint;

        private GameData _gameData;

        private CancellationTokenSource _moveCancellationTokenSource;

        public void Initialize(GameData gameData)
        {
            _gameData = gameData;
        }

        public void SetCharacterModel(int modelIndex)
        {
            for (var i = 0; i < characterModels.Length; i++)
            {
                characterModels[i].SetActive(i == modelIndex);
            }
        }

        public void Fire()
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
            var angleSpread = _gameData.BulletData.AngleSpread;
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
                DoubleShotBullet(position, rotation).Forget();
            }
            else
            {
                var bullet = ObjectPoolManager.Spawn<Bullet>(position, rotation);
                bullet.Initialize(_gameData.BulletData, rotation);
            }
        }

        private async UniTask DoubleShotBullet(Vector3 position, Quaternion rotation)
        {
            try
            {
                for (var i = 0; i < 2; i++)
                {
                    var bullet = ObjectPoolManager.Spawn<Bullet>(position, rotation);
                    bullet.Initialize(_gameData.BulletData, rotation);
                    await UniTask.Delay(100, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Double Shot Bullet movement cancelled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error in bullet movement: {ex}");
            }
        }

        public async void OnGameEnd(Vector3 movePosition)
        {
            try
            {
                await Move(movePosition).AttachExternalCancellation(_moveCancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Character move operation was canceled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error during OnGameEnd: {ex}");
            }
            finally
            {
                ReturnToPool();
            }
        }

        public async UniTask Move(Vector3 movePosition)
        {
            CancelMove();
            _moveCancellationTokenSource = new CancellationTokenSource();
            RotateTowards(movePosition, _moveCancellationTokenSource.Token).Forget();
            await transform.DOMove(movePosition, 1f)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(_moveCancellationTokenSource.Token);
        }

        private async UniTask RotateTowards(Vector3 targetPosition, CancellationToken cancellationToken)
        {
            var direction = (targetPosition - transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(direction).eulerAngles;

            await transform.DORotate(targetRotation, 0.3f, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuad)
                .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
        }

        public override void OnDeSpawned()
        {
            base.OnDeSpawned();
            CancelMove();
            DisableCharacterModels();
        }

        private void DisableCharacterModels()
        {
            foreach (var mainCharacter in characterModels)
            {
                mainCharacter.gameObject.SetActive(false);
            }
        }

        private void CancelMove()
        {
            _moveCancellationTokenSource?.Cancel();
        }

        private void OnDestroy()
        {
            _moveCancellationTokenSource?.Cancel();
            _moveCancellationTokenSource?.Dispose();
        }
    }
}