using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
	public class LevelSelectCoordinator : MonoBehaviour
	{
		[SerializeField] [Range(0.1f, 5f)] float totalMovementTime = 1f;
		const int NUM_LEVEL_SELECT_SCREENS = 4;
		const int NUM_LEVELS_PER_SELECT_SCREEN = 20;
		[SerializeField] [Tooltip ("If not 4 here, change in code")] Transform[] levelSelectCanvases;
		int currentDisplayedLevelScreen = 0;

		Transform canvasOnScreenTransform;
		Transform canvasOffScreenTransform;
		Vector3 onScreenStartPos;
		Vector3 onScreenEndPos;
		Vector3 offScreenStartPos;
		Vector3 offScreenEndPos;
		float xMoveAmount = 2463f;
		float yMoveAmount = 1080f;
		GameObject[] buttons;
		LevelLoader levelLoader;
		LevelCanvasSpot[] levelCanvasSpots = new LevelCanvasSpot[NUM_LEVEL_SELECT_SCREENS];
		Vector2[] levelCanvasOrigPos = new Vector2[NUM_LEVEL_SELECT_SCREENS];
		Vector2[] levelCanvasCurrentPos = new Vector2[NUM_LEVEL_SELECT_SCREENS];
		float startingTime;
		float elapsedMovementTime;
		bool movingCanvas = false;
		bool canMove = false;
		bool startsOverLevel;
		 
		
		private void Start()
		{
			buttons = GameObject.FindGameObjectsWithTag("LevelCanvasClickable");
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
					levelCanvasSpots[i] = LevelCanvasSpot.Right;
					levelCanvasOrigPos[i] = new Vector2 (levelSelectCanvases[i].transform.position.x, levelSelectCanvases[i].transform.position.y);
					levelCanvasCurrentPos[i] = new Vector2(levelSelectCanvases[i].transform.position.x, levelSelectCanvases[i].transform.position.y);
				}
			}
		}

		private void Update()
		{
			if (canMove)
			{
				MoveCanvas();
			}
		}

		public void LevelClickOpen()
		{
			int currentLevel = levelLoader.GetLevel();
			int screenNum = currentLevel / NUM_LEVELS_PER_SELECT_SCREEN;
			
			//Move current screen to above the main screen, then slide it down
			MoveHiddenMenu(levelSelectCanvases[screenNum], Direction.Top);
			MoveOneShowingMenu(levelSelectCanvases[screenNum], Direction.Top, Direction.Bottom);
			currentDisplayedLevelScreen = screenNum;
		}

		public void LevelClickLeft()
		{
			//Move current screen right, and lower screen # right, publicly
			if (currentDisplayedLevelScreen == 0)
			{
				MoveTwoShowingMenus(levelSelectCanvases[currentDisplayedLevelScreen], Direction.Bottom, Direction.Right, 
					levelSelectCanvases[NUM_LEVEL_SELECT_SCREENS - 1], Direction.Left, Direction.Bottom);
			} else
			{
				MoveTwoShowingMenus(levelSelectCanvases[currentDisplayedLevelScreen], Direction.Bottom, Direction.Right, 
					levelSelectCanvases[currentDisplayedLevelScreen - 1], Direction.Left, Direction.Bottom);
			}
		}

		public void LevelClickRight()
		{
			//Move current screen left, and higher screen # left, publicly
			if (currentDisplayedLevelScreen == NUM_LEVEL_SELECT_SCREENS - 1)
			{
				MoveTwoShowingMenus(levelSelectCanvases[currentDisplayedLevelScreen], Direction.Bottom, Direction.Left,
					levelSelectCanvases[0], Direction.Right, Direction.Bottom);
			} else
			{
				MoveTwoShowingMenus(levelSelectCanvases[currentDisplayedLevelScreen], Direction.Bottom, Direction.Left, 
					levelSelectCanvases[currentDisplayedLevelScreen + 1], Direction.Right, Direction.Bottom);
			}
		}

		public void LevelClickClose()
		{
			//Move current screen up, publicly
			MoveOneShowingMenu(levelSelectCanvases[currentDisplayedLevelScreen], Direction.Bottom, Direction.Top);
			
			//Move current screen to the right, once done with the public sliding
			//How to navigate the definite timing issue here?

			//Move all screens to the right, privately
			for (int i = 0; i < NUM_LEVEL_SELECT_SCREENS; i++)
			{
				if (i != currentDisplayedLevelScreen)
				{
					MoveHiddenMenu(levelSelectCanvases[i], Direction.Right);
				}
			}
		}

		private void MoveHiddenMenu(Transform transformToMove, Direction dirRelativeToMain)
		{
			//Instantly move the requested transform to the position specified
			switch (dirRelativeToMain)
			{
				case Direction.Top:
					transformToMove.position = levelCanvasOrigPos[0] + yMoveAmount * Vector2.up + xMoveAmount * Vector2.left;
					break;
				case Direction.Right:
					transformToMove.position = levelCanvasOrigPos[0];
					break;
				case Direction.Bottom:
					transformToMove.position = levelCanvasOrigPos[0] + xMoveAmount * Vector2.left;
					break;
				case Direction.Left:
					transformToMove.position = levelCanvasOrigPos[0] + 2.0f * xMoveAmount * Vector2.left;
					break;
				case Direction.Nowhere:
					transformToMove.position = levelCanvasOrigPos[0];
					break;
				default:
					transformToMove.position = levelCanvasOrigPos[0];
					break;
			}
		}

		private void MoveOneShowingMenu(Transform transformToMove, Direction dirBeginPos, Direction dirEndPos)
		{
			MoveTwoShowingMenus(transformToMove, dirBeginPos, dirEndPos, null, Direction.Right, Direction.Right);
		}

		private void MoveTwoShowingMenus(Transform transformOnScreen, Direction dirOnScreenBeginPos, Direction dirOnScreenEndPos,
			Transform transformOffScreen, Direction dirOffScreenBeginPos, Direction dirOffScreenEndPos)
		{
			MoveHiddenMenu(transformOnScreen, dirOnScreenBeginPos);
			canvasOnScreenTransform = transformOnScreen;
			onScreenStartPos = canvasOnScreenTransform.position;
			onScreenEndPos = CalcEndPos(dirOnScreenEndPos);

			if (transformOffScreen != null)
			{
				MoveHiddenMenu(transformOffScreen, dirOffScreenBeginPos);
				canvasOffScreenTransform = transformOffScreen;
				offScreenStartPos = canvasOffScreenTransform.position;
				offScreenEndPos = CalcEndPos(dirOffScreenEndPos);
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

		private Vector3 CalcEndPos(Direction dirToEndAt)
		{
			switch (dirToEndAt)
			{
				case Direction.Top:
					return levelCanvasOrigPos[0] + yMoveAmount * Vector2.up + xMoveAmount * Vector2.left;
				case Direction.Right:
					return levelCanvasOrigPos[0];
				case Direction.Bottom:
					return levelCanvasOrigPos[0] + xMoveAmount * Vector2.left;
				case Direction.Left:
					return levelCanvasOrigPos[0] + 2.0f * xMoveAmount * Vector2.left;
				case Direction.Nowhere:
					return levelCanvasOrigPos[0];
				default:
					return levelCanvasOrigPos[0];
			}
		}

		//private void DisableLevelButtons()
		//{
		//	foreach (GameObject button in buttons)
		//	{
		//		button.gameObject.SetActive(false);
		//	}
		//}

		//private void EnableLevelButtons()
		//{
		//	foreach (GameObject button in buttons)
		//	{
		//		button.gameObject.SetActive(true);
		//	}
		//}

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
			}
		}
	}

	public enum LevelCanvasSpot { Left, Center, Right}
}
