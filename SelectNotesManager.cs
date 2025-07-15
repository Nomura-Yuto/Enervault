using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[Serializable]
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

    [Header("Effect & Audio")]
    [Tooltip("成功時に生成するエフェクト")]
    public GameObject excellentEffectPrefab;
    public Vector3 effectOffset = new Vector3(0, 0, 5);

    [Tooltip("成功時に鳴らすSE")]
    [SerializeField] AudioClip excellentSE;    // SE1
    [Tooltip("ミス時に鳴らすSE")]
    [SerializeField] AudioClip missSE;         // SE2
    [Tooltip("SE の音量")]
    [SerializeField, Range(0f, 1f)] float seVolume = 1f;

    void Start()
    {
        _circleTransform = circle.gameObject.transform;
        _circleTransform.localScale = new Vector3(initialScale, initialScale, initialScale);
        excellentImg.gameObject.SetActive(false);
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

        if ((circleOverRadius >= circleJustUnderRadius && circleOverRadius <= circleJustOverRadius) ||
            (circleUnderRadius >= circleJustUnderRadius && circleUnderRadius <= circleJustOverRadius))
        {
            ShowExcellent();
            isJust = true;
        }
        else
        {
            // ミス時の SE を再生
            if (missSE != null && Camera.main != null)
            {
                AudioSource.PlayClipAtPoint(
                    missSE,
                    Camera.main.transform.position,
                    seVolume
                );
            }
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
        if (!_isDisplayingExcellent) return;
        _excellentTimer -= Time.unscaledDeltaTime;
        if (_excellentTimer <= 0f)
        {
            excellentImg.gameObject.SetActive(false);
            justCircle.gameObject.SetActive(false);
            _isDisplayingExcellent = false;
        }
    }

    /// <summary>excellentImgを指定時間表示し、SE1を再生</summary>
    public void ShowExcellent()
    {
        var camera = Camera.main;
        if (camera != null)
        {
            // エフェクトをカメラ前に生成
            Instantiate(
                excellentEffectPrefab,
                camera.transform.position + (camera.transform.forward * 5) + effectOffset,
                Quaternion.identity,
                camera.transform
            );

            // 成功時の SE を再生
            if (excellentSE != null)
            {
                AudioSource.PlayClipAtPoint(
                    excellentSE,
                    camera.transform.position,
                    seVolume
                );
            }
        }

        // 画像表示の開始
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
        return _circleTransform.localScale.x <= 0f;
    }

    /// <summary>ノーツ開始時にUIを初期化して開始</summary>
    public void StartNotes()
    {
        _circleTransform.localScale = new Vector3(initialScale, initialScale, initialScale);
        _isDisplayingExcellent = false;
        excellentImg.gameObject.SetActive(false);

        circle.gameObject.SetActive(true);
        justCircle.gameObject.SetActive(true);
    }
}
