using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class MultiPlayerBoard : Board
{
	private PhotonView photonView;

	protected override void Awake()
	{
		base.Awake();
		photonView = GetComponent<PhotonView>();
		//promotionManager = FindObjectOfType<PromotionPieceManager>().GetComponent<PromotionPieceManager>();
	}
	public void SetBoardDependencies(PromotionPieceManager pieceManager)
	{
		promotionManager = pieceManager;
	}
	public override void SelectPieceMoved(Vector2 coords)
	{
		photonView.RPC(nameof(RPC_OnSelectedPieceMoved), RpcTarget.AllBuffered, new object[] { coords });
	}
	public override void SetSelectedPiece(Vector2 coords)
	{
		photonView.RPC(nameof(RPC_OnSelectedPiece), RpcTarget.AllBuffered, new object[] { coords });

	}

	[PunRPC]
	private void RPC_OnSelectedPieceMoved(Vector2 coords)
	{
		Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
		OnSelectedPieceMoved(intCoords);
	}

	[PunRPC]
	private void RPC_OnSelectedPiece(Vector2 coords)
	{
		Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
		OnSetSelectedPiece(intCoords);
	}



	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
