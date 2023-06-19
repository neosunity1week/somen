using UnityEngine;


//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
/// <summary>
/// プレイヤークラス.
/// </summary>
//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

public sealed class Player : MonoBehaviour
{
    //================================================================================
    // Fields.
    //================================================================================

    [SerializeField] private Rigidbody2D    Rb             = default;
    [SerializeField] private float          MaxResistPower = 10.0f;
    [SerializeField] private SpriteRenderer Renderer       = default;
    [SerializeField] private ParticleSystem TrailParticle  = default;
    [SerializeField] private ParticleSystem DeadParticle   = default;
    [SerializeField] private float          DeadZone       = 2.6f;
    [SerializeField] private GameObject     StepPrefab     = default;

    private float         Gravity            = 0.0f;
    private float         CurrentResistPower = default;
    private System.Action OnDead             = default;
    private bool          IsDead             = default;
    
    //================================================================================
    // Properties.
    //================================================================================

    /// <summary>
    /// プレイヤーの現在の色.
    /// </summary>
    public Color Color => Renderer.color;
    
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
        // 画面外に行ったら死亡する.
        if (!IsDead && ((transform.position.x < -DeadZone) || (transform.position.x > DeadZone)))
        {
            Dead();
        }
    }

    /// <summary>
    /// 固定フレーム処理.
    /// </summary>
    private void FixedUpdate()
    {
        // 重力を加える.
        Rb.AddForce(new Vector2(Gravity, 0.0f), ForceMode2D.Force);
    }

    /// <summary>
    /// 衝突処理.
    /// </summary>
    /// <param name="collision">衝突相手.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 相手が障害物なら.
        if (collision.gameObject.TryGetComponent<Obstacle>(out var obstacle))
        {
            // 死亡する.
            Dead();
        }
    }

    //--------------------------------------------------------------------------------
    // Public methods.
    //--------------------------------------------------------------------------------
    
    /// <summary>
    /// 初期設定.
    /// </summary>
    /// <param name="onDead">死亡すると呼ばれるコールバック.</param>
    public void Setup(System.Action onDead)
    {
        OnDead = onDead;
    }

    /// <summary>
    /// 重力を設定する.
    /// </summary>
    /// <param name="gravity">x方向の重力.</param>
    public void SetGravity(float gravity)
    {
        Gravity = gravity;
    }

    /// <summary>
    /// 色を設定する.
    /// </summary>
    /// <param name="color">色.</param>
    public void SetColor(Color color)
    {
        Renderer.color = color;
        var main = TrailParticle.main;
        main.startColor = color;
    }

    /// <summary>
    /// 重力に抵抗する.
    /// </summary>
    public void Resist()
    {
        if (Mathf.Approximately(Gravity, 0.0f))
        {
            return;
        }

        if (Gravity < 0.0f)
        {
            Rb.AddForce(new Vector2(MaxResistPower, 0.0f), ForceMode2D.Impulse);
        }
        else
        {
            Rb.AddForce(new Vector2(-MaxResistPower, 0.0f), ForceMode2D.Impulse);
        }

        // エフェクトを生成する.
        var instance = Instantiate(StepPrefab, transform.position, Quaternion.identity);
        if (instance.TryGetComponent<ParticleSystem>(out var particleSystem))
        {
            var main = particleSystem.main;
            main.startColor = Color;
        }

        Destroy(instance, 0.5f);
    }

    //--------------------------------------------------------------------------------
    // Private methods.
    //--------------------------------------------------------------------------------

    /// <summary>
    /// 死亡する.
    /// </summary>
    private void Dead()
    {
        // プレイヤーを非表示にする.
        Renderer.gameObject.SetActive(false);
        TrailParticle.gameObject.SetActive(false);
        
        // 死亡エフェクトを生成する.
        var main = DeadParticle.main;
        main.startColor = Color;
        DeadParticle.gameObject.SetActive(true);
        
        // コールバックを呼ぶ.
        OnDead?.Invoke();
        
        // フラグを立てる.
        IsDead = true;
    }
}
