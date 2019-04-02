using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
	public class Piece : MonoBehaviour
	{
		Vector2 worldPos;
		Vector2Int boardPos;
		int numWaterPasses = 0;

		[Header("Water Entering")]
		[SerializeField] bool canWaterEnterTop = false;
		[SerializeField] bool canWaterEnterRight = false;
		[SerializeField] bool canWaterEnterBottom = false;
		[SerializeField] bool canWaterEnterLeft = false;

		[Header("Paths Available")]
		[SerializeField] Direction topGoesWhere = Direction.Nowhere;
		[SerializeField] Direction rightGoesWhere = Direction.Nowhere;
		[SerializeField] Direction bottomGoesWhere = Direction.Nowhere;
		[SerializeField] Direction leftGoesWhere = Direction.Nowhere;

		[Header("Cross Sprites")]
		[SerializeField] Sprite leftOverVertical;
		[SerializeField] Sprite leftOverHorizontal;
		[SerializeField] Sprite leftOverFull;
		[SerializeField] Sprite topOverVertical;
		[SerializeField] Sprite topOverHorizontal;
		[SerializeField] Sprite topOverFull;

		Board board;
		Scoring scoring;

		void Start()
		{
			board = FindObjectOfType<Board>().GetComponent<Board>();
			scoring = FindObjectOfType<Scoring>().GetComponent<Scoring>();
		}

		public bool CanWaterEnter(Direction dir)
		{
			switch (dir)
			{
				case Direction.Top:
					return canWaterEnterTop;
				case Direction.Right:
					return canWaterEnterRight;
				case Direction.Bottom:
					return canWaterEnterBottom;
				case Direction.Left:
					return canWaterEnterLeft;
				case Direction.Nowhere:
					return false;
				default:
					return false;
			}
		}

		public Direction GetTopGoesWhere() { return topGoesWhere; }
		public Direction GetRightGoesWhere() { return rightGoesWhere; }
		public Direction GetBottomGoesWhere() { return bottomGoesWhere; }
		public Direction GetLeftGoesWhere() { return leftGoesWhere; }

		public Direction WhereWaterExits(Direction entranceDirection)
		{
			switch (entranceDirection)
			{
				case Direction.Top:
					return topGoesWhere;
				case Direction.Right:
					return rightGoesWhere;
				case Direction.Bottom:
					return bottomGoesWhere;
				case Direction.Left:
					return leftGoesWhere;
				case Direction.Nowhere:
					return Direction.Nowhere;
				default:
					return Direction.Nowhere;
			}
		}

		public void AnimationComplete() //Called by animations
		{
			board.AnimationComplete();
		}

		public void LoopCompleted() //Called by animations
		{
			scoring.LoopCompleted();
		}
	}

	public enum Direction { Nowhere, Top, Right, Bottom, Left }
}
