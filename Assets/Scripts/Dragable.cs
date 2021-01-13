using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Dragable : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler {

	RectTransform rectTransform;
	CanvasGroup canvasGroup;

	void Awake() {
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void OnBeginDrag(PointerEventData eventData) {
		
	}

	public void OnDrag(PointerEventData eventData) {
		rectTransform.anchoredPosition += eventData.delta;
	}	

	public void OnEndDrag(PointerEventData eventData) {
		//canvasGroup.alpha = 1f;
		canvasGroup.blocksRaycasts = true;
		rectTransform.localScale = new Vector3(1, 1, 1);
	}

	public void OnPointerDown(PointerEventData eventData) {
		//canvasGroup.alpha = 0.6f;
		canvasGroup.blocksRaycasts = false;
		rectTransform.localScale = new Vector3(2, 2, 2);
	}

	public void OnDrop(PointerEventData eventData) {

	}
}
