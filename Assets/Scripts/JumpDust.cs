using UnityEngine;

public class JumpDust : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);    
        Destroy(gameObject, 1.2f);
    }

    // Update is called once per frame
    void Update()
    {
        //Quaternion Rot = transform.rotation.eulerAngles;
        transform.localScale += Vector3.one/36;
        Vector3 rot = transform.rotation.eulerAngles;
        rot.y += 10f * Time.deltaTime;
        transform.Rotate(0, 300 * Time.deltaTime, 0);


    }
}
