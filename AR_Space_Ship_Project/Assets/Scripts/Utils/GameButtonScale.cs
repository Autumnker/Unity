using UnityEngine;
using UnityEngine.EventSystems;

public class GameButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public float scalValue = 0.9f;
	public Vector3 localScal;

	void Awake()
	{
		localScal = transform.localScale;
	}

    public void OnPointerDown(PointerEventData eventData)
    {
        iTween.ScaleTo(gameObject, iTween.Hash("scale", localScal * scalValue, "islocal", true, "easetype", iTween.EaseType.linear, "time", 0.1f));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        iTween.ScaleTo(gameObject, iTween.Hash("scale", localScal * 1.1f, "islocal", true, "easetype", iTween.EaseType.linear, "time", 0.08f, "oncomplete", "ScaleOver", "oncompletetarget", gameObject));
    }

	private void ScaleOver()
	{
		iTween.ScaleTo (gameObject, iTween.Hash ("scale" ,localScal, "islocal",true, "easetype", iTween.EaseType.linear, "time", 0.06f));
	}
}

