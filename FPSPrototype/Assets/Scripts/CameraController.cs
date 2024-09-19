using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] int sense;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //get input 
        float mouseY = Input.GetAxis("Mouse Y") * sense * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sense * Time.deltaTime;


        //invert y camera
        if (!invertY)
            rotX -= mouseY;
        else
            rotX += mouseY;


        // clamp the rotX  on the X-axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        //rotate camera on the X-axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //rotate the player on the Y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }

}
