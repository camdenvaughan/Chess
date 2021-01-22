using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionPieceCreator : MonoBehaviour
{
    [SerializeField] private GameObject[] promotionPieces;
    [SerializeField] private GameObject promotionParent;
    public ChessGameController chessController;
    private Board board;

    [SerializeField] private Material whiteMaterial;
    [SerializeField] private Material blackMaterial;

    private Piece promotionPiece;
    private PromotionPieceManager pickedPiece;

    public void InitAndPlacePieces()
    {
        foreach (var piece in promotionPieces)
        {
            PromotionPieceManager promotionManager = piece.GetComponent<PromotionPieceManager>();
            Instantiate(piece, new Vector3(promotionManager.initXValue, 1f, 0f), Quaternion.identity, promotionParent.transform);
            promotionManager.SetMaterial(GetMaterial());

        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {

                if (hit.transform.gameObject.tag == "PromotionPiece")
                {
                    StartPiecePromotion(hit.transform.gameObject);
                }
            }
        }
    }

    private void StartPiecePromotion(GameObject piece)
    {
        pickedPiece = piece.GetComponent<PromotionPieceManager>();
        switch (pickedPiece.pieceName)
        {
            case "queen":
                Debug.Log("queen");
                board.PromotePiece(promotionPiece, typeof(Queen));
                break;
            case "Rook":
                Debug.Log("rook");
                break;
            default:
                Debug.Log("other");
                break;
        }
    }

    private Material GetMaterial()
    {
        if (chessController.IsActiveTeamWhite())
            return whiteMaterial;
        else
            return blackMaterial;
    }

    public void StorePromotionPiece(Piece piece)
    {
        promotionPiece = piece;
        //board.PromotePiece(piece, promotionPiece.GetType());
    }

}
