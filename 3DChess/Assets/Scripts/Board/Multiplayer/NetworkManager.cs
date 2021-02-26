using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	private const string LEVEL = "level";
	private const int MAX_PLAYERS = 2;
	private const string TEAM = "team";
	private UINavigator navigatorUI;

	private ChessLevel playerLevel;

	public void SetUIDependencies(UINavigator navigator)
	{
		navigatorUI = navigator;
		
	}

	private void Awake()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	private void Update()
	{
		if (navigatorUI)
			navigatorUI.SetConnectionText(PhotonNetwork.NetworkClientState.ToString());

	}
public void Connect()
	{
		if (PhotonNetwork.IsConnected)
		{
			Debug.LogError($"Connected to server. Looking for random room with level {playerLevel}");
			PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }, MAX_PLAYERS);
		}
		else
		{
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	#region Photon Callbacks
	public override void OnConnectedToMaster()
	{
		Debug.LogError($"Connected to server. Looking for random room with level {playerLevel}");
		PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }, MAX_PLAYERS);

	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.LogError($"Joining random room failed because of {message}. Creating a new one with player level {playerLevel}.");
		PhotonNetwork.CreateRoom(null, new RoomOptions
		{
			CustomRoomPropertiesForLobby = new string[] { LEVEL },
			MaxPlayers = MAX_PLAYERS,
			CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }
		});
	}

	public override void OnJoinedRoom()
	{
		Debug.LogError($"Player {PhotonNetwork.LocalPlayer.ActorNumber} joined the room with level {(ChessLevel)PhotonNetwork.CurrentRoom.CustomProperties[LEVEL]}");
		PrepareTeamSelectionOptions();
		navigatorUI.SetJoinedRoomUI();
	}

	private void PrepareTeamSelectionOptions()
	{
		if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
		{
			var firstPlayer = PhotonNetwork.CurrentRoom.GetPlayer(1);
			if (firstPlayer.CustomProperties.ContainsKey(TEAM))
			{
				var occupiedTeam = firstPlayer.CustomProperties[TEAM];
				navigatorUI.RestrictTeamChoice((TeamColor)occupiedTeam);
			}
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Debug.LogError($"Player {newPlayer.ActorNumber} joined the room");

	}

	public void SetPlayerLevel(ChessLevel level)
	{
		playerLevel = level;
		PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { LEVEL, level } });
	}

	internal void SelectTeam(int team)
	{
		PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { TEAM, team } });
	}


	#endregion
}
