using System;
using System.Linq;
using BepInEx;
using GorillaNetworking;
using GUIMenu;
using Photon.Pun;
using PlayFab.MultiplayerModels;
using UnityEngine;

namespace Mod
{
    [BepInPlugin("com.bubblestogo.roomjoiner", "Room Joiner", "1.0.0")]
    internal class Plugin : BaseUnityPlugin
  {
        public RoomJoiner roomJoiner = new RoomJoiner();

        public void Start()
        {
            GorillaTagger.OnPlayerSpawned(OnGameInit);
        }

        public void OnGameInit()
        {

        }

        public void OnGUI()
        {
            roomJoiner.Start();

        }

        public void Update()
        {

        }
    }
}

namespace GUIMenu
{
  internal class RoomJoiner
  {
    public string roomCode = "";
    private string joinResult = "Result: N/A";
    private Rect windowRect = new Rect(20, 20, 250, 180);

    public void Start()
    {
      windowRect = GUI.Window(0, windowRect, DrawWindow, "Room Joiner");
    }

    void DrawWindow(int windowID)
    {
      GUI.Label(new Rect(10, 20, 80, 20), "Room Code:");
      roomCode = GUI.TextField(new Rect(90, 20, 150, 20), roomCode);

      if (GUI.Button(new Rect(10, 50, 80, 30), "Join"))
      {
        joinResult = "Result: Waiting...";

        roomCode = roomCode.Replace(" ", "");

        if (roomCode == "")
        {
          joinResult = "Result: Must input a string of letters or numbers!";
          return;
        }

        if (roomCode.Length > 10)
        {
          joinResult = "Result: Room code too long!";
          return;
        }

        if (roomCode.All(char.IsLetterOrDigit)) {
          if (NetworkSystem.Instance.InRoom)
          {
            PhotonNetwork.Disconnect();
          }

          roomCode = roomCode.ToUpper();

          PhotonNetworkController.Instance.AttemptToJoinSpecificRoomAsync(roomCode, JoinType.Solo, handleJoinResult);
        }

        else
        {
          joinResult = "Result: Room code has invalid characters!";
        }
      }

      if (GUI.Button(new Rect(10, 90, 80, 30), "Disconnect"))
      {
        if (NetworkSystem.Instance.InRoom)
        {
          PhotonNetwork.Disconnect();

          joinResult = "Result: Succesfully disconnected!";
        }

        else
        {
          joinResult = "Result: Must be in a room!";
        }
      }

      GUI.Label(new Rect(10, 120, 230, 20), NetworkSystem.Instance.InRoom ? "Players: " + NetworkSystem.Instance.RoomPlayerCount : "Not in a room!");

      GUI.Label(new Rect(10, 150, 230, 20), joinResult);

      GUI.DragWindow();
    }
     
    private void handleJoinResult(NetJoinResult result)
    {
      switch (result)
      {
        case NetJoinResult.Success:
          joinResult = "Result: Successfully joined room!";
          break;

        case NetJoinResult.Failed_Full:
          joinResult = "Result: Failed to join room: Room is full.";
          break;

        case NetJoinResult.FallbackCreated:
          joinResult = "Result: Created a new room!";
          break;

        default: 
          joinResult = "Result: N/A = " + result.ToString();
          break;
      }
    }
  }
}

