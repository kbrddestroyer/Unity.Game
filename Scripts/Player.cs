using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] private float speed;
    [SerializeField, Range(0f, 10f)] private float msens;
    private Vector3 rotation;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rotation = new Vector3(0, 0, 0);
    }
    void Update()
    { 
        float new_x = Input.GetAxis("Mouse X") * msens;
        float new_y = Input.GetAxis("Mouse Y") * msens;

        rotation.x -= new_y;
        rotation.y += new_x;
        rotation.x = Mathf.Clamp(rotation.x, -89f, 89f);
        rotation.z = 0;

        transform.rotation = Quaternion.Euler(0, rotation.y, 0);
        Camera.main.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);

        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed * Time.deltaTime);
    }
}
