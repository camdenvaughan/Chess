using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private ChessGameController chessController;
    [SerializeField] private PromotionPieceManager promotionManager;
    [SerializeField] private float cameraMovementWaitTime;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float flipSpeed;


    private bool spinCamera;


    void Update()
    {
        if (spinCamera)
            CameraRotation();
    }


    private void CameraRotation()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    public void SetCameraToSpin(bool shouldSpin)
    {
        spinCamera = shouldSpin;
    }


    public void MoveCamera()
    {
        StartCoroutine(FlipCamera(flipSpeed));
    }

    IEnumerator FlipCamera(float duration)
    {
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + 180.0f;
        float t = 0.0f;
        yield return new WaitForSeconds(cameraMovementWaitTime);
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation,transform.eulerAngles.z);
            yield return null;
        }
    }
}
