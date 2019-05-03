using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
	public class Piece : MonoBehaviour
	{
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

		[Header("Splitter Secondary Path")]
		[SerializeField] Direction secondTopLeads = Direction.Nowhere;
		[SerializeField] Direction secondRightLeads = Direction.Nowhere;
		[SerializeField] Direction secondBottomLeads = Direction.Nowhere;
		[SerializeField] Direction secondLeftLeads = Direction.Nowhere;

		[Header("Water Park")]
		[SerializeField] Direction park1stEntrance = Direction.Nowhere;
		[SerializeField] [Range(1, 3)] int parkEntrance1Slot = 0;
		[SerializeField] Direction park2ndEntrance = Direction.Nowhere;
		[SerializeField] [Range(1, 3)] int parkEntrance2Slot = 0;
		[SerializeField] Direction parkExit = Direction.Nowhere;
		[SerializeField] [Range(1, 3)] int parkExitSlot = 0;

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

		public void LoopCompleted() //Called by animations
		{
			scoring.LoopCompleted();
		}

		public void EndReached() //Called by animations
		{
			scoring.EndReached();
		}

		public void AnimationComplete(string startingDirection) //Called by animations
		{
			Direction startDirection = GetStartingDirection(startingDirection);
			board.AnimationComplete(gameObject.transform, startDirection);
		}

		private Direction GetStartingDirection(string startingDirection)
		{
			switch (startingDirection)
			{
				case "Top":
					return Direction.Top;
				case "Right":
					return Direction.Right;
				case "Bottom":
					return Direction.Bottom;
				case "Left":
					return Direction.Left;
				case "Nowhere":
					Debug.LogError("GetStartingDirection received a Nowhere argument.");
					return Direction.Nowhere;
				default:
					Debug.LogError("GetStartingDirection had to use the default.");
					return Direction.Nowhere;
			}
		}

		public Direction GetPark1stEntrance()
		{
			return park1stEntrance;
		}

		public Direction GetPark2ndEntrance()
		{
			return park2ndEntrance;
		}

		public int GetParkEntrance1Slot()
		{
			return parkEntrance1Slot;
		}

		public int GetParkEntrance2Slot()
		{
			return parkEntrance2Slot;
		}

		public Direction GetParkExit()
		{
			return parkExit;
		}

		public int GetParkExitSlot()
		{
			return parkExitSlot;
		}
	}

	public enum Direction { Nowhere, Top, Right, Bottom, Left }
}
