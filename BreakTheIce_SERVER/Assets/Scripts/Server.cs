﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{

    private const int MAX_USER = 2; // max number of players
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
    private const int BYTE_SIZE = 1024; // max byte size of a message (~8,000 booleans)

    // Return types for network stuffs
    private byte reliableChannel;
    private int hostID;

    private bool isStarted = false;
    private byte error; // to record numeric error codes when things go wrong 

    #region Monobehaviour
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }
    private void Update()
    {
        UpdateMessagePump();
    }
    #endregion

    // server related start up
    private void Init()
    {
        NetworkTransport.Init();

        // create channel or road for the data
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);    // returns a byte that marks the type

        // blue print of server, map of the roads
        // topology def needs to be the same on the client and the server
        HostTopology topo = new HostTopology(cc, MAX_USER);

        // SERVER ONLY CODE 
        hostID = NetworkTransport.AddHost(topo, PORT, null); // IP set to null so that anyone can be server, not limited

        Debug.Log(string.Format("Opening connection on port {0} and webport {1}", PORT, WEB_PORT));
        isStarted = true;


    }
    public void Shutdown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }

    public void UpdateMessagePump()
    {
        if (!isStarted) return;

        int recHostID;      // will always be standalone in this case (which platform)
        int connectionID;   // Which user is sending me this
        int channelID;      // Which lane is he sending that message from?

        byte[] recBuffer = new byte[BYTE_SIZE];     // the actual passed data
        int datasize;   // actual size of message

        // out parameters get filled in
        NetworkEventType type = NetworkTransport.Receive(out recHostID, out connectionID, out channelID, recBuffer, BYTE_SIZE, out datasize, out error);

        switch (type)
        {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                Debug.Log(string.Format("User {0} has connected through host {1}!", connectionID, recHostID));
                // save connected player as Polar bear or Penguin, reply with what type of player connID is
                OnConnect(connectionID, channelID, recHostID);
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log(string.Format("User {0} has disconnected :(", connectionID));
                OnDisconnect(connectionID, channelID, recHostID);
                break;

            // where all the important stuff happens!
            case NetworkEventType.DataEvent:
                //Debug.Log(recBuffer[0]);
                // to receive data, you do the same thing as the client but in the reverse order
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMsg msg = (NetMsg)formatter.Deserialize(ms); // Deserialize the passed serialised msg

                // do a switch on msg to figure out what type of msg it is to parse it appropriatly
                OnData(connectionID, channelID, recHostID, msg);
                break;

            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Unexpected network event type"); // this should never be called
                break;
        }
    }
    private void OnConnect(int cnnID, int channelID, int recHostID)
    {
        // save connected player as Polar bear or Penguin, reply with what type of player connID is
        // connID player 0 will always be polar bear
        // conID player 1 will always be penguin

        int addPlayer = GameManager.Instance.addPlayer();
        if (addPlayer == 0)
        {
            // Failed to add player, game is full
            Debug.Log("Could not add Player to game");
        }
        else if (addPlayer == 1)
        {
            // Adding player succeeded
            Debug.Log(string.Format("Setting player {0} as {1}", cnnID, PlayerType.ToType(cnnID)));

            Net_OnClientConnect occ = new Net_OnClientConnect();
            occ.playerNum = cnnID;

            SendClient(recHostID, cnnID, occ);
        }
        else
        {
            Debug.Log("Error in OnConnect() in Server.cs trying to add player " + cnnID);
        }

    }

    private void OnDisconnect(int cnnID, int channelID, int recHostID)
    {
        int removePlayer = GameManager.Instance.removePlayer();
        if (removePlayer == 0)
        {
            // Failed to add player, game is full
            Debug.Log("Could not Disconnect player for who knows what reason");
        }
        else if (removePlayer == 1)
        {
            // Removing player succeeded
            Debug.Log(string.Format("Player {0}, {1}, removed from game", cnnID, PlayerType.ToType(cnnID)));
        }
        else if(removePlayer == 2)
        {
            Debug.Log(string.Format("Player {0}, {1}, removed from game\nCan start a new game!", cnnID, PlayerType.ToType(cnnID)));
        }
        else
        {
            Debug.Log("Error in OnDisconnect() in Server.cs trying to remove player " + cnnID);
        }
    }

    #region OnData
    private void OnData(int cnnID, int channelID, int recHostID, NetMsg msg)
    {
        // who sent it, which channel sent on, what platform, the actual msg
        Debug.Log("Received a msg of type " + msg.OP);

        switch (msg.OP)
        {
            case NetOP.None:
                Debug.Log("Unexpected NETOP"); // should not be sending none
                break;
            // create new case for each type of msg expected
            // not good for scaling up tho

            case NetOP.DropPiece:
                // Server basically forwards the msg to the penguin client
                SendDroppedPieceToPenguin(cnnID, channelID, recHostID, (Net_DropPiece)msg);
                break;
            case NetOP.PenguinMove:
                SendPenguinMove(cnnID, channelID, recHostID, (Net_PenguinMove)msg);
                break;
            case NetOP.StartGame:
                SendStartGameToPenguin(cnnID, channelID, recHostID, (Net_StartGame)msg);
                break;
            case NetOP.EndGame:
                SendEndGameToPolarBear(cnnID, channelID, recHostID, (Net_EndGame)msg);
                break;
        }

    }
    private void SendDroppedPieceToPenguin(int cnnID, int channelID, int recHostID, Net_DropPiece dpMsg)
    {
        // since there is only 2 clients and only client 1 (polarbear) can send this type of data, forward onto client 2 (penguin)
        // but just to be safe, make sure data is being recieved by first client
        if (cnnID == PlayerType.PolarBear)
        {
            Debug.Log(string.Format("{0} piece dropping from ({1}, 0, {2})", PieceType.ToType(dpMsg.PieceType), dpMsg.xPos, dpMsg.yPos));
            SendClient(recHostID, PlayerType.Penguin, dpMsg);
        }
        else
        {
            Debug.Log(string.Format("Should not have recieved a DropPiece msg from client {0}, error in SendDroppedPieceToPenguin in Server.cs", cnnID));
        }
        
    }
    private void SendPenguinMove(int cnnID, int channelID, int recHostID, Net_PenguinMove pmMsg)
    {
        if (cnnID == PlayerType.Penguin)
        {
            Debug.Log(string.Format("Penguin move at ({0}, {1}, {2})", pmMsg.xPos, pmMsg.yPos, pmMsg.zPos));
            SendClient(recHostID, PlayerType.PolarBear, pmMsg);
        }
        else
        {
            Debug.Log(string.Format("Should not have recieved a PemguinMove msg from client {0}, error in SendPenguinMove in Server.cs", cnnID));
        }
    }
    private void SendStartGameToPenguin(int cnnID, int channelID, int recHostID, Net_StartGame sgMsg)
    {
        // only polarbear can start game
        if (cnnID == PlayerType.PolarBear)
        {
            Debug.Log("PolarBear starting the game!");
            SendClient(recHostID, PlayerType.Penguin, sgMsg);
        }
        else
        {
            Debug.Log(string.Format("Should not have recieved a StartGame msg from client {0}, error in SendStartGameToPenguin in Server.cs", cnnID));
        }
    }
    private void SendEndGameToPolarBear(int cnnID, int channelID, int recHostID, Net_EndGame egMsg)
    {
        // penguin ends the game with its death
        if (cnnID == PlayerType.Penguin)
        {
            Debug.Log("Penguin ednign the game (with its death)");
            SendClient(recHostID, PlayerType.PolarBear, egMsg);
        }
        else
        {
            Debug.Log(string.Format("Should not have recieved an EndGame msg from client {0}, error in SendEndGameToPolarBear in Server.cs", cnnID));
        }
    }
    #endregion

    #region Send
    public void SendClient(int recHost, int cnnID, NetMsg msg)
    {
        // Need to know what platformt the client is on (web or standalone)
        // Need to know who to send to (cnnID), all msg types inherit from NetMsg ^^^

        // This is where we hold our data
        byte[] buffer = new byte[BYTE_SIZE];

        // This is where you would crush your data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        NetworkTransport.Send(hostID, cnnID, reliableChannel, buffer, BYTE_SIZE, out error);
    }
    #endregion
}
