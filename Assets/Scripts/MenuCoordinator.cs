using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
	public class MenuCoordinator : MonoBehaviour
	{
        //In this script I am using the directions as relative to the main game screen.
        //The exception is Direction.Bottom, which is used the same as the main game screen.

        const int NUM_LEVEL_SELECT_SCREENS = 5;
        const int NUM_LEVELS_PER_SELECT_SCREEN = 20;
        const float X_MOVE_PIXELS = 2463f;
        const float Y_MOVE_PIXELS = 1080f;
        [SerializeField] [Range(0.1f, 5f)] float totalMovementTime = 1f;
        [SerializeField] Transform commonItems = null;
        [SerializeField] [Tooltip ("If not 5 here, change in code")] Transform[] levelSelectCanvases = null;

        int currentDisplayedLevelScreen = 0;

		Transform canvasOnScreenTransform;
		Transform canvasOffScreenTransform;
		Vector3 onScreenStartPos;
		Vector3 onScreenEndPos;
		Vector3 offScreenStartPos;
		Vector3 offScreenEndPos;
	
		GameObject[] buttons;
		
		Vector2[] levelCanvasOrigPos = new Vector2[NUM_LEVEL_SELECT_SCREENS];
		float startingTime;
		float elapsedMovementTime;
		bool movingCanvas = false;
		bool canMove = false;
        bool startsOverLevel = false;

        float commonItemsStartingTime;
        float commonItemsElapsedTime;
        bool movingCommonItems = false;
        Direction commonStart = Direction.Top;
        Direction commonEnd = Direction.Top;

        //Just a setup for a future improvement
        LevelLoader levelLoader;

        private void Start()
		{
			buttons = GameObject.FindGameObjectsWithTag("LevelCanvasClickable");

            //Just setup for a future improvement
			levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();

			if (levelSelectCanvases.Length < 1)
			{
				Debug.LogError("No level canvases found");
			} else
			{
				for (int i = 0; i < levelSelectCanvases.Length; i++)
				{
					//All canvases will start on the right side of the main level to begin with,
					//will get moved off screen to the left or right depending on the need
					levelCanvasOrigPos[i] = new Vector2 (levelSelectCanvases[i].transform.position.x, levelSelectCanvases[i].transform.position.y);
				}
			}
		}

		private void Update()
		{
            if (movingCommonItems)
            {
                MoveCommonItems();
            }

            if (canMove)
			{
				MoveCanvas();
			}

            
		}

		public void LevelClickOpen()
		{
            MoveCommonItems(commonItems, Direction.Top, Direction.Bottom);

            MoveHiddenMenu(levelSelectCanvases[0], Direction.Top);
			MoveOneShowingMenu(levelSelectCanvases[0], Direction.Top, Direction.Bottom);
            if (!movingCanvas)
            {
                movingCanvas = true;
                canMove = true;
                startingTime = Time.time;
                DisableLevelButtons();
                MoveCanvas();
            }
            currentDisplayedLevelScreen = 0;
        }

		public void LevelClickLeft()
		{
			//Move current screen right, and lower screen # right, publicly
			if (currentDisplayedLevelScreen == 0)
			{
				MoveTwoShowingMenus(levelSelectCanvases[currentDisplayedLevelScreen], Direction.Bottom, Direction.Right, 
					levelSelectCanvases[NUM_LEVEL_SELECT_SCREENS - 1], Direction.Left, Direction.Bottom);
                currentDisplayedLevelScreen = NUM_LEVEL_SELECT_SCREENS - 1;
			} else
			{
				MoveTwoShowingMenus(levelSelectCanvases[currentDisplayedLevelScreen], Direction.Bottom, Direction.Right, 
					levelSelectCanvases[currentDisplayedLevelScreen - 1], Direction.Left, Direction.Bottom);
                currentDisplayedLevelScreen--;
			}
		}

		public void LevelClickRight()
		{
			//Move current screen left, and higher screen # left, publicly
			if (currentDisplayedLevelScreen == NUM_LEVEL_SELECT_SCREENS - 1)
			{
				MoveTwoShowingMenus(levelSelectCanvases[currentDisplayedLevelScreen], Direction.Bottom, Direction.Left,
					levelSelectCanvases[0], Direction.Right, Direction.Bottom);
                currentDisplayedLevelScreen = 0;
			} else
			{
				MoveTwoShowingMenus(levelSelectCanvases[currentDisplayedLevelScreen], Direction.Bottom, Direction.Left, 
					levelSelectCanvases[currentDisplayedLevelScreen + 1], Direction.Right, Direction.Bottom);
                currentDisplayedLevelScreen++;
			}
		}

		public void LevelClickClose()
		{
			//Move current screen up, publicly
			MoveOneShowingMenu(levelSelectCanvases[currentDisplayedLevelScreen], Direction.Bottom, Direction.Top);

            //Move the menu frame up
            MoveCommonItems(commonItems, Direction.Bottom, Direction.Top);
						
            EnableLevelButtons();
            currentDisplayedLevelScreen = 0;
		}

		private void MoveHiddenMenu(Transform transformToMove, Direction dirRelativeToMain)
		{
            //Instantly move the requested transform to the position specified
            transformToMove.position = DirectionToPosition(dirRelativeToMain);
		}

		private void MoveOneShowingMenu(Transform transformToMove, Direction dirBeginPos, Direction dirEndPos)
		{
			MoveTwoShowingMenus(transformToMove, dirBeginPos, dirEndPos, null, Direction.Top, Direction.Top);
		}

		private void MoveTwoShowingMenus(Transform transformOnScreen, Direction dirOnScreenBeginPos, Direction dirOnScreenEndPos,
			Transform transformOffScreen, Direction dirOffScreenBeginPos, Direction dirOffScreenEndPos)
		{
			MoveHiddenMenu(transformOnScreen, dirOnScreenBeginPos);
			canvasOnScreenTransform = transformOnScreen;
			onScreenStartPos = canvasOnScreenTransform.position;
			onScreenEndPos = DirectionToPosition(dirOnScreenEndPos);

			if (transformOffScreen != null)
			{
				MoveHiddenMenu(transformOffScreen, dirOffScreenBeginPos);
				canvasOffScreenTransform = transformOffScreen;
				offScreenStartPos = canvasOffScreenTransform.position;
				offScreenEndPos = DirectionToPosition(dirOffScreenEndPos);
			}

			if (canvasOnScreenTransform != null && !movingCanvas)
			{
				movingCanvas = true;
				canMove = true;
				startingTime = Time.time;
				if (startsOverLevel)
				{ //Moving away from level
					//EnableLevelButtons(); 
				} else
				{ //Moving towards level
					canvasOnScreenTransform.gameObject.SetActive(true);
					//DisableLevelButtons();
				}
				MoveCanvas();
			}
		}

		private Vector3 DirectionToPosition(Direction dirToEndAt)
		{
			switch (dirToEndAt)
			{
				case Direction.Top:
                    return levelCanvasOrigPos[0];
				case Direction.Right:
					return levelCanvasOrigPos[0] + Y_MOVE_PIXELS * Vector2.down + X_MOVE_PIXELS * Vector2.right;
                case Direction.Bottom:
					return levelCanvasOrigPos[0] + Y_MOVE_PIXELS * Vector2.down;
				case Direction.Left:
					return levelCanvasOrigPos[0] + Y_MOVE_PIXELS * Vector2.down + X_MOVE_PIXELS * Vector2.left;
                case Direction.Nowhere:
					return levelCanvasOrigPos[0];
				default:
					return levelCanvasOrigPos[0];
            }
		}

        private void DisableLevelButtons()
        {
            foreach (GameObject button in buttons)
            {
                button.gameObject.SetActive(false);
            }
        }

        private void EnableLevelButtons()
        {
            foreach (GameObject button in buttons)
            {
                button.gameObject.SetActive(true);
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

				canvasOnScreenTransform.position = Vector3.Lerp(onScreenStartPos, onScreenEndPos, t);

				if (canvasOffScreenTransform != null)
				{
					canvasOffScreenTransform.position = Vector3.Lerp(offScreenStartPos, offScreenEndPos, t);
				}
			} else
			{
				canMove = false;
                canvasOnScreenTransform = null;
                canvasOffScreenTransform = null;
			}
		}

        private void MoveCommonItems(Transform commonItems, Direction dirStart, Direction dirEnd)
        {
            movingCommonItems = true;
            commonItemsStartingTime = Time.time;
            commonStart = dirStart;
            commonEnd = dirEnd;
        }

        private void MoveCommonItems()
        {
            if (movingCommonItems)
            {
                commonItemsElapsedTime = Time.time - commonItemsStartingTime;

                if (commonItemsElapsedTime > totalMovementTime)
                {
                    commonItemsElapsedTime = totalMovementTime;
                    movingCommonItems = false;
                }

                float t = commonItemsElapsedTime / totalMovementTime;

                commonItems.position = Vector3.Lerp(DirectionToPosition(commonStart), DirectionToPosition(commonEnd), t);
            }
            else
            {
                movingCommonItems = false;
            }
        }
	}
}
