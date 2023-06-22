using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
/// <summary>
/// 遷移コントローラー.
/// </summary>
//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    
public sealed class TransitionController : MonoBehaviour
{
    //================================================================================
    // Definitions.
    //================================================================================

    /// <summary>
    /// 遷移の種類.
    /// </summary>
    private enum TransitionType : int
    {
        /// <summary>
        /// フェードイン.
        /// </summary>
        In
        /// <summary>
        /// フェードアウト.
        /// </summary>
      , Out
    }

    //================================================================================
    // Fields.
    //================================================================================

    /// <summary>
    /// 遷移の種類.
    /// </summary>
    [SerializeField] private TransitionType Type = default;

    /// <summary>
    /// 遷移する秒数.
    /// </summary>
    [SerializeField] private float Duration = 3.0f;

    /// <summary>
    /// フェード画像.
    /// </summary>
    [SerializeField] private RawImage FadeImage = default;

    /// <summary>
    /// 遷移するシーン名.
    /// </summary>
    /// <remarks>
    /// フェードアウトのみ使用.
    /// </remarks>
    [SerializeField] private string TransitionSceneName = default;

    /// <summary>
    /// 遷移が終わると呼ばれる.
    /// </summary>
    /// <remarks>
    /// フェードイン時のみ呼ばれる.
    /// </remarks>
    [SerializeField] private UnityEvent OnCompleted = default;

    /// <summary>
    /// 自動で実行するか？
    /// </summary>
    [SerializeField] private bool PlayOnAwake = default;

    /// <summary>
    /// 開始時のアルファ.
    /// </summary>
    private float From = default;

    /// <summary>
    /// 終了時のアルファ.
    /// </summary>
    private float To = default;

    /// <summary>
    /// 一定時間処理し続ける際に使用.
    /// </summary>
    private float T = default;

    /// <summary>
    /// 実行中か？
    /// </summary>
    private bool IsPlaying = default;

    //================================================================================
    // Methods.
    //================================================================================
    
    //--------------------------------------------------------------------------------
    // MonoBehaviour methods.
    //--------------------------------------------------------------------------------

    private void Start()
    {
        if (PlayOnAwake)
        {
            Do();
        }
    }

    private void Update()
    {
        if (!IsPlaying)
        {
            return;
        }
        
        T = Mathf.Clamp01(T + Time.deltaTime / Duration);
        var color = FadeImage.color;
        color.a         = Mathf.Lerp(From, To, T);
        FadeImage.color = color;

        if (T >= 1.0f)
        {
            color.a         = To;
            FadeImage.color = color;
            IsPlaying       = false;

            OnCompleted?.Invoke();

            if      (Type == TransitionType.In)
            {
                FadeImage.gameObject.SetActive(false);
            }
            else if (!string.IsNullOrEmpty(TransitionSceneName))
            {
                SceneManager.LoadScene(TransitionSceneName);
            }
        }
    }
    
    //--------------------------------------------------------------------------------
    // Public methods.
    //--------------------------------------------------------------------------------

    /// <summary>
    /// 実行する.
    /// </summary>
    public void Do()
    {
        FadeImage.gameObject.SetActive(true);
        From      = Type == TransitionType.In ? 1.0f : 0.0f;
        To        = Type == TransitionType.In ? 0.0f : 1.0f;
        T         = 0.0f;
        IsPlaying = true;
    }
}
