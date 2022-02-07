using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Range(0 , 100) , SerializeField]
    public float moveSpeed = 5f;
    private Rigidbody _rigibody;

    private Vector2 mouseInput;
    private Vector3 moveDirection, movement;
    public float mouseSensitivity = 1f;
    private float verticalRotationStore;
    public int verticalRotationLimits = 60;
    public bool invertLook;
    public Transform viewPoint;
    // Start is called before the first frame update
    void Start()
    {
        _rigibody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        verticalRotationStore += mouseInput.y;
        verticalRotationStore = Mathf.Clamp(verticalRotationStore, -verticalRotationLimits, verticalRotationLimits);

        if (invertLook == true)
        {
            viewPoint.rotation = Quaternion.Euler(verticalRotationStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }
        else
        {
            viewPoint.rotation = Quaternion.Euler(-verticalRotationStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }


        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        movement = ((transform.forward * moveDirection.z) + (transform.right * moveDirection.x)).normalized;

        _rigibody.AddForce(movement * moveSpeed, ForceMode.Force);

        if (Input.GetKey(KeyCode.LeftShift)) 
        {
            moveSpeed = 10f;
        }
        else
        {
            moveSpeed = 5f;
        }
    }
}
