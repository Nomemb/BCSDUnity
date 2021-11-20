using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    public GameObject projectilePrefab;
    public AudioClip throwSound;
    public AudioClip hitSound;

    public int health { get { return currentHealth; } }
    int currentHealth;

    // 현 상태가 무적인지 저장한다.
    bool isInvincivle;
    // 다시 데미지를 입는 상태로 전환되기 전까지 무적상태의 지속 정도를 저장한다.
    float invincibleTimer;

    AudioSource audioSource;
    Rigidbody2D rigidbody2d;
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);
    // Start is called before the first frame update
    void Start()
    {

        // 초당 프레임
        // 각 모니터 간의 프레임 차이에 따라 움직임이 달라지면 안되기 때문에
        // 이동을 초당 유닛으로 표현해야 한다.
        // deltaTime을 곱해줘서 모든 컴퓨터 환경에서 동일하게 이동하게 맞추고 나면
        // 아래의 두 행을 작동시킬 때 이동 연출이 끊기게 보일 수 있다.
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        Vector2 position = rigidbody2d.position;
        position = position + move * speed * Time.deltaTime;

        rigidbody2d.MovePosition(position);

        
        if (isInvincivle)
        {
            invincibleTimer -= Time.deltaTime;
            if(invincibleTimer < 0)
                isInvincivle = false;   
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        // talk : key X
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if(hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if(character != null)
                {
                    character.DisplayDialog();
                }
            }
        }
    }
    public void ChangeHealth(int amount)
    {
        // 현재 데미지를 입고 있으면 
        if (amount < 0)
        {
            // 무적상태를 확인한 후 무적상태이면 종료
            if (isInvincivle)
                return;

            // 아니면 체력을 바꾸고 무적상태로 바꾼다.
            isInvincivle = true;
            invincibleTimer = timeInvincible;
          
        }

        // 0 <= currentHealth + amount <= maxHealth
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
    }
}
