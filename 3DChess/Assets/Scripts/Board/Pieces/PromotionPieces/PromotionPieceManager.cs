using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionPieceManager : MonoBehaviour
{
    [SerializeField] private GameObject[] promotionPieces;
    [SerializeField] private GameObject promotionParent;
    public ChessGameController chessController;
    [SerializeField]private Board board;

    [SerializeField] private Material whiteMaterial;
    [SerializeField] private Material blackMaterial;

    private Piece promotionPiece;

    public void InitAndPlacePieces(Piece piece)
    {
        promotionPiece = piece;
        Material color = piece.GetComponent<MeshRenderer>().material;
        foreach (var selectorPiece in promotionPieces)
        { 
            PromotionPiece promotionManager = selectorPiece.GetComponent<PromotionPiece>();
            promotionManager.SetMaterial(color);
            Instantiate(selectorPiece, new Vector3(promotionManager.initXValue, promotionParent.transform.position.y, promotionParent.transform.position.z), Quaternion.identity, promotionParent.transform);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {

                if (hit.transform.gameObject.CompareTag("PromotionPiece"))
                {
                    StartPiecePromotion(hit.transform.gameObject.GetComponent<PromotionPiece>());
                }
            }
        }
    }

    private void StartPiecePromotion(PromotionPiece piece)
    {
        string notation = board.XCoordToLetter(promotionPiece.occupiedSquare.x) + (promotionPiece.occupiedSquare.y + 1).ToString();
        switch (piece.pieceName)
        {
            case "queen":
                PromoteTo(typeof(Queen), notation + "Q");
                break;
            case "rook":
                PromoteTo(typeof(Rook), notation + "R");
                break;
            case "knight":
                PromoteTo(typeof(Knight), notation + "N");
                break;
            case "bishop":
                PromoteTo(typeof(Bishop), notation + "B");
                break;
            default:
                Debug.Log("other");
                break;
        }
    }


    private void PromoteTo(Type type, string notation)
    {
        board.PromotePiece(promotionPiece, type);
        DestroyPromotionScreen(notation);
    }

    private void DestroyPromotionScreen(string notation)
    {
        promotionPiece = null;
        chessController.ResumeGame();
        chessController.EndTurn(notation);
        foreach (Transform child in promotionParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
