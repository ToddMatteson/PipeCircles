using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
	public class MenuClick : MonoBehaviour
	{
		[SerializeField] GameObject menuCanvas = null;
		[SerializeField] [Range(0.1f, 5f)] float totalMovementTime = 1f;

		Transform canvasTransform;
		Vector3 upPos;
		Vector3 downPos;
		float yMoveAmount = 1080f;
		GameObject[] buttons;
		float currentMovementTime;
		bool movingCanvas = false;
		bool isCanvasUp = true;
		bool canMove = false;
		//bool canClickLevelCanvas = true;
		
		private void Start()
		{
			if (menuCanvas == null)
			{
				Debug.Log("Please add a canvas to the button");
			}
			{
				canvasTransform = menuCanvas.transform;
				upPos = canvasTransform.position;
				downPos = upPos + yMoveAmount * Vector3.down;
				buttons = GameObject.FindGameObjectsWithTag("LevelCanvasClickable"); 
			}
		}

		public void Update()
		{
			if (canMove)
			{
				MoveCanvas(!isCanvasUp);
			}
		}

		public void OnMouseUp()
		{
			if (canvasTransform != null && !movingCanvas)
			{
				movingCanvas = true;
				canMove = true;
				if (isCanvasUp)
				{
					canvasTransform.gameObject.SetActive(true);
					MoveCanvas(false);
				} else
				{
					EnableLevelButtons(); //Moving up, so buttons need to be on first
					MoveCanvas(true);
				}		
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

		private void MoveCanvas(bool moveCanvasUp)
		{
			if (movingCanvas)
			{
				currentMovementTime += Time.deltaTime;

				if (currentMovementTime > totalMovementTime)
				{
					currentMovementTime = totalMovementTime;
					movingCanvas = false;
				}

				float t = currentMovementTime / totalMovementTime;
				if (moveCanvasUp)
				{
					canvasTransform.position = Vector3.Lerp(downPos, upPos, t);
				} else
				{
					//t = t * t * t * (t * (6f * t - 15f) + 10f); //Uncomment to make movement like water drops
					canvasTransform.position = Vector3.Lerp(upPos, downPos, t);
				}
			} else
			{
				currentMovementTime = 0;
				isCanvasUp = !isCanvasUp; //Movement finished, so the canvas is the opposite spot now 
				if (!isCanvasUp) 
				{
					DisableLevelButtons();
				}
				canMove = false;
			}
		}
	}
}
