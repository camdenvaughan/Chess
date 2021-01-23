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
    private PromotionPiece pickedPiece;

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

                if (hit.transform.gameObject.tag == "PromotionPiece")
                {
                    StartPiecePromotion(hit.transform.gameObject);
                }
            }
        }
    }

    private void StartPiecePromotion(GameObject piece)
    {
        pickedPiece = piece.GetComponent<PromotionPiece>();
        switch (pickedPiece.pieceName)
        {
            case "queen":
                PromoteTo(typeof(Queen));
                break;
            case "rook":
                PromoteTo(typeof(Rook));
                break;
            case "knight":
                PromoteTo(typeof(Knight));
                break;
            case "bishop":
                PromoteTo(typeof(Bishop));
                break;
            default:
                Debug.Log("other");
                break;
        }
    }


    private void PromoteTo(Type type)
    {
        board.PromotePiece(promotionPiece, type);
        DestroyPromotionScreen();
    }

    private void DestroyPromotionScreen()
    {
        promotionPiece = null;
        chessController.ResumeGameFromPromotion();
        foreach (Transform child in promotionParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
