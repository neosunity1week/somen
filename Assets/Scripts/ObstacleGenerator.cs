using UnityEngine;

using System.Collections.Generic;


//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
/// <summary>
/// 障害物生成クラス.
/// </summary>
//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

public sealed class ObstacleGenerator : MonoBehaviour
{
    //================================================================================
    // Fields.
    //================================================================================

    [SerializeField] private GameObject ObstaclePrefab    = default;
    [SerializeField] private float      GeneratePositionX = 2.8f;
    [SerializeField] private float      GeneratePositionY = 7.0f;
    [SerializeField] private float      MinScale          = 1.0f;
    [SerializeField] private float      MaxScale          = 5.0f;

    private List<Obstacle> Obstacles = new();
    private float          Ratio     = 0.5f;

    //================================================================================
    // Methods.
    //================================================================================

    //--------------------------------------------------------------------------------
    // Public methods.
    //--------------------------------------------------------------------------------

    /// <summary>
    /// 色の割合を設定する.
    /// </summary>
    /// <remarks>
    /// 0がネガティブ色、1がポジティブ色.
    /// </remarks>
    /// <param name="ratio">色の割合(0 - 1).</param>
    public void SetRatio(float ratio)
    {
        Ratio = ratio;

        foreach (var obstacle in Obstacles)
        {
            obstacle.SetRatio(ratio);
        }
    }

    /// <summary>
    /// 障害物を生成する.
    /// </summary>
    public void Generate()
    {
        var scaleX = Random.Range(MinScale, MaxScale);
        var scaleY = Random.Range(MinScale, MaxScale);

        var positionX = Random.Range(0, 2) == 0 ? GeneratePositionX : -GeneratePositionX;
        var positionY = GeneratePositionY;

        // 障害物を生成する.
        var instance = Instantiate(
            ObstaclePrefab
          , new Vector3(positionX, positionY + scaleY, 0.0f)
          , Quaternion.identity
        );

        // スケールをランダムに設定する.
        instance.transform.localScale = new Vector3(scaleX, scaleY, 1.0f);

        if (instance.TryGetComponent<Obstacle>(out var obstacle))
        {
            // 障害物を初期設定する.
            obstacle.Setup(o =>
            {
                if (Obstacles.Contains(obstacle))
                {
                    Obstacles.Remove(obstacle);
                    Destroy(obstacle.gameObject);
                }
            });
            obstacle.SetRatio(Ratio);
            Obstacles.Add(obstacle);
        }
    }
}
