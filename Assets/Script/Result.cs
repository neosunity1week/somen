using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
/// <summary>
/// 最初のアニメーション再生クラス.
/// </summary>
//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    
public sealed class Result : MonoBehaviour
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
      , FadeIn 
      , End
    }

    //================================================================================
    // Fields.
    //================================================================================

    /// <summary>
    /// カメラのオブジェクト.
    /// </summary>
    [SerializeField] private GameObject Camera = default;

    /// <summary>
    /// イベントシステム.
    /// </summary>
    [SerializeField] private GameObject EventSystem = default;

    /// <summary>
    /// キャンバス.
    /// </summary>
    [SerializeField] private Canvas[] Canvases = default;

    /// <summary>
    /// キャンバスグループ.
    /// </summary>
    [SerializeField] private CanvasGroup[] CanvasGroups = default;

    /// <summary>
    /// フェードインの秒数.
    /// </summary>
    [SerializeField] private float FadeInDuration = 3.0f;
    
    /// <summary>
    /// 現在のステート.
    /// </summary>
    private State CurrentState = default;

    /// <summary>
    /// 一定時間処理し続ける際に使用.
    /// </summary>
    private float T = default;

    //================================================================================
    // Methods.
    //================================================================================
    
    //--------------------------------------------------------------------------------
    // MonoBehaviour methods.
    //--------------------------------------------------------------------------------
    
    private void Start()
    {
        Initialize();
        SetState(State.FadeIn);

        /// <summary>
        /// 初期化する.
        /// </summary>
        void Initialize()
        {
            // 親シーンを取得する.
            var parentScene           = SceneManager.GetSceneByName("SampleScene");

            if (!parentScene.IsValid())
            {
                Camera.SetActive(true);
                EventSystem.SetActive(true);
                return;
            }
            
            var parentRootGameObjects = parentScene.GetRootGameObjects();

            if (parentRootGameObjects == null)
            {
                Camera.SetActive(true);
                EventSystem.SetActive(true);
                return;
            }
            
            // 親シーンのカメラのオブジェクトを取得する.
            var parentCamera = parentRootGameObjects.FirstOrDefault(o => o.name == "Main Camera");

            if (parentCamera == null)
            {
                Camera.SetActive(true);
                EventSystem.SetActive(true);
                return;
            }

            // 親シーンのカメラを取得する.
            if (!parentCamera.TryGetComponent<Camera>(out var camera))
            {
                Camera.SetActive(true);
                EventSystem.SetActive(true);
                return;
            }
            
            // リザルトにあるキャンバスのカメラを親シーンのカメラにする.
            foreach (var canvas in Canvases)
            {
                canvas.worldCamera = camera;
            }
            
            // 元々あるカメラとイベントシステムを削除する.
            Destroy(Camera);
            Destroy(EventSystem);
        }
    }

    private void Update()
    {
        if      (CurrentState == State.FadeIn)
        {
            FadeInProcess();
        }
    }
    //--------------------------------------------------------------------------------
    // Private methods.
    //--------------------------------------------------------------------------------
    
    /// <summary>
    /// フェードイン処理.
    /// </summary>
    private void FadeInProcess()
    {
        T                 = Mathf.Clamp01(T + Time.deltaTime / FadeInDuration);

        foreach (var canvasGroup in CanvasGroups)
        {
            canvasGroup.alpha = T;
        }

        if (T >= 1.0f)
        {
            T = 0.0f;
            SetState(State.End);
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
