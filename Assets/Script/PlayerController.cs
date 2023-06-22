using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum GravityDirection
    {
        right,
        left,
    }

    [Header("jump no ookisa")][SerializeField]  private float forceSize = 4.0f;
    [Header("jyuuryoku no tsuyosa")] public float gravitySize = 700f;
    private Rigidbody2D rb;
    private Vector2 gravity;

    /// <summary>
    /// アニメーター.
    /// </summary>
    [SerializeField] private Animator _animator = default;

    /// <summary>
    /// アニメーションコントローラー.
    /// </summary>
    [SerializeField] private RuntimeAnimatorController AnimatorController = default;

    /// <summary>
    /// 三角巾.
    /// </summary>
    [SerializeField] private GameObject Sankaku = default;

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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetPlayerGravity(GravityDirection.right);
    }
    private void Update()
    {
        if (!IsPlaying)
        {
            return;
        }
        
        rb.AddForce(gravity * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(Mathf.Sign(gravity.x) * -1 * forceSize,0);
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
    public void SetAnimationController() => Animator.runtimeAnimatorController = AnimatorController;

    /// <summary>
    /// 三角巾のアクティブ状態を変更する.
    /// </summary>
    /// <param name="active">設定するアクティブ状態.</param>
    public void SetSankakuActive(bool active) => Sankaku.SetActive(active);
    
    public void SetPlayerGravity(GravityDirection gravityDirection)
    {
        if(gravityDirection == GravityDirection.right)
        {
            gravity = Vector2.right * gravitySize;
        }
        else if(gravityDirection == GravityDirection.left)
        {
            gravity = Vector2.left * gravitySize;
        }
    }

    private void OnBecameInvisible() => this.PlayerRespawn();

    private float coolTime = 0.0f;
    [SerializeField] private float coolTimeDuration = 0.0f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tagName = collision.transform.tag;
        if(tagName == "Cloud" && coolTime < Time.time)
        {
            PlayerRespawn();
            coolTime = Time.time + coolTimeDuration;
        }
        else if(tagName == "Item")
        {
            GameObject item = collision.gameObject;
            item.SetActive(false);
        }
    }
    private void PlayerRespawn()
    {
        GetComponent<Animator>().SetTrigger("Hit");
        transform.position = new Vector2(0, transform.position.y);
        rb.velocity = Vector2.zero;
    }
}
