using UnityEngine;


//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
/// <summary>
/// 障害物クラス.
/// </summary>
//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

public sealed class Obstacle : MonoBehaviour
{
    //================================================================================
    // Fields.
    //================================================================================

    [SerializeField] private float          Speed            = 4.0f;
    [SerializeField] private float          DeadZone         = -5.0f;
    [SerializeField] private SpriteRenderer NegativeRenderer = default;
    [SerializeField] private SpriteRenderer PositiveRenderer = default;

    private System.Action<Obstacle> OnDestroyed = default;
    private bool                    IsDestroyed = default;
    
    //================================================================================
    // Methods.
    //================================================================================

    //--------------------------------------------------------------------------------
    // MonoBehaviour methods.
    //--------------------------------------------------------------------------------

    /// <summary>
    /// フレーム処理.
    /// </summary>
    private void Update()
    {
        // 障害物を移動させる.
        transform.Translate(0.0f, Time.deltaTime * -Speed, 0.0f);

        // 画面外になったら.
        if (!IsDestroyed && (transform.position.y < DeadZone - transform.localScale.y))
        {
            // 破壊フラグを立てる.
            OnDestroyed(this);
            IsDestroyed = true;
        }
    }

    //--------------------------------------------------------------------------------
    // Public methods.
    //--------------------------------------------------------------------------------

    /// <summary>
    /// 初期設定.
    /// </summary>
    /// <param name="onDestroyed">破壊されたら呼ばれるコールバック.</param>
    public void Setup(System.Action<Obstacle> onDestroyed)
    {
        OnDestroyed = onDestroyed;
    }

    /// <summary>
    /// 色の割合を設定する.
    /// </summary>
    /// <remarks>
    /// 0がネガティブ色、1がポジティブ色.
    /// </remarks>
    /// <param name="ratio">色の割合(0 - 1).</param>
    public void SetRatio(float ratio)
    {
        var color = NegativeRenderer.color;
        color.a                = 1.0f - ratio;
        NegativeRenderer.color = color;
        color                  = PositiveRenderer.color;
        color.a                = ratio;
        PositiveRenderer.color = color;
    }
}
