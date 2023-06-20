using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Ryocatusn.TileTransforms
{
    public class Movement
    {
        private MoveData moveData { get; }
        private MoveRate moveRate { get; }
        private TilePosition prevPosition;
        private TilePosition nextPosition;
        public bool isCompleted { get; private set; } = false;
        private bool stop = false;
        public TileDirection moveDirection { get; private set; } = new TileDirection(TileDirection.Direction.Up);
        private IDisposable moveUpdateDisposable;
        private CancellationTokenSource moveCancellationToken;

        private Subject<TilePosition> changeTilePositionEvent = new Subject<TilePosition>();
        private Subject<Vector2> changeWorldPositionEvent = new Subject<Vector2>();
        private Subject<float> changeAngleEvent = new Subject<float>();
        private Subject<Unit> completeEvent = new Subject<Unit>();

        public IObservable<TilePosition> ChangeTilePositionEvent => changeTilePositionEvent;
        public IObservable<Vector2> ChangeWorldPositionEvent => changeWorldPositionEvent;
        public IObservable<float> ChangeAngleEvent => changeAngleEvent;
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
                Delete();
            });
        }
        private async UniTaskVoid StartMove()
        {
            moveCancellationToken = new CancellationTokenSource();
            await Move().WithCancellation(moveCancellationToken.Token);
        }

        //Stopは切りのいいところまで進むんでから終了する対してKillはすぐに終了させる
        public void Stop()
        {
            stop = true;
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
                if (stop)
                {
                    completeEvent.OnNext(Unit.Default);
                    break;
                }

                prevPosition = moveData[index - 1];
                nextPosition = moveData[index];
                index++;

                float angle = -90 + Mathf.Atan2(
                    nextPosition.GetWorldPosition().y - prevPosition.GetWorldPosition().y, 
                    nextPosition.GetWorldPosition().x - prevPosition.GetWorldPosition().x) * Mathf.Rad2Deg;

                moveDirection = new TileDirection(angle);

                float setNextPosition = Time.fixedTime;

                //なぜかMainThreadにしないとOnNextされない
                IDisposable disposable = null; 
                disposable = Observable.Return(Unit.Default)
                    .ObserveOn(Scheduler.MainThread)
                    .Subscribe(_ =>
                    {
                        changeTilePositionEvent.OnNext(nextPosition);
                        changeAngleEvent.OnNext(angle);
                        disposable.Dispose();
                    });

                moveUpdateDisposable = Observable.EveryUpdate()
                    .ObserveOn(Scheduler.MainThreadEndOfFrame)
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
            changeWorldPositionEvent.OnNext(worldPosition);
        }

        private void Delete()
        {
            if (moveUpdateDisposable != null) moveUpdateDisposable.Dispose();
            moveCancellationToken.Cancel();
            moveCancellationToken.Dispose();

            Observable.NextFrame().Subscribe(_ =>
            {
                changeTilePositionEvent.Dispose();
                changeWorldPositionEvent.Dispose();
                changeAngleEvent.Dispose();
                completeEvent.Dispose();
            });
        }
    }
}
