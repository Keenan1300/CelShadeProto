using UnityEngine;

public class SequenceAutoPlayLogii : MonoBehaviour
{

    public Animation Anim;
    public AnimationClip spray;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Anim = GetComponent<Animation>();
        spray = GetComponent<AnimationClip>();
        Anim.Play("spray");

    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
