using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

//Code in this script was inspired by a tutorial from Omnirift on Youtube at https://www.youtube.com/watch?v=fhBJWTO09Lw

namespace PipeCircles
{
	public class Draggable : MonoBehaviour
	{
		public const string DRAGGABLE_TAG = "Draggable";
		public const string REPLACEABLE_TAG = "Replaceable";

		private bool dragging = false;

		private Vector2 originalPos;

		private Transform objectToDrag;
		private Image objectToDragImage;

		List<RaycastResult> hitObjects = new List<RaycastResult>();

		[SerializeField] Transform upcomingTransform;

		void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				print("Got to GetMouseButtonDown");
				objectToDrag = GetDraggableTransformUnderMouse(true);
			
				if (objectToDrag != null)
				{
					dragging = true;

					objectToDrag.SetAsLastSibling();

					originalPos = objectToDrag.position;
					objectToDragImage = objectToDrag.GetComponent<Image>();
					objectToDragImage.raycastTarget = false;
				}
			}

			if (dragging)
			{
				objectToDrag.position = Input.mousePosition;
			}

			if (Input.GetMouseButtonUp(0))
			{
				if (objectToDrag != null)
				{
					Transform objectToReplace = GetDraggableTransformUnderMouse(false);

					if (objectToReplace != null)
					{
						objectToDrag.position = objectToReplace.position;
						objectToDrag.tag = REPLACEABLE_TAG;
						upcomingTransform.GetComponent<Upcoming>().PieceClicked();

						Destroy(objectToReplace.gameObject); //TODO Needs to be reworked to show reconstruction time
					} else
					{
						objectToDrag.position = originalPos;
					}

					objectToDragImage.raycastTarget = true;
					objectToDrag = null;
				}
				dragging = false;
			}
		}

		private Transform GetDraggableTransformUnderMouse(bool draggable)
		{
			bool objectDraggable = draggable;
			GameObject clickedObject = GetObjectUnderMouse();
			if (clickedObject == null)
			{
				print("null object");
			} else
			{
				print("clickedObject " + clickedObject.name);
			}
			

			if (draggable)
			{
				if (clickedObject != null && clickedObject.tag == DRAGGABLE_TAG)
				{
					return clickedObject.transform;
				}
			} else
			{
				if (clickedObject != null && clickedObject.tag == REPLACEABLE_TAG)
				{
					return clickedObject.transform;
				}
			}
			return null;
		}

		private GameObject GetObjectUnderMouse()
		{
			var pointer = new PointerEventData(EventSystem.current);
			pointer.position = Input.mousePosition;

			EventSystem.current.RaycastAll(pointer,hitObjects);
			if (hitObjects.Count <= 0) { return null; }
			return hitObjects.First().gameObject;
		}
	}
}
