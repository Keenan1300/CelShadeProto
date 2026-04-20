using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class JumpDust : MonoBehaviour
{
    private MeshRenderer alph;
    public Color transp;
    public Color opaq;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);   
        alph = GetComponent<MeshRenderer>();
        Destroy(gameObject, 0.6f);
        //alph.material.color = opaq;
    }

    // Update is called once per frame
    void Update()
    {
        //Quaternion Rot = transform.rotation.eulerAngles;
        transform.localScale += Vector3.one/50;
        Vector3 rot = transform.rotation.eulerAngles;
        rot.y += 10f * Time.deltaTime;
        transform.Rotate(0, 300 * Time.deltaTime, 0);
        //alph.material.color = Color.Lerp(transp,opaq, 1 - Time.deltaTime * 5f);

    }
}
