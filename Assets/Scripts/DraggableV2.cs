using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

namespace PipeCircles
{
	public class DraggableV2 : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
	{
		Transform parentToReturnTo = null;

		public void OnBeginDrag(PointerEventData eventData)
		{
			Debug.Log("OnBeginDrag");
			/*
			parentToReturnTo = this.transform.parent;
			this.transform.SetParent(this.transform.parent.parent);

			GetComponent<CanvasGroup>().blocksRaycasts = false;
			*/
		}

		public void OnDrag(PointerEventData eventData)
		{
			Debug.Log("On Drag has started");
			this.transform.position = eventData.position;
		}

		public void OnClick()
		{
			Debug.Log("Clicked");
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			Debug.Log("OnEndDrag");
			this.transform.SetParent(parentToReturnTo);
			GetComponent<CanvasGroup>().blocksRaycasts = true;
		}

		public void StartDrag()
		{
			Debug.Log("Reached StartDrag in DraggableV2");
		}

		public void EndDrag()
		{
			Debug.Log("Reached EndDrag in DraggableV2");
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			Debug.Log("Reached OnPointerDown");
		}
	}
}
