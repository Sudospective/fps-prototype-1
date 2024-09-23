using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(animator);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
