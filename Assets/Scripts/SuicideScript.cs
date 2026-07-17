using UnityEngine;

public class SuicideScript : MonoBehaviour
{
    public GameObject Player;
    public PlayerController Dance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Suicide()
    {
        Player = GameObject.Find("Player");
        Dance = Player.GetComponent<PlayerController>();
        Dance.Dancing = false;
        Dance.PlayerMesh.SetActive(true);
        Dance.RB.isKinematic = false;
        Destroy(gameObject);
    }
}
