using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectNotesManager : MonoBehaviour
{
    [Tooltip("判定用の円画像")]
	[SerializeField] NotesRing circle;

    [Tooltip("ジャスト判定用の円画像")]
	[SerializeField] NotesRing justCircle;
	
    [Tooltip("成功時に表示される画像")]
	[SerializeField] Image excellentImg;

    [Tooltip("成功時に画像を表示する時間")]
	[SerializeField] float excellentDisplayTime = 1f;
    public float decreaseSpeed = 0.5f;

    [Tooltip("円の初期スケール")]
	[SerializeField] float initialScale = 2f;
	private Transform _circleTransform;
    private float _excellentTimer;
    private bool _isDisplayingExcellent;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        _circleTransform = circle.gameObject.transform;

		_circleTransform.localScale = new Vector3(initialScale, initialScale, initialScale);
        excellentImg.gameObject.SetActive(false);
        justCircle.gameObject.SetActive(false);
        circle.gameObject.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
        CircleUpdate();
        UpdateExcellentDisplay();
        // 終了判定時に円を非表示
        if (GetEnd())
        {
            circle.gameObject.SetActive(false);
            justCircle.gameObject.SetActive(false);
		}
    }

    /// <summary>タイミング判定(コライダーによる判定)</summary>
    public bool TimingJudg()
    {
		bool isJust = false;
        // 円の半径を取得
        float circleOverRadius = circle.GetOverRadius();
        float circleUnderRadius = circle.GetUnderRadius();

        // ジャスト判定の半径を取得
        float circleJustOverRadius = justCircle.GetOverRadius();
        float circleJustUnderRadius = justCircle.GetUnderRadius();

        if (circleOverRadius  >= circleJustUnderRadius
            && circleOverRadius <= circleJustOverRadius||
            circleUnderRadius >= circleJustUnderRadius
            && circleUnderRadius <= circleJustOverRadius)
        {
            ShowExcellent();
            isJust = true;
        }

		circle.gameObject.SetActive(false);
		justCircle.gameObject.SetActive(false);

		return isJust;
    }

    /// <summary>円を縮小更新</summary>
    void CircleUpdate()
    {
        Vector3 newScale;
        float value = decreaseSpeed * Time.unscaledDeltaTime;
        newScale = _circleTransform.localScale - new Vector3(value, value, value);
        _circleTransform.localScale = newScale;
    }

    private void UpdateExcellentDisplay()
    {
        if (!_isDisplayingExcellent) return;
        _excellentTimer -= Time.unscaledDeltaTime;
        if (_excellentTimer <= 0f)
        {
            excellentImg.gameObject.SetActive(false);
			justCircle.gameObject.SetActive(false);
            _isDisplayingExcellent = false;
        }
    }

    /// <summary>excellentImgを指定時間表示</summary>
    public void ShowExcellent()
    {
        excellentImg.gameObject.SetActive(true);
        _isDisplayingExcellent = true;
        _excellentTimer = excellentDisplayTime;
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
        if (_circleTransform.localScale.x <= 0f)
        {
            return true;
        }
        return false;
    }

    /// <summary>ノーツ開始時にUIを初期化して開始</summary>
    public void StartNotes()
    {
        // 円のスケールを初期値に戻す
        _circleTransform.localScale = new Vector3(initialScale, initialScale, initialScale);
        // excellent表示関連をリセット
        _isDisplayingExcellent = false;
        excellentImg.gameObject.SetActive(false);

        circle.gameObject.SetActive(true);
        justCircle.gameObject.SetActive(true);
    }
}
