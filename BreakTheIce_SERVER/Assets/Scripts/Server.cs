using System.IO;
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
