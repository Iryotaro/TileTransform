using System;
using UnityEngine;

namespace TileTransforms.Movements
{
    public class Movement
    {
        private MoveData moveData { get; }
        private MoveRate moveRate { get; }
        private TilePosition prevPosition;
        private TilePosition nextPosition;
        public float speed = 1;
        private float setNextPositionTime;
        public bool finish { get; private set; }

        public event Action FinishPublisher;

        public Movement(MoveData moveData, MoveRate moveRate)
        {
            this.moveData = moveData;
            this.moveRate = moveRate;
            prevPosition = moveData.GetNextPosition();
            nextPosition = moveData.GetNextPosition();
            setNextPositionTime = Time.fixedTime;

            FinishPublisher += FinishSubscriber;
        }
        public void Update()
        {
            if ((int)GetTimeFromSetNextPosition() >= 1)
            {
                SetNextPosition();
            }

            if (moveData.finish && !finish && (int)GetTimeFromSetNextPosition() >= 1)
            {
                FinishPublisher?.Invoke();
            }
        }
        public void SetNextPosition()
        {
            if (moveData.finish) return;

            prevPosition = nextPosition;
            nextPosition = moveData.GetNextPosition();

            setNextPositionTime = Time.fixedTime;
        }
        public TilePosition GetTilePosition()
        {
            return nextPosition;
        }
        public Vector2 GetWorldPosition()
        {
            return Vector2.Lerp(prevPosition.GetWorldPosition(), nextPosition.GetWorldPosition(), GetTimeFromSetNextPosition());
        }
        private float GetTimeFromSetNextPosition()
        {
            return (Time.fixedTime - setNextPositionTime) * moveRate.value * speed;
        }
        private void FinishSubscriber()
        {
            finish = true;
        }
    }
}
