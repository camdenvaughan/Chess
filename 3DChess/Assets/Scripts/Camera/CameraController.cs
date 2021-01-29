using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private ChessGameController chessController;
    [SerializeField] private PromotionPieceManager promotionManager;
    [SerializeField] private float cameraMovementWaitTime;
    [SerializeField] private float spinSpeed;

    private float yValue;


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
        if (chessController.IsActiveTeamWhite())
            yValue = 0;
        else if (!chessController.IsActiveTeamWhite())
            yValue = 180;

        for (float t = 0f; t < spinSpeed; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, yValue, transform.rotation.z), t / spinSpeed);
            yield return 0;
        }

        transform.rotation = Quaternion.Euler(transform.rotation.x, yValue, transform.rotation.z);
    }
}
