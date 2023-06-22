using UnityEngine;


//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
/// <summary>
/// 最後のアニメーション再生クラス.
/// </summary>
//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    
public sealed class Outro : MonoBehaviour
{
    //================================================================================
    // Definitions.
    //================================================================================

    /// <summary>
    /// ステート.
    /// </summary>
    private enum State : int
    {
        Neutral
      , AnimationPlay
      , AnimationPlaying
    }

    //================================================================================
    // Fields.
    //================================================================================

    /// <summary>
    /// プレイヤー.
    /// </summary>
    [SerializeField] private PlayerController Player = default;

    /// <summary>
    /// X座標を中央にする秒数.
    /// </summary>
    [SerializeField] private float NeutralSeconds = 1.5f;

    /// <summary>
    /// X座標を中央にするカーブ.
    /// </summary>
    [SerializeField] private AnimationCurve NeutralCurve = default;

    /// <summary>
    /// プレイヤーのアニメーションコントローラー.
    /// </summary>
    [SerializeField] private RuntimeAnimatorController PlayerAnimationController = default;

    /// <summary>
    /// 現在のステート.
    /// </summary>
    private State CurrentState = default;

    /// <summary>
    /// 一定時間処理し続ける際に使用.
    /// </summary>
    private float T = default;

    /// <summary>
    /// 移動時の移動前の位置.
    /// </summary>
    private Vector3 From = default;

    /// <summary>
    /// 移動時の移動後の位置.
    /// </summary>
    private Vector3 To = default;

    /// <summary>
    /// 最後のアニメーションステートのハッシュ.
    /// </summary>
    private int LastStateHash = default;
    
    //================================================================================
    // Methods.
    //================================================================================
    
    //--------------------------------------------------------------------------------
    // MonoBehaviour methods.
    //--------------------------------------------------------------------------------
    
    private void Start()
    {
        Player.SetPlaying(false);
        From = Player.transform.position;
        To   = From;
        To.x = 0.0f;
        SetState(State.Neutral);
    }

    private void Update()
    {
        if      (CurrentState == State.Neutral)
        {
            NeutralProcess();
        }
        else if (CurrentState == State.AnimationPlay)
        {
            AnimationPlayProcess();
        }
        else if (CurrentState == State.AnimationPlaying)
        {
            AnimationPlayingProcess();
        }
    }

    //--------------------------------------------------------------------------------
    // Private methods.
    //--------------------------------------------------------------------------------
    
    /// <summary>
    /// 入力待ち処理.
    /// </summary>
    private void NeutralProcess()
    {
        T = Mathf.Clamp01(T + Time.deltaTime / NeutralSeconds);
        var ratio = NeutralCurve.Evaluate(T);
        Player.transform.position = Vector3.Lerp(From, To, ratio);

        if (T >= 1.0f)
        {
            SetState(State.AnimationPlay);
        }
    }

    /// <summary>
    /// アニメーション処理.
    /// </summary>
    private void AnimationPlayProcess()
    {
        Player.SetAnimationController(PlayerAnimationController);
        Player.Animator.SetTrigger("Outro");
        LastStateHash = Player.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        SetState(State.AnimationPlaying);
    }

    /// <summary>
    /// アニメーション処理.
    /// </summary>
    private void AnimationPlayingProcess()
    {
        var currentAnimatorState = Player.Animator.GetCurrentAnimatorStateInfo(0);

        if ((currentAnimatorState.fullPathHash != LastStateHash) || (currentAnimatorState.normalizedTime >= 1.0f))
        {
            gameObject.SetActive(false);
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
