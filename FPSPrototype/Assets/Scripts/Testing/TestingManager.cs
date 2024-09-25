using UnityEngine;

public class TestingManager : MonoBehaviour
{
    public static TestingManager Instance;

    public GameObject target;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        target = GameObject.FindWithTag("Target");
    }
}
