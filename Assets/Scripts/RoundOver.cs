using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
    public class RoundOver : MonoBehaviour
    {
        const float X_MOVE_PIXELS = 2463f;
        const float Y_MOVE_PIXELS = 1080f;
        const int MAX_REQUIREMENT = 4;
        [SerializeField] [Range(0.1f, 5f)] float totalMovementTime = 0.4f;
        [SerializeField] Transform roundCanvas = null;
        [SerializeField] Sprite successImage = null;
        [SerializeField] Sprite failureImage = null;
        [SerializeField] Transform[] imageHolders = null;

        LevelRequirements levelRequirements;

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
            if (roundCanvas == null)
            {
                Debug.LogError("No round over canvas found");
            }
            else
            {
                canvasOrigPos = new Vector2(roundCanvas.transform.position.x, roundCanvas.transform.position.y);
            }

            AcquireLevelRequirementsReference();
        }

        private void AcquireLevelRequirementsReference()
        {
            levelRequirements = GameObject.FindObjectOfType<LevelRequirements>().GetComponent<LevelRequirements>();

            if (levelRequirements == null)
            {
                Debug.LogWarning("LevelRequirements not found");
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
            MoveHiddenMenu(roundCanvas, Direction.Bottom);
            MoveOneShowingMenu(roundCanvas, Direction.Bottom, Direction.Top);
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
            MoveHiddenMenu(roundCanvas, Direction.Top);
            MoveOneShowingMenu(roundCanvas, Direction.Top, Direction.Bottom);
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

        private void ResetRequirements()
        {

        }

        private void ShowRequirement()
        {
            
        }

        private void ResetResults()
        {
            for (int i = 0; i < MAX_REQUIREMENT; i++)
            {
                SpriteRenderer sr = imageHolders[i].GetComponent<SpriteRenderer>();
                if (sr == null)
                {
                    Debug.LogWarning("Missing a sprite renderer on an image holder transform");
                    return;
                }
                sr.sprite = null;
            }
        }

        private void ShowResultImage(int requirement)
        {
            if (requirement < 1 || requirement > MAX_REQUIREMENT)
            {
                Debug.LogWarning("Invalid requirement passed in");
                return;
            }

            if (imageHolders[requirement - 1] == null)
            {
                Debug.LogWarning("Missing an imageHolder transform");
                return;
            }

            SpriteRenderer sr = imageHolders[requirement - 1].GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Debug.LogWarning("Missing a sprite renderer on an image holder transform");
                return;
            }

            int level = GetCurrentLevel();
            if(!levelRequirements.DoesLevelRequirementExist(level, requirement))
            {
                //Don't want to show a result if the requirement doesn't exist
                return;
            }       

            if (levelRequirements.IsLevelRequirementMet(level, requirement))
            {
                sr.sprite = successImage;
            }
            else
            {   
                sr.sprite = failureImage;
            }
        }

        private int GetCurrentLevel()
        {
            //TODO implement this
            return 1;
        }
    }
}
