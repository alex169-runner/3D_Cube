using System.Collections;
using UnityEngine;

public class PeekCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float radius = 10f;
    public float rotationSpeed = 1.5f;
    public float smoothTime = 0.15f;
    public float returnDuration = 0.7f;

    [Header("Angle Limits")]
    public float minVerticalAngle = 10f;
    public float maxVerticalAngle = 170f;

    private float currentHorizontalAngle;
    private float currentVerticalAngle;
    private float targetHorizontalAngle;
    private float targetVerticalAngle;
    private float startHorizontalAngle = -90f;
    private float startVerticalAngle = 90f;

    private Vector3 horizontalVelocity = Vector3.zero;
    private Vector3 verticalVelocity = Vector3.zero;

    private bool isReturningToStart = false;
    private Vector3 startPosition = new Vector3(0, 0, -10);
    private Quaternion startRotation;
    private float returnProgress = 0f;
    private float returnPrecision = 0.2f;

    private void Start()
    {
        transform.position = startPosition;
        startRotation = transform.rotation;

        currentHorizontalAngle = targetHorizontalAngle = startHorizontalAngle;
        currentVerticalAngle = targetVerticalAngle = startVerticalAngle;

        transform.LookAt(Vector3.zero);
    }

    private void Update()
    {
        if (!isReturningToStart) {
            HandleInput();
            SmoothUpdateCamera();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            StartCoroutine(ReturnToStart());
            return;
        }

        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            horizontalInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            horizontalInput = 1f;

        if (Input.GetKey(KeyCode.UpArrow))
            verticalInput = -1f;
        else if (Input.GetKey(KeyCode.DownArrow))
            verticalInput = 1f;

        targetHorizontalAngle += horizontalInput * rotationSpeed;

        targetVerticalAngle += verticalInput * rotationSpeed;
        targetVerticalAngle = Mathf.Clamp(targetVerticalAngle, minVerticalAngle, maxVerticalAngle);
    }

    private void SmoothUpdateCamera()
    {
        currentHorizontalAngle = Mathf.SmoothDamp(currentHorizontalAngle, targetHorizontalAngle, ref horizontalVelocity.x, smoothTime);
        currentVerticalAngle = Mathf.SmoothDamp(currentVerticalAngle, targetVerticalAngle, ref verticalVelocity.x, smoothTime);

        UpdateCameraPosition();
        transform.LookAt(Vector3.zero);

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
    }

    private void UpdateCameraPosition()
    {
        float horizontalRad = currentHorizontalAngle * Mathf.Deg2Rad;
        float verticalRad = currentVerticalAngle * Mathf.Deg2Rad;

        float x = radius * Mathf.Sin(verticalRad) * Mathf.Cos(horizontalRad);
        float y = radius * Mathf.Cos(verticalRad);
        float z = radius * Mathf.Sin(verticalRad) * Mathf.Sin(horizontalRad);

        transform.position = new(x, y, z);
    }

    private IEnumerator ReturnToStart()
    {
        isReturningToStart = true;
        returnProgress = 0.0f;

        while (returnProgress < 1.0f) {
            returnProgress += Time.deltaTime / returnDuration;

            transform.position = Vector3.Slerp(transform.position, startPosition, returnProgress);
            transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, returnProgress);

            if (Vector3.Distance(transform.position, startPosition) / radius < returnPrecision) {
                break;
            }

            yield return null;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;

        currentHorizontalAngle = targetHorizontalAngle = startHorizontalAngle;
        currentVerticalAngle = targetVerticalAngle = startVerticalAngle;

        isReturningToStart = false;
        returnProgress = 0.0f;

    }
}