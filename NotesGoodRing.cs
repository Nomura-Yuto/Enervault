using UnityEngine;

public class NotesGoodRing : MonoBehaviour
{
	public NotesRing baseRing;
	public float threshold = 0.2f; // îªíËÇÃËáíl
	[SerializeField] Color ringColor = new Color(1, 0.5f, 0f, 0.5f); // â~ÇÃêF
	public RectTransform rectTrans;
	public RectTransform parentTransform;

	public float GetOverRadius()
	{
		float radius = baseRing.GetOverRadius();
		float scale = rectTrans.localScale.x * parentTransform.localScale.x;
		return radius + threshold * scale;
	}

	public float GetUnderRadius()
	{
		float radius = baseRing.GetUnderRadius();
		float scale = rectTrans.localScale.x * parentTransform.localScale.x;
		return Mathf.Max(0f, radius - threshold * scale);
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = ringColor;
		// ì‡ë§ÇÃâ~
		Gizmos.DrawWireSphere(transform.position, GetUnderRadius());
		// äOë§ÇÃâ~
		Gizmos.DrawWireSphere(transform.position, GetOverRadius());
	}
}
