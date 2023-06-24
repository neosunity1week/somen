using UnityEngine;
using UnityEngine.Playables;


//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
/// <summary>
/// 最初のアニメーション再生クラス.
/// </summary>
//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    
public sealed class Intro : MonoBehaviour
{
    //================================================================================
    // Definitions.
    //================================================================================

    /// <summary>
    /// ステート.
    /// </summary>
    private enum State : int
    {
        None
      , Idle
      , Fade
      , Intro
      , Float
    }

    //================================================================================
    // Fields.
    //================================================================================

    /// <summary>
    /// プレイヤー.
    /// </summary>
    [SerializeField] private PlayerController Player = default;

    /// <summary>
    /// タイトル画面BGMのソース.
    /// </summary>
    [SerializeField] private AudioSource AudioSource = default;

    /// <summary>
    /// タイトル画面BGMのフェードアウトの秒数.
    /// </summary>
    [SerializeField] private float FadeSeconds = 1.5f;

    /// <summary>
    /// タイムライン再生コンポーネント.
    /// </summary>
    [SerializeField] private PlayableDirector PlayableDirector = default;

    /// <summary>
    /// プレイヤーのアニメーションコントローラー.
    /// </summary>
    [SerializeField] private RuntimeAnimatorController PlayerAnimationController = default;

    /// <summary>
    /// ゲーム中のプレイヤーの位置.
    /// </summary>
    [SerializeField] private Transform Locator = default;

    /// <summary>
    /// 上に移動する秒数.
    /// </summary>
    [SerializeField] private float FloatSeconds = 1.5f;

    /// <summary>
    /// 上に移動するカーブ.
    /// </summary>
    [SerializeField] private AnimationCurve FloatCurve = default;

    /// <summary>
    /// 雲のオブジェクト.
    /// </summary>
    [SerializeField] private GameObject Clouds = default;

    /// <summary>
    /// 現在のステート.
    /// </summary>
    private State CurrentState = default;

    /// <summary>
    /// 一定時間処理し続ける際に使用.
    /// </summary>
    private float T = default;

    /// <summary>
    /// フェードアウト前の音量.
    /// </summary>
    private float FromVolume = default;

    /// <summary>
    /// 移動時の移動前の位置.
    /// </summary>
    private Vector3 FromPosition = default;

    /// <summary>
    /// 移動時の移動後の位置.
    /// </summary>
    private Vector3 ToPosition = default;

    /// <summary>
    /// Scroll
    /// </summary>
    [SerializeField] private Scroll scroll;

    //================================================================================
    // Methods.
    //================================================================================
    
    //--------------------------------------------------------------------------------
    // MonoBehaviour methods.
    //--------------------------------------------------------------------------------
    
    private void Start()
    {
        Player.SetPlaying(false);
    }

    private void Update()
    {
        if      (CurrentState == State.Idle)
        {
            IdleProcess();
        }
        else if (CurrentState == State.Fade)
        {
            FadeProcess();
        }
        else if (CurrentState == State.Intro)
        {
            IntroProcess();
        }
        else if (CurrentState == State.Float)
        {
            FloatProcess();
        }
    }

    //--------------------------------------------------------------------------------
    // Public methods.
    //--------------------------------------------------------------------------------

    /// <summary>
    /// 最初のアニメーションを再生する.
    /// </summary>
    public void Play()
    {
        SetState(State.Idle);
    }

    //--------------------------------------------------------------------------------
    // Private methods.
    //--------------------------------------------------------------------------------
    
    /// <summary>
    /// 入力待ち処理.
    /// </summary>
    private void IdleProcess()
    {
        // どれかのキーが押されたら.
        if (Input.anyKeyDown)
        {
            FromVolume = AudioSource.volume;
            SetState(State.Fade);
        }
    }

    /// <summary>
    /// タイトル画面BGMフェードアウト処理.
    /// </summary>
    private void FadeProcess()
    {
        T                  = Mathf.Clamp01(T + Time.deltaTime / FadeSeconds);
        AudioSource.volume = Mathf.Lerp(FromVolume, 0.0f, T);

        if (T >= 1.0f)
        {
            T = 0.0f;
            
            // アニメーションを再生する.
            PlayableDirector.Play();
            
            SetState(State.Intro);
        }
    }

    /// <summary>
    /// アニメーション処理.
    /// </summary>
    private void IntroProcess()
    {
        if (PlayableDirector.time >= PlayableDirector.duration)
        {
            // 三角巾を非表示にする.
            Player.SetSankakuActive(false);
            
            // プレイヤーのアニメーションコントローラーを設定する.
            // 落下アニメーションを再生するため.
            Player.SetAnimationController(PlayerAnimationController);
            
            FromPosition = Vector3.zero;
            ToPosition   = Locator.position;
            SetState(State.Float);
        }
    }

    /// <summary>
    /// 上に移動する処理.
    /// </summary>
    private void FloatProcess()
    {
        T = Mathf.Clamp01(T + Time.deltaTime / FloatSeconds);
        var ratio = FloatCurve.Evaluate(T);
        Player.transform.position = Vector3.Lerp(FromPosition, ToPosition, ratio);

        if (T >= 1.0f)
        {
            Clouds.SetActive(false);
            Player.SetPlaying(true);
            gameObject.SetActive(false);
            scroll.ScrollStart();
        }
    }

    /// <summary>
    /// ステートを設定する.
    /// </summary>
    /// <param name="state">設定するステート.</param>
    private void SetState(State state)
    {
        CurrentState = state;
        Debug.Log($"CurrentState: {state}");
    }
}
