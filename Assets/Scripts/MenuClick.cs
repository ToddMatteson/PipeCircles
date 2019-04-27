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
		float currentMovementTime;
		bool moveCanvas = false;
		bool isCanvasUp = true;
		bool canMove = false;
		float yMoveAmount = 1080f;
		Vector3 upPos;
		Vector3 downPos;

		private void Awake()
		{
			if (menuCanvas == null)
			{
				Debug.Log("Please add a canvas to the button");
			}
			{
				canvasTransform = menuCanvas.transform;
				upPos = canvasTransform.position;
				downPos = upPos + yMoveAmount * Vector3.down;
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
			if (canvasTransform != null && !moveCanvas)
			{
				moveCanvas = true;
				canMove = true;
				if (isCanvasUp)
				{
					canvasTransform.gameObject.SetActive(true);	
					MoveCanvas(false);
				} else
				{
					MoveCanvas(true);
				}		
			}
		}

		private void MoveCanvas(bool moveCanvasUp)
		{
			if (moveCanvas)
			{
				currentMovementTime += Time.deltaTime;

				if (currentMovementTime > totalMovementTime)
				{
					currentMovementTime = totalMovementTime;
					moveCanvas = false;
				}

				float t = currentMovementTime / totalMovementTime;
				if (moveCanvasUp) //Move up should move at constant time, move down should move at smoothed time
				{
					canvasTransform.position = Vector3.Lerp(downPos, upPos, t);
				} else
				{
					t = t * t * t * (t * (6f * t - 15f) + 10f);
					canvasTransform.position = Vector3.Lerp(upPos, downPos, t);
				}
			} else
			{
				currentMovementTime = 0;
				isCanvasUp = !isCanvasUp;
				canMove = false;
			}
		}
	}
}
