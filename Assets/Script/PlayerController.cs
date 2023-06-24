using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;


public class PlayerController : MonoBehaviour
{
    public enum GravityDirection
    {
        right,
        left,
    }

    [Header("jump no ookisa")][SerializeField] private float forceSize = 4.0f;
    [Header("jyuuryoku no tsuyosa")] public float gravitySize = 700f;
    private Rigidbody2D rb;
    private Vector2 gravity;

    /// <summary>
    /// アニメーター.
    /// </summary>
    [SerializeField] private Animator _animator = default;

    /// <summary>
    /// 三角巾.
    /// </summary>
    [SerializeField] private GameObject Sankaku = default;

    /// <summary>
    /// 右から左に重力が変更されることを通知するアニメーション.
    /// </summary>
    [SerializeField] private PlayableDirector GravityChangeNoticeRightToLeft = default;

    /// <summary>
    /// 左から右に重力が変更されることを通知するアニメーション.
    /// </summary>
    [SerializeField] private PlayableDirector GravityChangeNoticeLeftToRight = default;

    /// <summary>
    /// プレイ中か？
    /// </summary>
    /// <remarks>
    /// プレイ中の場合、重力が有効になる.
    /// </remarks>
    private bool IsPlaying = default;

    /// <summary>
    /// アニメーター.
    /// </summary>
    public Animator Animator => _animator;

    [SerializeField] private AudioManager audioManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (!IsPlaying)
        {
            return;
        }

        rb.AddForce(gravity * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            rb.velocity = new Vector2(Math.Sign(gravity.x) * -1 * forceSize, 0);
            audioManager.Jump();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetPlayerGravity(GravityDirection.right);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetPlayerGravity(GravityDirection.left);
        }
    }

    /// <summary>
    /// プレイ中かどうかを設定する.
    /// </summary>
    /// <param name="value">設定する値.</param>
    public void SetPlaying(bool value) => IsPlaying = value;

    /// <summary>
    /// アニメーションコントローラーを設定する.
    /// </summary>
    public void SetAnimationController(RuntimeAnimatorController animatorController) =>
        Animator.runtimeAnimatorController = animatorController;

    /// <summary>
    /// アニメーションコントローラーをリセットする.
    /// </summary>
    public void ResetAnimationController() => Animator.runtimeAnimatorController = null;

    /// <summary>
    /// 三角巾のアクティブ状態を変更する.
    /// </summary>
    /// <param name="active">設定するアクティブ状態.</param>
    public void SetSankakuActive(bool active) => Sankaku.SetActive(active);

    public void SetPlayerGravity(GravityDirection gravityDirection)
    {
        if (gravityDirection == GravityDirection.right)
        {
            gravity = Vector2.right * gravitySize;
        }
        else if (gravityDirection == GravityDirection.left)
        {
            gravity = Vector2.left * gravitySize;
        }
    }

    /// <summary>
    /// 重力を左に変更する.
    /// </summary>
    public void ChangeToLeftGravity() => SetPlayerGravity(GravityDirection.left);

    /// <summary>
    /// 重力を右に変更する.
    /// </summary>
    public void ChangeToRightGravity() => SetPlayerGravity(GravityDirection.right);

    private void OnBecameInvisible() => this.PlayerRespawn();

    private float coolTime = 0.0f;
    [SerializeField] private float coolTimeDuration = 0.0f;
    [SerializeField] private ScoreManager score;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tagName = collision.transform.tag;
        if (tagName == "Cloud" && coolTime < Time.time)
        {
            PlayerRespawn();
            coolTime = Time.time + coolTimeDuration;

            TryPlayGravityChangeNoticeAnimation();
        }
        else if (tagName == "Item")
        {
            GameObject item = collision.gameObject;
            item.SetActive(false);
            score.AddScore(50);
            audioManager.ItemGet();
        }
    }
    private void PlayerRespawn()
    {
        audioManager.Damage();
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Hit");
        StartCoroutine(Flash(anim, coolTimeDuration));
        transform.position = new Vector2(0, transform.position.y);
        rb.velocity = Vector2.zero;
        score.AddScore(-50);
    }

    /// <summary>
    /// 重力変更を通知するアニメーション再生する.
    /// </summary>
    public void PlayChangeNoticeAnimation()
    {
        // 重力が左にかかっているなら.
        if (gravity.x < 0.0f)
        {
            GravityChangeNoticeLeftToRight.Play();
        }
        // 重力が右にかかっているなら.
        else
        {
            GravityChangeNoticeRightToLeft.Play();
        }
    }

    /// <summary>
    /// 重力変更を通知するアニメーションの再生を試みる.
    /// </summary>
    /// <returns>再生できたら、trueを返す.</returns>
    private bool TryPlayGravityChangeNoticeAnimation()
    {
        if (IsChangeNoticeAnimationPlaying())
        {
            // すでに再生中.
            return false;
        }
        PlayChangeNoticeAnimation();

        return true;

        /// <summary>
        /// 重力変更を通知するアニメーションが再生中か？
        /// </summary>
        bool IsChangeNoticeAnimationPlaying() => (GravityChangeNoticeRightToLeft.state == PlayState.Playing) ||
                                                 (GravityChangeNoticeLeftToRight.state == PlayState.Playing);
    }

    private IEnumerator Flash(Animator animator, float seconds)
    {
        Debug.Log("call");
        animator.SetBool("Flash",true);
        yield return new WaitForSeconds(seconds);
        animator.SetBool("Flash", false);
    }
}
