using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
	public class MenuCoordinator : MonoBehaviour
	{
		[SerializeField] [Range(0.1f, 5f)] float totalMovementTime = 1f;

		Transform canvasTransform;
		Vector3 startPos;
		Vector3 endPos;
		float xMoveAmount = 1920f;
		float yMoveAmount = 1080f;
		GameObject[] buttons;
		float startingTime;
		float elapsedMovementTime;
		bool movingCanvas = false;
		bool canMove = false;
		bool startsOverLevel;
		
		private void Start()
		{
			buttons = GameObject.FindGameObjectsWithTag("LevelCanvasClickable"); 
		}

		public void Update()
		{
			if (canMove)
			{
				MoveCanvas();
			}
		}

		public void MoveMenu(Transform transformToMove, Direction dirToMoveTowards, bool startedOverLevel)
		{
			canvasTransform = transformToMove;
			startsOverLevel = startedOverLevel;
			startPos = canvasTransform.position;
			endPos = CalcEndPos(dirToMoveTowards);

			if (canvasTransform != null && !movingCanvas)
			{
				movingCanvas = true;
				canMove = true;
				startingTime = Time.time;
				if (startsOverLevel)
				{ //Moving away from level
					EnableLevelButtons(); 
				} else
				{ //Moving towards level
					canvasTransform.gameObject.SetActive(true);
					DisableLevelButtons();
				}
				MoveCanvas();
			}
		}

		private Vector3 CalcEndPos(Direction dirToMoveTowards)
		{
			switch (dirToMoveTowards)
			{
				case Direction.Top:
					return startPos + yMoveAmount * Vector3.up;
				case Direction.Right:
					return startPos + xMoveAmount * Vector3.right;
				case Direction.Bottom:
					return startPos + yMoveAmount * Vector3.down;
				case Direction.Left:
					return startPos + xMoveAmount * Vector3.left;
				case Direction.Nowhere:
					return startPos;
				default:
					return startPos;
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
				canvasTransform.position = Vector3.Lerp(startPos, endPos, t);
			} else
			{
				if (startsOverLevel)
				{	//Is now away from level, doesn't need to be active
					canvasTransform.gameObject.SetActive(false);
				}
				canMove = false;
			}
		}
	}
}
