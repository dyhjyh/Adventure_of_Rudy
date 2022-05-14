using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 控制角色移动、生命、动画等
/// <summary>
public class RubyController : MonoBehaviour
{
    public float speed = 5f; //移动速度
    public float timeInvincible = 2.0f;  //受伤后的无敌时间
    public int maxHealth = 5;  //最大生命值
    public int Health { get { return currentHealth; } }
    private int currentHealth;  //当前生命值

    private bool isInvincible;  //存储角色当前是否处于无敌状态
    private float invincibleTimer;  //存储角色恢复到受伤状态前剩余的无敌状态时间
    
    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private Vector2 lookDirection = new Vector2(1, 0);
    
    private float horizontal;
    private float vertical;

    public GameObject projectilePrefab;
    
    // Start is called before the first frame update
    void Start() // Unity 仅在游戏开始时执行一次 Start 中的代码。
    {
        // QualitySettings.vSyncCount = 0;
        // Application.targetFrameRate = 10;  // 设置Unity每秒渲染10帧
        rigidbody2d = GetComponent<Rigidbody2D>();  // 此代码要求 Unity 向你提供与脚本附加到同一游戏对象（即你的角色）上的 Rigidbody2D。
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;  // 游戏开始时角色的生命值为最大生命值
    }

    // Update is called once per frame
    // 在此 Update 函数中，你可以编写想要在游戏中连续发生的任何操作（例如，读取玩家的输入、移动游戏对象或计算经过的时间）
    void Update() // Unity 在每帧执行 Update 内的代码。
    {
        horizontal = Input.GetAxis("Horizontal"); //控制水平移动方向 A:-1 D:1 不按:0
        vertical = Input.GetAxis("Vertical"); //控制垂直移动方向 S:-1 W:1 不按:0

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                isInvincible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Launch();
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x += speed * horizontal * Time.deltaTime;  // 乘以Unity渲染一帧所需的时间，使移动速度的单位由"单位长度/帧"转换到"单位长度/秒"，
        position.y += speed * vertical * Time.deltaTime;    // 以解决在不同计算机上运行的帧率不同所导致的角色运动速度不同的问题
        rigidbody2d.MovePosition(position);  // 这行代码会将刚体移动到你想要的位置，但是如果刚体在移动中与另一个碰撞体碰撞，则会中途停止刚体。
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
            {
                return;
            }

            isInvincible = true;
            invincibleTimer = timeInvincible;
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);  //钳制功能使当前生命值在0和最大生命值之间
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 3000);
        
        animator.SetTrigger("Launch");
    }
}
