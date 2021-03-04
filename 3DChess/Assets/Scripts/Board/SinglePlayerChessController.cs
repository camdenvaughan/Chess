using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerChessController : ChessGameController
{
	private void Start()
	{
		StartNewGame();
	}
	public override bool CanPerformMove()
	{
		return IsGameInProgress();
	}

	public override void SetUpCamera()
	{
		cameraController.SetCameraToSpin(false);
	}

	public override void TryToStartCurrentGame()
	{
		SetGameState(GameState.Play);
	}

	protected override void HandleCamera()
	{
		if (PlayerPrefs.GetInt("isCameraFlipOn") == 0)
			cameraController.MoveCamera();
	}

	protected override void SetGameState(GameState state)
	{
		this.state = state;
	}
}
