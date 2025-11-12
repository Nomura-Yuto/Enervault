using UnityEngine;

public class NotesRing : MonoBehaviour
{
    public float radius = 125f; // 半径の初期値
    public float threshold = 0.2f; // 判定の閾値
    [SerializeField] Color ringColor = new Color(1, 0.5f, 0f, 0.5f); // 円の色
    public RectTransform rectTrans;
	public RectTransform parentTransform;

	public float GetOverRadius()
	{
		float scale = rectTrans.localScale.x * parentTransform.localScale.x;
		return (radius * scale) + (threshold * scale);
    }

    public float GetUnderRadius()
	{
		float scale = rectTrans.localScale.x * parentTransform.localScale.x;
		return Mathf.Max(0f, (radius * scale) - (threshold * scale));
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = ringColor;
        // 内側の円
        Gizmos.DrawWireSphere(transform.position, GetUnderRadius());
        // 外側の円
        Gizmos.DrawWireSphere(transform.position,GetOverRadius());

    }
}
