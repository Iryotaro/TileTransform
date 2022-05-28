using System.Threading.Tasks;
using UnityEngine;

namespace TileTransforms.Movements
{
    public class Movement
    {
        private MoveData moveData { get; }
        private MoveRate moveRate { get; }
        private TilePosition prevPosition;
        private TilePosition nextPosition;
        private float setNextPositionTime;

        public Movement(MoveData moveData, MoveRate moveRate)
        {
            this.moveData = moveData;
            this.moveRate = moveRate;
            prevPosition = moveData.GetNextPosition();
            nextPosition = moveData.GetNextPosition();
            setNextPositionTime = Time.fixedTime;

            SetNextPosition();
        }

        public Vector2 GetWorldPosition()
        {
            return Vector2.Lerp(prevPosition.GetWorldPosition(), nextPosition.GetWorldPosition(), GetTimeFromSetNextPosition());
        }
        public TilePosition GetTilePosition()
        {
            return nextPosition;
        }
        public bool IsCompleted()
        {
            if (moveData.completed && (int)GetTimeFromSetNextPosition() >= 1) return true;
            return false;
        }

        private async void SetNextPosition()
        {
            if (moveData.completed) return;

            prevPosition = nextPosition;
            nextPosition = moveData.GetNextPosition();

            setNextPositionTime = Time.fixedTime;

            float waitTime = 1 / moveRate.value;
            await Task.Delay((int)waitTime * 1000);
            SetNextPosition();
        }
        private float GetTimeFromSetNextPosition()
        {
            return (Time.fixedTime - setNextPositionTime) * moveRate.value;
        }
    }
}
