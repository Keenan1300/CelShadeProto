using UnityEngine;
using UnityEngine.Events;

public class GrindCanCollect : MonoBehaviour
{
    public UnityEvent CollectGraffitiCan;
    public GameObject TouchedCan;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void deleteThisCan()
    {
        Destroy(TouchedCan);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
