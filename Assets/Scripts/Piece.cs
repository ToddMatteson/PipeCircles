using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
	//What other properties are common to all pieces?
	
	public enum Direction { Nowhere, Top, Right, Bottom, Left }

	//TODO get these directly from the board itself
	const int BOARD_UNITS_WIDE = 12;
	const int BOARD_UNITS_HIGH = 9;

	[Header ("Water Entering")]
	[SerializeField] bool canWaterEnterTop = false;
	[SerializeField] bool canWaterEnterRight = false;
	[SerializeField] bool canWaterEnterBottom = false;
	[SerializeField] bool canWaterEnterLeft = false;
	
	[Header ("Water Exiting")]
	[SerializeField] bool canWaterExitTop = false;
	[SerializeField] bool canWaterExitRight = false;
	[SerializeField] bool canWaterExitBottom = false;
	[SerializeField] bool canWaterExitLeft = false;

	[Header("Paths Available")]
	[SerializeField] Direction TopGoesWhere = Direction.Nowhere;
	[SerializeField] Direction RightGoesWhere = Direction.Nowhere;
	[SerializeField] Direction BottomGoesWhere = Direction.Nowhere;
	[SerializeField] Direction LeftGoesWhere = Direction.Nowhere;

	Vector2 worldPos;
	Vector2Int boardPos;

	bool pieceOnBoard = false;

    void Update()
    {
		UpdateWorldAndBoardPos();
		CheckIfPiecePlacedOnBoard();
    }

	private void UpdateWorldAndBoardPos()
	{
		worldPos = gameObject.transform.position;
		TranslateWorldPosToBoardPos();
	}

	private void TranslateWorldPosToBoardPos()
	{
		//TODO Temporary fix, minimum needed for functionality and completely wrong
		//Wait until the scaling issues are done being sorted out???
		boardPos.x = Mathf.RoundToInt(worldPos.x);
		boardPos.y = Mathf.RoundToInt(worldPos.y);
	}

	private void CheckIfPiecePlacedOnBoard()
	{
		bool pieceWasOnBoard = pieceOnBoard;
		bool pieceNowOnBoard = true;

		if (boardPos.x < 0 || boardPos.x >= BOARD_UNITS_WIDE)
		{
			pieceWasOnBoard = false;
		}

		if (boardPos.y < 0 || boardPos.y >= BOARD_UNITS_HIGH)
		{
			pieceWasOnBoard = false;
		}
	}
}
