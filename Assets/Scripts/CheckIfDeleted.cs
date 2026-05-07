using UnityEngine;

public class CheckIfDeleted : MonoBehaviour
{

    public GameObject Target;
    private MeshRenderer Renderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer = GetComponent<MeshRenderer>();
        Renderer.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Target = Target.GetComponent<GameObject>();
        if (Target == null)
        {
            Renderer.enabled = true;
        }
    }

    public void setVisibility() 
    {
        Renderer.enabled = true;
    }
}
