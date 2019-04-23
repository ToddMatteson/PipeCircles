using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
	public class Trajectory : MonoBehaviour
	{
		private const int PIXELS_PER_BOARD_UNIT = 100;

		public BezierStartingPos BezierCubicMiddlePos(Vector2Int startingPos, Vector2Int endingPos, int i)
		{
			bool usePositiveResult = i % 2 == 0;
			Vector3 startingWorldPos = BoardPosToWorldPos(startingPos);
			Vector3 endingWorldPos = BoardPosToWorldPos(endingPos);
			float deltaXSquared = (endingWorldPos.x - startingWorldPos.x) * (endingWorldPos.x - startingWorldPos.x);
			float deltaYSquared = (endingWorldPos.y - startingWorldPos.y) * (endingWorldPos.y - startingWorldPos.y);
			float distBetweenPoints = Mathf.Sqrt(deltaXSquared + deltaYSquared);
			float slope;
			float orthogonalSlope;

			float distToUse = distBetweenPoints * 0.4f; //Adjustable here to get curves to look right

			Vector3 result1Pos;
			Vector3 result1Neg;
			Vector3 result2Pos;
			Vector3 result2Neg;

			if ((endingWorldPos.x - startingWorldPos.x != 0) && (endingWorldPos.y - startingWorldPos.y != 0))
			{
				slope = (endingWorldPos.y - startingWorldPos.y) / (endingWorldPos.x - startingWorldPos.x);
				orthogonalSlope = -1 / slope;

				//From the GeeksForGeeks website
				//x1 = x0 +- dist * sqrt[1 / (1 + m * m)]
				//y1 = y0 +- dist * m * sqrt[1 / (1 + m * m)]
				float sqrtPortion = Mathf.Sqrt(1 / (1 + orthogonalSlope * orthogonalSlope));
				float x1Pos = startingWorldPos.x + distToUse * sqrtPortion;
				float x1Neg = startingWorldPos.x - distToUse * sqrtPortion;
				float y1Pos = startingWorldPos.y + distToUse * orthogonalSlope * sqrtPortion;
				float y1Neg = startingWorldPos.y - distToUse * orthogonalSlope * sqrtPortion;

				float x2Pos = endingWorldPos.x + distToUse * sqrtPortion;
				float x2Neg = endingWorldPos.x - distToUse * sqrtPortion;
				float y2Pos = endingWorldPos.y + distToUse * orthogonalSlope * sqrtPortion;
				float y2Neg = endingWorldPos.y - distToUse * orthogonalSlope * sqrtPortion;

				result1Pos = new Vector3(x1Pos, y1Pos, 0);
				result1Neg = new Vector3(x1Neg, y1Neg, 0);
				result2Pos = new Vector3(x2Pos, y2Pos, 0);
				result2Neg = new Vector3(x2Neg, y2Neg, 0);
			} else if (endingWorldPos.x - startingWorldPos.x == 0)
			{   //Horizontal
				result1Pos = new Vector3(startingWorldPos.x, startingWorldPos.y + distToUse, 0);
				result1Neg = new Vector3(startingWorldPos.x, startingWorldPos.y - distToUse, 0);
				result2Pos = new Vector3(endingWorldPos.x, startingWorldPos.y + distToUse, 0);
				result2Neg = new Vector3(endingWorldPos.x, startingWorldPos.y - distToUse, 0);
			} else
			{   //Vertical
				result1Pos = new Vector3(startingWorldPos.x + distToUse, startingWorldPos.y, 0);
				result1Neg = new Vector3(startingWorldPos.x - distToUse, startingWorldPos.y, 0);
				result2Pos = new Vector3(endingWorldPos.x + distToUse, startingWorldPos.y, 0);
				result2Neg = new Vector3(endingWorldPos.x - distToUse, startingWorldPos.y, 0);
			}

			if (usePositiveResult)
			{
				return new BezierStartingPos(result1Pos, result2Pos);
			} else
			{
				return new BezierStartingPos(result1Neg, result2Neg);
			}
		}

		public Vector3 CalcBezierCubicPos(Vector2Int startingPos, Vector2Int endingPos, Vector3 middle1, Vector3 middle2, float time01)
		{
			Vector3 startingWorldPos = BoardPosToWorldPos(startingPos);
			Vector3 endingWorldPos = BoardPosToWorldPos(endingPos);

			Vector3 round1a = Vector3.Lerp(startingWorldPos, middle1, time01);
			Vector3 round1b = Vector3.Lerp(middle1, middle2, time01);
			Vector3 round1c = Vector3.Lerp(middle2, endingWorldPos, time01);
			Vector3 round2a = Vector3.Lerp(round1a, round1b, time01);
			Vector3 round2b = Vector3.Lerp(round1b, round1c, time01);
			Vector3 round3 = Vector3.Lerp(round2a, round2b, time01);

			return round3;
		}

		private Vector3 BoardPosToWorldPos(Vector2Int boardPos)
		{
			float x = (float) boardPos.x * Board.PIXELS_PER_BOARD_UNIT;
			float y = (float) boardPos.y * Board.PIXELS_PER_BOARD_UNIT;
			float z = 0;
			return new Vector3(x, y, z);
		}
	}

	public struct BezierStartingPos
	{
		public Vector2 middlePos1;
		public Vector2 middlePos2;

		public BezierStartingPos(Vector2 middle1, Vector2 middle2)
		{
			middlePos1 = middle1;
			middlePos2 = middle2;
		}
	}
}
