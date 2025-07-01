using UnityEngine;
using UnityEngine.UI;

public class CurvedGauge : MaskableGraphic
{
	[Header("ゲージの割合(0～1)")]
	[Range(0f, 1f)]
	[SerializeField] public float gaugeRate = 1;

	[Header("ゲージの設定")]
	[SerializeField] int segments = 100;
	[SerializeField] AnimationCurve innerCurve = AnimationCurve.EaseInOut(0, 0.2f, 1, 1f);
	[SerializeField] AnimationCurve outerCurve = AnimationCurve.EaseInOut(0, 0f, 1, 1f);

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();

		Rect r = rectTransform.rect;
		float height = r.height;
		float width = r.width;


		for (int i = 0; i < segments; i++)
		{
			float t0 = (float)i / segments;
			float t1 = (float)(i + 1) / segments;

			float x0 = t0 * width;
			float x1 = t1 * width;

			// カーブの値の取得	
			float innerVal0 = innerCurve.Evaluate(t0);
			float innerVal1 = innerCurve.Evaluate(t1);
			float outerVal0 = outerCurve.Evaluate(t0);
			float outerVal1 = outerCurve.Evaluate(t1);

			// 現在値を超えた場合描画終了
			if (innerVal0 >= gaugeRate)
			{
				break;
			}

			// 現在値よりも超えないように
			if (innerVal1 > gaugeRate)
			{
				innerVal1 = gaugeRate;
			}
			if (outerVal1 > gaugeRate)
			{
				outerVal1 = gaugeRate;
			}

			// 内側カーブのオフセット
			float innerOffset0 = (1f - t0) * 0.04f * height;
			float innerOffset1 = (1f - t1) * 0.04f * height;

			float innerY0 = innerVal0 * height + innerOffset0;
			float innerY1 = innerVal1 * height + innerOffset1;
			float outerY0 = outerVal0 * height;
			float outerY1 = outerVal1 * height;

			Vector2 innerPos0 = new Vector2(x0 * 0.5f, innerY0);
			Vector2 innerPos1 = new Vector2(x1 * 0.5f, innerY1);
			Vector2 outerPos0 = new Vector2(x0, outerY0);
			Vector2 outerPos1 = new Vector2(x1, outerY1);

			int idx = vh.currentVertCount;

			vh.AddVert(innerPos0, color, Vector2.zero);
			vh.AddVert(outerPos0, color, Vector2.zero);
			vh.AddVert(outerPos1, color, Vector2.zero);
			vh.AddVert(innerPos1, color, Vector2.zero);

			vh.AddTriangle(idx, idx + 1, idx + 2);
			vh.AddTriangle(idx, idx + 2, idx + 3);
		}
	}

}
