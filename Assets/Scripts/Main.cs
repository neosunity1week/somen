using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
/// <summary>
/// メインクラス.
/// </summary>
//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

public sealed class Main : MonoBehaviour
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
        /// <summary>
        /// トランジションイン.
        /// </summary>
      , Entry
        /// <summary>
        /// ゲーム開始前.
        /// </summary>
      , Intro
        /// <summary>
        /// 重力の向きが安定している状態.
        /// </summary>
      , Idle
        /// <summary>
        /// 重力の変更前.
        /// </summary>
      , WillGravityChange
        /// <summary>
        /// 重力の変更中.
        /// </summary>
      , GravityChanging
        /// <summary>
        /// 重力の変更後.
        /// </summary>
      , GravityChanged
        /// <summary>
        /// ゲームオーバー.
        /// </summary>
      , GameOver
        /// <summary>
        /// トランジションアウト.
        /// </summary>
      , Leave
    }
    
    //================================================================================
    // Fields.
    //================================================================================

    [SerializeField] private Player                               Player                           = default;
    [SerializeField] private UIManager                            UIManager                        = default;
    [SerializeField] private ObstacleGenerator                    ObstacleGenerator                = default;
    [SerializeField] private Color                                NegativeColor                    = default;
    [SerializeField] private Color                                PositiveColor                    = default;
    [SerializeField] private float                                IntroWaitSeconds                 = 3.0f;
    [SerializeField] private float                                MaxGravity                       = 4.9f;
    [SerializeField] private float                                GravityChangeWaitSeconds         = 3.0f;
    [SerializeField] private float                                GravityChangeSeconds             = 3.0f;
    [SerializeField] private float                                GravityChangeIntervalRangeMin    = 10.0f;
    [SerializeField] private float                                GravityChangeIntervalRangeMax    = 60.0f;
    [SerializeField] private AnimationCurve                       GravityChangeCurve               = default;
    [SerializeField] private Image                                FadeImage                        = default;
    [SerializeField] private float                                FadeSeconds                      = 1.0f;
    [SerializeField] private float                                ObstacleGenerateIntervalRangeMin = 0.5f;
    [SerializeField] private float                                ObstacleGenerateIntervalRangeMax = 5.0f; 
    [SerializeField] private Cinemachine.CinemachineImpulseSource CinemachineImpulseSource         = default;

    private State CurrentState             = State.None;
    private float T                        = default;
    private bool  IsPositive               = true;
    private float GravityChangeInterval    = default;
    private float FromGravity              = default;
    private float ToGravity                = default;
    private float CurrentGravity           = default;
    private Color FromBackgroundColor      = default;
    private Color ToBackgroundColor        = default;
    private Color FromPlayerColor          = default;
    private Color ToPlayerColor            = default;
    private float ObstacleGenerateTimer    = default;
    private float ObstacleGenerateInterval = default;
    
    //================================================================================
    // Methods.
    //================================================================================

    //--------------------------------------------------------------------------------
    // MonoBehaviour methods.
    //--------------------------------------------------------------------------------

    /// <summary>
    /// 開始処理.
    /// </summary>
    private void Start()
    {
        // カメラの背景色を設定する.
        Camera.main.backgroundColor = Color.Lerp(Color.white, Color.black, 0.5f);
        
        // プレイヤーを初期設定する.
        Player.Setup(() =>
        {
            SetState(State.GameOver);
            CinemachineImpulseSource.GenerateImpulse();
        });
        
        // プレイヤーの色を設定する.
        Player.SetColor(Color.Lerp(NegativeColor, PositiveColor, 0.5f));
        
        // 重力の変更間隔を設定する.
        GravityChangeInterval = 1.0f;
        
        // ステートを進める.
        CurrentState = State.Entry;
    }

    /// <summary>
    /// 更新処理.
    /// </summary>
    private void Update()
    {
        var dt = Time.deltaTime;

        if      (CurrentState == State.Entry)
        {
            EntryProcess();
        }
        else if (CurrentState == State.Intro)
        {
            IntroProcess();
        }
        else if (CurrentState == State.Idle)
        {
            IdleProcess();
        }
        else if (CurrentState == State.WillGravityChange)
        {
            WillGravityChangeProcess();
        }
        else if (CurrentState == State.GravityChanging)
        {
            GravityChangingProcess();
        }
        else if (CurrentState == State.GravityChanged)
        {
            GravityChangedProcess();
        }
        else if (CurrentState == State.GameOver)
        {
            GameOverProcess();
        }
        else if (CurrentState == State.Leave)
        {
            LeaveProcess();
        }

        // スペースキーが押されるかマウスが左クリックされたら.
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            // ゲームオーバーなら.
            if (CurrentState == State.GameOver)
            {
                // トランジションアウトする.
                SetState(State.Leave);
            }
            else
            {
                // プレイヤーをジャンプさせる.
                Player.Resist();
            }
        }
        
        // Rキーが押されたら.
        if (Input.GetKeyDown(KeyCode.R))
        {
            // トランジションアウトする.
            SetState(State.Leave);
        }

        /// <summary>
        /// 重力の変更間隔を設定する.
        /// </summary>
        void SetGravityChangeInterval() => GravityChangeInterval = Random.Range(GravityChangeIntervalRangeMin, GravityChangeIntervalRangeMax);

        /// <summary>
        /// 重力のラベルを設定する.
        /// </summary>
        void SetGravityLabel()
        {
            UIManager.SetNegativeGravity(Mathf.Abs(Mathf.Min(CurrentGravity, 0.0f)));
            UIManager.SetPositiveGravity(Mathf.Max(CurrentGravity, 0.0f));
        }

        /// <summary>
        /// トランジションイン処理.
        /// </summary>
        void EntryProcess()
        {
            T = Mathf.Clamp01(T + dt / FadeSeconds);

            // フェード画像の透明度を変更する.
            var color = FadeImage.color;
            color.a = 1.0f - T;
            FadeImage.color = color;

            if (T >= 1.0f)
            {
                // ステートを進める.
                SetState(State.Intro);
                
                // 重力のラベルを変更する.
                SetGravityLabel();
            }
        }

        /// <summary>
        /// 開始前処理.
        /// </summary>
        void IntroProcess()
        {
            T = Mathf.Clamp01(T + dt / IntroWaitSeconds);

            if (T >= 1.0f)
            {
                // ステートを進める.
                SetState(State.Idle);
                
                // 重力のラベルを変更する.
                SetGravityLabel();
            }
        }

        /// <summary>
        /// アイドル処理.
        /// </summary>
        void IdleProcess()
        {
            T = Mathf.Clamp01(T + dt / GravityChangeInterval);

            // 障害物生成タイマーを進める.
            ObstacleGenerateTimer += dt;

            if (ObstacleGenerateTimer >= ObstacleGenerateInterval)
            {
                // 障害物を生成する.
                ObstacleGenerator.Generate();
                
                // タイマーをリセットする.
                ObstacleGenerateTimer -= ObstacleGenerateInterval;
                
                // 障害物を生成する間隔を設定する.
                ObstacleGenerateInterval = Random.Range(ObstacleGenerateIntervalRangeMin, ObstacleGenerateIntervalRangeMax);
            }

            if (T >= 1.0f)
            {
                // ステートを進める.
                SetState(State.WillGravityChange);
                
                // 重力の方向を変更する.
                IsPositive = !IsPositive;
                
                // 次の重力の方向を設定する.
                FromGravity = CurrentGravity;
                ToGravity = IsPositive ? MaxGravity : -MaxGravity;
                
                // タイマーをリセットする.
                ObstacleGenerateTimer = 0.0f;
            }
        }

        /// <summary>
        /// 重力変更前処理.
        /// </summary>
        void WillGravityChangeProcess()
        {
            T = Mathf.Clamp01(T + dt / GravityChangeWaitSeconds);

            // UIを徐々に表示する.
            UIManager.SetAlpha(T);

            if (T >= 1.0f)
            {
                // ステートを進める.
                SetState(State.GravityChanging);
                
                // 次に変更する背景色を設定する.
                FromBackgroundColor = Camera.main.backgroundColor;
                ToBackgroundColor = IsPositive ? Color.black : Color.white;
                
                // 次に変更するプレイヤーの色を設定する.
                FromPlayerColor = Player.Color;
                ToPlayerColor = IsPositive ? NegativeColor : PositiveColor;
            }
        }

        /// <summary>
        /// 重力変更中処理.
        /// </summary>
        void GravityChangingProcess()
        {
            T = Mathf.Clamp01(T + dt / GravityChangeSeconds);

            // 重力を変更する.
            var ratio = GravityChangeCurve.Evaluate(T);
            CurrentGravity = Mathf.Lerp(FromGravity, ToGravity, ratio);
            Player.SetGravity(CurrentGravity);

            // 重力のラベルを設定する.
            SetGravityLabel();

            // 背景色を徐々に変更する.
            Camera.main.backgroundColor = Color.Lerp(FromBackgroundColor, ToBackgroundColor, ratio);

            // プレイヤーの色を徐々に変更する.
            var color = Color.Lerp(FromPlayerColor, ToPlayerColor, ratio);
            Player.SetColor(color);

            // 障害物の色を徐々に変更する.
            ObstacleGenerator.SetRatio(Mathf.InverseLerp(-MaxGravity, MaxGravity, CurrentGravity));

            if (T >= 1.0f)
            {
                // ステートを進める.
                SetState(State.GravityChanged);
                
                // 重力を変更する間隔を設定する.
                SetGravityChangeInterval();
            }
        }

        /// <summary>
        /// 重力変更後処理.
        /// </summary>
        void GravityChangedProcess()
        {
            T = Mathf.Clamp01(T + dt / GravityChangeWaitSeconds);

            // UIを徐々に透明にする.
            UIManager.SetAlpha(1.0f - T);

            if (T >= 1.0f)
            {
                // ステートを進める.
                SetState(State.Idle);
            }
        }

        /// <summary>
        /// ゲームオーバー処理.
        /// </summary>
        void GameOverProcess()
        {

        }

        /// <summary>
        /// トランジションアウト処理.
        /// </summary>
        void LeaveProcess()
        {
            T = Mathf.Clamp01(T + dt / FadeSeconds);

            // フェード画像を徐々に不透明にする.
            var color = FadeImage.color;
            color.a = T;
            FadeImage.color = color;

            if (T >= 1.0f)
            {
                // シーンを読み込み直す.
                SceneManager.LoadScene("Main");
            }
        }
    }

    //--------------------------------------------------------------------------------
    // Private methods.
    //--------------------------------------------------------------------------------

    /// <summary>
    /// ステートを設定する.
    /// </summary>
    private void SetState(State state)
    {
        if (CurrentState == State.Leave)
        {
            // トランジションアウト中.
            return;
        }

        if ((CurrentState == State.GameOver) && (state != State.Leave))
        {
            // ゲームオーバー中にトランジションアウト以外のステートを設定しようとした場合.
            return;
        }

        // ステートを設定する.
        CurrentState = state;
        Debug.Log($"CurrentState: {CurrentState}");

        T = 0.0f;
    }
}
