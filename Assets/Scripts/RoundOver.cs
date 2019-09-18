using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
	public class RoundOver : MonoBehaviour
	{
        const float X_MOVE_PIXELS = 2463f;
        const float Y_MOVE_PIXELS = 1080f;
        [SerializeField] [Range(0.1f, 5f)] float totalMovementTime = 0.4f;
        [SerializeField] Transform roundOverCanvas = null;

        Transform canvasScreenTransform;
        Vector3 screenStartPos;
        Vector3 screenEndPos;

        Vector2 canvasOrigPos;
        float startingTime;
        float elapsedMovementTime;
        bool movingCanvas = false;
        bool canMove = false;

        private void Start()
        {
            if (roundOverCanvas == null)
            {
                Debug.LogError("No round over canvas found");
            }
            else
            {
                canvasOrigPos = new Vector2(roundOverCanvas.transform.position.x, roundOverCanvas.transform.position.y);
            }
        }

        private void Update()
        {
            if (canMove)
            {
                MoveCanvas();
            }
        }

        public void OnMouseUp()
        {
            ShowRoundOverScreen();
        }

        public void ShowRoundOverScreen()
        {
            MoveHiddenMenu(roundOverCanvas, Direction.Bottom);
            MoveOneShowingMenu(roundOverCanvas, Direction.Bottom, Direction.Top);
            if (!movingCanvas)
            {
                movingCanvas = true;
                canMove = true;
                startingTime = Time.time;
                MoveCanvas();
            }
        }

        public void HideRoundOverScreen()
        {
            MoveHiddenMenu(roundOverCanvas, Direction.Top);
            MoveOneShowingMenu(roundOverCanvas, Direction.Top, Direction.Bottom);
            if (!movingCanvas)
            {
                movingCanvas = true;
                canMove = true;
                startingTime = Time.time;
                MoveCanvas();
            }
        }

        private void MoveHiddenMenu(Transform transformToMove, Direction dirRelativeToMain)
        {
            //Instantly move the requested transform to the position specified
            transformToMove.position = DirectionToPosition(dirRelativeToMain);
        }

        private void MoveOneShowingMenu(Transform transformToMove, Direction dirBeginPos, Direction dirEndPos)
        {
            MoveHiddenMenu(transformToMove, dirBeginPos);
            canvasScreenTransform = transformToMove;
            screenStartPos = canvasScreenTransform.position;
            screenEndPos = DirectionToPosition(dirEndPos);

            if (canvasScreenTransform != null && !movingCanvas)
            {
                movingCanvas = true;
                canMove = true;
                startingTime = Time.time;
                MoveCanvas();
            }
        }

        private Vector3 DirectionToPosition(Direction dirToEndAt)
        {
            switch (dirToEndAt)
            {
                case Direction.Top:
                    return canvasOrigPos + Y_MOVE_PIXELS * Vector2.up;
                case Direction.Right:
                    return canvasOrigPos + Y_MOVE_PIXELS * Vector2.up + X_MOVE_PIXELS * Vector2.right;
                case Direction.Bottom:
                    return canvasOrigPos;
                case Direction.Left:
                    return canvasOrigPos + Y_MOVE_PIXELS * Vector2.up + X_MOVE_PIXELS * Vector2.left;
                case Direction.Nowhere:
                    return canvasOrigPos;
                default:
                    return canvasOrigPos;
            }
        }

        private void MoveCanvas()
        {
            if (movingCanvas)
            {
                elapsedMovementTime = Time.time - startingTime;

                if (elapsedMovementTime > totalMovementTime)
                {
                    elapsedMovementTime = totalMovementTime;
                    movingCanvas = false;
                }

                float t = elapsedMovementTime / totalMovementTime;
                canvasScreenTransform.position = Vector3.Lerp(screenStartPos, screenEndPos, t);
            }
            else
            {
                canMove = false;
                canvasScreenTransform = null;
            }
        }
    }
}
