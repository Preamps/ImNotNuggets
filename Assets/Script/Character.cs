using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //public Animator anim;
    protected Rigidbody2D rb;
    [SerializeField] private int health;
    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
        }
    }
    
    public bool IsDead()
    {
        if (health <= 0)
        {
            Destroy(this.gameObject);
            return true;
        }
        else return false;
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;  
        IsDead();
    }
    public void Init(int newHealth)
    {
        Health = newHealth;
        rb = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();
        
    }
}
