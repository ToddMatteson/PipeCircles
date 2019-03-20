using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
	//What other properties are common to all pieces?
	
	public enum Direction { Nowhere, Top, Right, Bottom, Left }

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

	//Location in world space, X and Y - how to get this?
	float worldXPos;
	float worldYPos;
	Vector2 worldPos;

	//Location in board grid, X and Y - how to get this?
	int boardXPos;
	int boardYPos;
	Vector2Int boardPos;


	void Start()
    {
        
    }

    void Update()
    {
        
    }
}
