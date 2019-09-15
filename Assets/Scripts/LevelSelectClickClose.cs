using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PipeCircles
{
	public class LevelSelectClickClose : MonoBehaviour
	{
		//[SerializeField] Transform menuTransform = null;
		GameObject coord;
		MenuCoordinator coordinator;

		private void Start()
		{
			coord = GameObject.FindGameObjectWithTag("MenuCoordinator");
			if (coord == null)
			{
				Debug.LogError("No object tagged MenuCoordinator was found");
			} else
			{
				coordinator = coord.GetComponent<MenuCoordinator>();
			}
			
			if (coordinator == null)
			{
				Debug.LogError("No Menu Coordinator script found on the object with tag MenuCoordinator");
			}
		}

		public void OnMouseUp()
		{
			//if (menuTransform == null) { return; }
			coordinator.LevelClickClose();
		}
	}
}
