using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>タイミング評価</summary>
public enum NotesJudge
{
	Excellent,
	Good,
	Miiss,
	Count
}

[Serializable]
public class SelectNotesManager : MonoBehaviour
{
    [Tooltip("判定用の円画像")]
    [SerializeField] NotesRing circle;

    [Tooltip("ジャスト判定用の円画像")]
    [SerializeField] NotesRing justCircle;

    [Tooltip("グッド判定用の円")]
    [SerializeField] NotesGoodRing goodRing;

    [Tooltip("タイミング判定の画像")]
	[SerializeField] Image judgeImg;
    [SerializeField] Sprite excellentImg;
    [SerializeField] Sprite goodImg;
    [SerializeField] Sprite missImg;
	
    [Tooltip("成功時に画像を表示する時間")]
    [SerializeField] float judgeDisplayTime = 1f;
    public float decreaseSpeed = 0.5f;

    [Tooltip("円の初期スケール")]
    [SerializeField] float initialScale = 2f;
    private Transform _circleTransform;
    private float _judgeTimer;
    private bool _isDisplayingJudge;

    [Header("Effect & Audio")]
    [Tooltip("成功時に生成するエフェクト")]
    public GameObject excellentEffectPrefab;
    public Vector3 effectOffset = new Vector3(0, 0, 5);

    [Tooltip("成功時に鳴らすSE")]
    [SerializeField] AudioClip excellentSE;		// SE1
	[Tooltip("グッド時に鳴らすSE")]
	[SerializeField] AudioClip goodSE;			// SE2
	[Tooltip("ミス時に鳴らすSE")]
    [SerializeField] AudioClip missSE;			// SE3
    [Tooltip("SE の音量")]
    [SerializeField, Range(0f, 1f)] float seVolume = 1f;

    void Start()
    {
        _circleTransform = circle.gameObject.transform;
        _circleTransform.localScale = new Vector3(initialScale, initialScale, initialScale);
        judgeImg.gameObject.SetActive(false);
        justCircle.gameObject.SetActive(false);
        circle.gameObject.SetActive(false);
    }

    void Update()
    {
        CircleUpdate();
        UpdateExcellentDisplay();

        if (GetEnd())
        {
            circle.gameObject.SetActive(false);
            justCircle.gameObject.SetActive(false);
        }
    }

    /// <summary>タイミング判定(半径による判定)</summary>
    public bool TimingJudg()
    {
        bool isJust = false;
		NotesJudge judge;

		// 円の半径を取得
		float circleOverRadius = circle.GetOverRadius();
        float circleUnderRadius = circle.GetUnderRadius();

        // ジャスト判定の半径を取得
        float circleJustOverRadius = justCircle.GetOverRadius();
        float circleJustUnderRadius = justCircle.GetUnderRadius();

		// グッド判定の半径を取得
		float circleGoodOverRadius = goodRing.GetOverRadius();
		float circleGoodUnderRadius = goodRing.GetUnderRadius();

		if ((circleOverRadius >= circleJustUnderRadius && circleOverRadius <= circleJustOverRadius) ||
            (circleUnderRadius >= circleJustUnderRadius && circleUnderRadius <= circleJustOverRadius))
        {
			ShowJudge(excellentImg, excellentSE, true, excellentEffectPrefab);
			judge = NotesJudge.Excellent;
			isJust = true;
        }
		else if((circleOverRadius >= circleGoodUnderRadius && circleOverRadius <= circleGoodOverRadius) ||
				(circleUnderRadius >= circleGoodUnderRadius && circleUnderRadius <= circleGoodOverRadius))
		{
			ShowJudge(goodImg, goodSE);
			judge = NotesJudge.Good;
		}
        else
		{
			ShowJudge(missImg, missSE);
			judge = NotesJudge.Miiss;
		}

        // 判定後は円を非表示
        circle.gameObject.SetActive(false);
        justCircle.gameObject.SetActive(false);

        return isJust;
    }

    /// <summary>円を縮小更新</summary>
    void CircleUpdate()
    {
        if (_circleTransform.localScale.x < 0f) return;
        float value = decreaseSpeed * Time.unscaledDeltaTime;
        _circleTransform.localScale -= new Vector3(value, value, value);
    }

    private void UpdateExcellentDisplay()
    {
        if (!_isDisplayingJudge) return;
        _judgeTimer -= Time.unscaledDeltaTime;
        if (_judgeTimer <= 0f)
        {
			judgeImg.gameObject.SetActive(false);
            justCircle.gameObject.SetActive(false);
            _isDisplayingJudge = false;
        }
    }

	/// <summary>判定の画像を指定時間表示</summary>
	public void ShowJudge(Sprite sprite, AudioClip se = null, bool useEffect = false, GameObject effectObj = null)
	{
		var camera = Camera.main;
		if (camera != null)
		{
			if (useEffect)
			{
				// エフェクトをカメラ前に生成
				Instantiate(
					effectObj,
					camera.transform.position + (camera.transform.forward * 5) + effectOffset,
					Quaternion.identity,
					camera.transform
				);
			}

			// SE を再生
			if (se != null)
			{
				AudioSource.PlayClipAtPoint(
					se,
					camera.transform.position,
					seVolume
				);
			}
		}

		// 画像の切り替え
		judgeImg.sprite = sprite;

		// 画像表示の開始
		judgeImg.gameObject.SetActive(true);
		_isDisplayingJudge = true;
		_judgeTimer = judgeDisplayTime;
	}

	/// <summary>縮小速度を設定</summary>
	public void SetDecreaseSpeed(float speed = 0.5f)
    {
        decreaseSpeed = speed;
    }

    /// <summary>初期スケールを設定</summary>
    public void SetInitialScale(float scale = 2f)
    {
        initialScale = scale;
    }

    /// <summary>円が消失したらtrue</summary>
    public bool GetEnd()
    {
        return _circleTransform.localScale.x <= 0f;
    }

    /// <summary>ノーツ開始時にUIを初期化して開始</summary>
    public void StartNotes()
    {
        _circleTransform.localScale = new Vector3(initialScale, initialScale, initialScale);
        _isDisplayingJudge = false;
		judgeImg.gameObject.SetActive(false);

        circle.gameObject.SetActive(true);
        justCircle.gameObject.SetActive(true);
    }
}
