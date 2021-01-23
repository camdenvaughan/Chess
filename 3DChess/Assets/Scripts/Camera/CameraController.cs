using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private ChessGameController chessController;
    [SerializeField] private PromotionPieceManager promotionManager;
    [SerializeField] private Vector3 whiteCoords;
    [SerializeField] private Vector3 blackCoords;
    [SerializeField] private float cameraMovementWaitTime;
    

    

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    public void MoveCamera()
    {
        StartCoroutine(CameraMovement(chessController.IsActiveTeamWhite()));
    }

    private IEnumerator CameraMovement(bool isWhiteTeam)
    {
        yield return new WaitForSeconds(cameraMovementWaitTime);
        if (isWhiteTeam)
        {
            StartCoroutine(LerpFromTo(blackCoords, Quaternion.Euler(50, 180f, 0f), whiteCoords, Quaternion.Euler(50f, 0f, 0f), 2f));

        }
        if (!isWhiteTeam)
        {
            StartCoroutine(LerpFromTo(whiteCoords,Quaternion.Euler(50, 0f, 0f), blackCoords,Quaternion.Euler(50f, 180f, 0f), 2f));

        }
        yield return new WaitForSeconds(cameraMovementWaitTime);
    }

    IEnumerator LerpFromTo(Vector3 pos1, Quaternion rot1, Vector3 pos2, Quaternion rot2, float duration)
    {
        yield return new WaitForSeconds(cameraMovementWaitTime);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(pos1, pos2, t / duration);
            transform.rotation = Quaternion.Slerp(rot1, rot2, t / duration);
            yield return 0;
        }
        transform.position = pos2;
        transform.rotation = rot2;
    }
}
