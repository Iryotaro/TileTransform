using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Ryocatusn.TileTransforms.Movements
{
    public class Movement
    {
        private MoveData moveData { get; }
        private MoveRate moveRate { get; }
        private TilePosition prevPosition;
        private TilePosition nextPosition;
        public bool isCompleted { get; private set; } = false;
        private bool cancel = false;
        private IDisposable moveUpdateDisposable;
        private CancellationTokenSource moveCancellationToken;

        private Subject<TilePosition> changeTilePositionEvent = new Subject<TilePosition>();
        private Subject<Vector2> changeWorldPositionEvent = new Subject<Vector2>();
        private Subject<Unit> completeEvent = new Subject<Unit>();

        public IObservable<TilePosition> ChangeTilePositionEvent => changeTilePositionEvent;
        public IObservable<Vector2> ChangeWorldPositionEvent => changeWorldPositionEvent;
        public IObservable<Unit> CompleteEvent;

        public Movement(MoveData moveData, MoveRate moveRate)
        {
            this.moveData = moveData;
            this.moveRate = moveRate;

            StartMove().Forget();

            CompleteEvent = completeEvent.FirstOrDefault();

            CompleteEvent.Subscribe(_ =>
            {
                isCompleted = true;
                Dispose();
            });
        }
        private async UniTaskVoid StartMove()
        {
            moveCancellationToken = new CancellationTokenSource();
            await Move().WithCancellation(moveCancellationToken.Token);
        }

        //Cancelは切りのいいところまで進むんでから終了する対してKillはすぐに終了させる
        //普段はCancelを使う
        //Tilemapがnullになってしまう場合などにKillを使用
        public void Cancel()
        {
            cancel = true;
        }
        public void Kill()
        {
            completeEvent.OnNext(Unit.Default);
        }

        private IEnumerator Move()
        {
            int index = 1;
            while (index <= moveData.GetCount() - 1)
            {
                if (cancel)
                {
                    completeEvent.OnNext(Unit.Default);
                    break;
                }

                prevPosition = moveData[index - 1];
                nextPosition = moveData[index];
                index++;

                float setNextPosition = Time.fixedTime;

                moveUpdateDisposable = Observable.EveryUpdate().ObserveOn(Scheduler.MainThread)
                    .Subscribe(x => Update((Time.fixedTime - setNextPosition) * moveRate.value));

                yield return new WaitForSeconds(1 / moveRate.value);

                moveUpdateDisposable.Dispose();
                moveUpdateDisposable = null;
            }
            completeEvent.OnNext(Unit.Default);
        }
        private void Update(float time)
        {
            Vector2 worldPosition = Vector2.Lerp(prevPosition.GetWorldPosition(), nextPosition.GetWorldPosition(), time);
            changeTilePositionEvent.OnNext(nextPosition);
            changeWorldPositionEvent.OnNext(worldPosition);
        }

        private void Dispose()
        {
            if (moveUpdateDisposable != null) moveUpdateDisposable.Dispose();
            moveCancellationToken.Cancel();
            moveCancellationToken.Dispose();

            Observable.NextFrame().Subscribe(_ =>
            {
                changeTilePositionEvent.Dispose();
                changeWorldPositionEvent.Dispose();
                completeEvent.Dispose();
            });
        }
    }
}
