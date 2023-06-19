using UnityEngine;

using TMPro;


//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
/// <summary>
/// UI管理クラス.
/// </summary>
//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

public sealed class UIManager : MonoBehaviour
{
    //================================================================================
    // Fields.
    //================================================================================

    [SerializeField] private TextMeshPro NegativeGravityLabel = default;
    [SerializeField] private TextMeshPro PositiveGravityLabel = default;

    //================================================================================
    // Methods.
    //================================================================================

    //--------------------------------------------------------------------------------
    // Public methods.
    //--------------------------------------------------------------------------------

    /// <summary>
    /// アルファを設定する.
    /// </summary>
    /// <param name="alpha">アルファ.</param>
    public void SetAlpha(float alpha)
    {
        var color = NegativeGravityLabel.color;
        color.a = alpha;
        NegativeGravityLabel.color = color;
        color = PositiveGravityLabel.color;
        color.a = alpha;
        PositiveGravityLabel.color = color;
    }

    /// <summary>
    /// 左方向の重力のラベルを設定する.
    /// </summary>
    /// <param name="value">重力.</param>
    public void SetNegativeGravity(float value) => NegativeGravityLabel.text = $"{value:F1}";

    /// <summary>
    /// 右方向の重力のラベルを設定する.
    /// </summary>
    /// <param name="value">重力.</param>
    public void SetPositiveGravity(float value) => PositiveGravityLabel.text = $"{value:F1}";
}
