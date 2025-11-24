using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Character : MonoBehaviour
{
    //public Animator anim;
    protected Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Color originalColor;
    public DeathUIManager deathUIManager;   // assign in Inspector

    [SerializeField] private int health;
    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            // Update health bar if assigned
            if (uiHealthBarrrrrr != null)
                uiHealthBarrrrrr.UpdateHealthBar(health);

        }
    }

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            originalColor = sprite.color;   // save starting color
    }

    [SerializeField] private HealthBar uiHealthBarrrrrr;


    public bool IsDead()
    {
        if (health <= 0)
        {

            if (deathUIManager != null)
                deathUIManager.ShowDeathUI();


            WaveManager.EnemyDeath notifier = GetComponent<WaveManager.EnemyDeath>();
            if (notifier != null)
                notifier.OnDeath();

            Destroy(this.gameObject);
            return true;
        }
        else return false;
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        StartCoroutine(FlashRed());
        IsDead();
    }

    IEnumerator FlashRed()
    {
        if (sprite != null)
        {
            sprite.color = Color.red;               // flash red
            yield return new WaitForSeconds(0.1f);
            sprite.color = originalColor;           // restore original color
        }
    }
    public void Init(int newHealth)
    {
        Health = newHealth;
        rb = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();
        if (uiHealthBarrrrrr != null)
            uiHealthBarrrrrr.SetMaxHealth(newHealth);


    }
}
