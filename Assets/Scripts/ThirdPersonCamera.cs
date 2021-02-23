using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Transform player;

    [SerializeField] float cameraSensitivity;
    [SerializeField] float minY, maxY;
    [SerializeField] float rotationSmoothTime;
    [SerializeField] int layerMaskIndex;

    float yaw, pitch;

    Vector3 currentRotation;
    Vector3 rotationSmoothVelocity;

    Transform camera;
    Vector3 initialCameraOffset;
    float initialCameraDistance;

    void Start()
    {
        camera = transform.GetChild(0);
        initialCameraOffset = camera.localPosition;
        initialCameraDistance = (camera.position - player.position).magnitude;
    }

    void LateUpdate()
    {
        OrbitPlayer();

        AdjustCameraClipping();
    }

    void OrbitPlayer()
    {
        transform.position = player.transform.position;

        yaw += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        pitch += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, minY, maxY);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime * Time.deltaTime);

        transform.eulerAngles = currentRotation;
    }

    void AdjustCameraClipping()
    {
        RaycastHit hit;

        if (Physics.Raycast(player.position + Vector3.up, -camera.forward, out hit, initialCameraDistance, 1 << layerMaskIndex))
        {
            camera.position = hit.point + camera.transform.forward * .25f;
        }
        else
            camera.localPosition = initialCameraOffset;
    }

}
