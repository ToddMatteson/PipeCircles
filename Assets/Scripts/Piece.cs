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

		public Direction GetTopGoesWhere() { return TopGoesWhere; }
		public Direction GetRightGoesWhere() { return RightGoesWhere; }
		public Direction GetBottomGoesWhere() { return BottomGoesWhere; }
		public Direction GetLeftGoesWhere() { return LeftGoesWhere; }
	}

	public enum Direction { Nowhere, Top, Right, Bottom, Left }
}
