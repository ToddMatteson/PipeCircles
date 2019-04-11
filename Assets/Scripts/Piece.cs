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

		[Header("Water Entering")]
		[SerializeField] bool canWaterEnterTop = false;
		[SerializeField] bool canWaterEnterRight = false;
		[SerializeField] bool canWaterEnterBottom = false;
		[SerializeField] bool canWaterEnterLeft = false;

		[Header("Primary Path")]
		[SerializeField] Direction topLeads = Direction.Nowhere;
		[SerializeField] Direction rightLeads = Direction.Nowhere;
		[SerializeField] Direction bottomLeads = Direction.Nowhere;
		[SerializeField] Direction leftLeads = Direction.Nowhere;

		[Header("Secondary Path")]
		[SerializeField] Direction secondTopLeads = Direction.Nowhere;
		[SerializeField] Direction secondRightLeads = Direction.Nowhere;
		[SerializeField] Direction secondBottomLeads = Direction.Nowhere;
		[SerializeField] Direction secondLeftLeads = Direction.Nowhere;

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

		public Direction GetTopLeads() { return topLeads; }
		public Direction GetRightLeads() { return rightLeads; }
		public Direction GetBottomLeads() { return bottomLeads; }
		public Direction GetLeftLeads() { return leftLeads; }

		public Direction FirstExitDirection(Direction entranceDirection) //Note this is done differently than Second direction due to complication of crosses
		{
			switch (entranceDirection)
			{
				case Direction.Top:
					return topLeads;
				case Direction.Right:
					return rightLeads;
				case Direction.Bottom:
					return bottomLeads;
				case Direction.Left:
					return leftLeads;
				case Direction.Nowhere:
					return Direction.Nowhere;
				default:
					return Direction.Nowhere;
			}
		}

		public bool HasSecondExitDirection()
		{
			return (SecondExitDirection() != Direction.Nowhere);
		}

		public Direction SecondExitDirection() //Note this is done differently than First direction because First has complications due to crosses
		{
			if (secondTopLeads != Direction.Nowhere) { return secondTopLeads; }
			if (secondRightLeads != Direction.Nowhere) { return secondRightLeads; }
			if (secondBottomLeads != Direction.Nowhere) { return secondBottomLeads; }
			if (secondLeftLeads != Direction.Nowhere) { return secondLeftLeads; }
			return Direction.Nowhere;
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
