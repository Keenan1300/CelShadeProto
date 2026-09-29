using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.Events;

public class HealthBar : MonoBehaviour

{
    public float Health;
    public float MaxHealth;
    public float Width;
    public float Height;
    public RectTransform healthbar;
    public Blackboard bb;
    public GameObject deadUI;
    public GameObject Player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deadUI.SetActive(false);
        Health = MaxHealth;
        bb = GetComponent<Blackboard>();
        healthbar = healthbar.GetComponent<RectTransform>();  
        healthbar.sizeDelta = new Vector2(100f, 100f);
        SetHealth(MaxHealth);
    }

    public void SetMaxHealth(float maxHealth)
    {
        MaxHealth = maxHealth;

    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        SetHealth(Health);
    }


    //set health to update 
    public void SetHealth(float health)
    {
        Health = health;
        float updatedwidth = (Health/MaxHealth)*Width;
        //float updatedwidth = Width - Health;
        healthbar.sizeDelta = new Vector2(updatedwidth, Height);

        if (Health <= 0)
        {
            deadUI.SetActive(true);
            Player.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
     
    }
}
