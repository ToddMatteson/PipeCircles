using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
	public class LevelSelectClickOpen : MonoBehaviour
	{
		//[SerializeField] Transform levelSelectTransform = null;
		GameObject coord;
		LevelSelectCoordinator coordinator;

		private void Start()
		{
			coord = GameObject.FindGameObjectWithTag("LevelSelectCoordinator");
			if (coord == null)
			{
				Debug.LogError("No object tagged LevelSelectCoordinator was found");
			} else
			{
				coordinator = coord.GetComponent<LevelSelectCoordinator>();
			}
			
			if (coordinator == null)
			{
				Debug.LogError("No Level Select Coordinator script found on the object with tag LevelSelectCoordinator");
			}
		}

		public void OnMouseUp()
		{
            //if (levelSelectTransform == null) { return; }
            Debug.Log("Within OnMouseUp");
            coordinator.LevelClickOpen();
		}
	}
}
