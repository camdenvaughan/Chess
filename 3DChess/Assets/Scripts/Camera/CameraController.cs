using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private ChessGameController chessController;
    [SerializeField] private PromotionPieceManager promotionManager;
    [SerializeField] private float cameraMovementWaitTime;
    [SerializeField] private float flipSpeed;

    private float yValue;

    private bool spinCamera;


    [SerializeField] private float rotationSpeed;
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
        StartCoroutine(CameraMovement());
    }

    private IEnumerator CameraMovement()
    {
        yield return new WaitForSeconds(cameraMovementWaitTime);

            StartCoroutine(LerpFromTo());

        
    }

    IEnumerator LerpFromTo()
    {
        yield return new WaitForSeconds(cameraMovementWaitTime);
        if (chessController.IsTeamTurnActive(TeamColor.White))
            yValue = 0;
        else if (chessController.IsTeamTurnActive(TeamColor.Black))
            yValue = 180;

        for (float t = 0f; t < flipSpeed; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, yValue, transform.rotation.z), t / flipSpeed);
            yield return 0;
        }

        transform.rotation = Quaternion.Euler(transform.rotation.x, yValue, transform.rotation.z);
    }
}
