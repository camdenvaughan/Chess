using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayerChessController : ChessGameController, IOnEventCallback
{
	protected const byte SET_GAME_STATE_EVENT_CODE = 1;

	private ChessPlayer localPlayer;
	[SerializeField]private NetworkManager networkManager;

	private void OnEnable()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	private void OnDisable()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	public void SetBoardDependency(GameObject boardObject)
	{
		board = boardObject.GetComponent<MultiPlayerBoard>();
	}
	public override bool CanPerformMove()
	{
		if (IsGameInProgress() || !IsLocalPlayersTurn())
			return false;
		return true;
	}

	private bool IsLocalPlayersTurn()
	{
		return localPlayer == activePlayer;
	}

	public void SetLocalPlayer(TeamColor team)
	{
		localPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
	}

	public override void TryToStartCurrentGame()
	{
		if (networkManager.IsRoomFull())
		{
			SetGameState(GameState.Play);
		}
	}

	protected override void SetGameState(GameState state)
	{
		object[] content = new object[] { (int)state };
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

		PhotonNetwork.RaiseEvent(SET_GAME_STATE_EVENT_CODE, content, raiseEventOptions, SendOptions.SendReliable);
	}

	public void OnEvent(EventData photonEvent)
	{
		byte eventCode = photonEvent.Code;
		if (eventCode == SET_GAME_STATE_EVENT_CODE)
		{
			object[] data = (object[])photonEvent.CustomData;
			GameState state = (GameState)data[0];
			this.state = state;
		}
	}

	protected override void HandleCamera()
	{
		return;
	}

	public override void SetUpCamera()
	{
		cameraController.SetCameraToSpin(false);
		cameraController.SetupCamera(localPlayer.team);
	}
}
