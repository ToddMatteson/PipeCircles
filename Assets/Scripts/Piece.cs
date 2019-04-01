using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
	public class Piece : MonoBehaviour
	{
		bool hasWater = false;
		bool pieceOnBoard = false;
		Vector2 worldPos;
		Vector2Int boardPos;
		int numWaterPasses = 0;

		[Header("Water Entering")]
		[SerializeField] bool canWaterEnterTop = false;
		[SerializeField] bool canWaterEnterRight = false;
		[SerializeField] bool canWaterEnterBottom = false;
		[SerializeField] bool canWaterEnterLeft = false;

		[Header("Water Exiting")]
		[SerializeField] bool canWaterExitTop = false;
		[SerializeField] bool canWaterExitRight = false;
		[SerializeField] bool canWaterExitBottom = false;
		[SerializeField] bool canWaterExitLeft = false;

		[Header("Paths Available")]
		[SerializeField] Direction TopGoesWhere = Direction.Nowhere;
		[SerializeField] Direction RightGoesWhere = Direction.Nowhere;
		[SerializeField] Direction BottomGoesWhere = Direction.Nowhere;
		[SerializeField] Direction LeftGoesWhere = Direction.Nowhere;

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

		public Direction GetTopGoesWhere() { return TopGoesWhere; }
		public Direction GetRightGoesWhere() { return RightGoesWhere; }
		public Direction GetBottomGoesWhere() { return BottomGoesWhere; }
		public Direction GetLeftGoesWhere() { return LeftGoesWhere; }

		public Direction WhereWaterExits(Direction entranceDirection)
		{
			switch (entranceDirection)
			{
				case Direction.Top:
					return TopGoesWhere;
				case Direction.Right:
					return RightGoesWhere;
				case Direction.Bottom:
					return BottomGoesWhere;
				case Direction.Left:
					return LeftGoesWhere;
				case Direction.Nowhere:
					return Direction.Nowhere;
				default:
					return Direction.Nowhere;
			}
		}
		
		public int GetNumWaterPasses()
		{
			return numWaterPasses;
		}

		public void IncrementNumWaterPasses()
		{
			numWaterPasses++;
		}



	}

	public enum Direction { Nowhere, Top, Right, Bottom, Left }
}
