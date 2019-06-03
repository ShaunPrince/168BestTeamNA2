using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    public static Client Instance { get; private set; } // to access the client object outside this script

    private const int MAX_USER = 2; // max number of players
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
    private const string SERVER_IP = "127.0.0.1"; // while on local host debuging
    private const int BYTE_SIZE = 1024;

    // Return types for network stuffs
    private byte reliableChannel;
    private int connectionId;
    private int hostID;     // client only has one host, will only be connecting to either stand alone server or web
    private byte error;     // when something goes wrong

    private bool isStarted = false;

    #region Monobehaviour
    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);  // super important cuz client changes scenes
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

        // CLIENT ONLY CODE 
        hostID = NetworkTransport.AddHost(topo, 0); // port 0, nobody supposed to connect to us (no peer to peer)

        // Standalone Client
        connectionId = NetworkTransport.Connect(hostID, SERVER_IP, PORT, 0, out error);
        Debug.Log("Connecting from standalone");

        Debug.Log(string.Format("Attempting to connect on {0}...", SERVER_IP));
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

        int recHostID;      // always going to be 1 is this case
        int connectionID;   // always 1, connecting to say "person", the server
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
                Debug.Log("We have connected to the server!");
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log("We have been disconnected");
                break;

            // where all the important stuff happens!
            case NetworkEventType.DataEvent:
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMsg msg = (NetMsg)formatter.Deserialize(ms); // Deserialize the passed serialised msg

                // do a switch on msg to figure out what type of msg it is to parse it appropriatly
                // don't really need to send connectionID, channelID, recHostID since client already knows all this info about themself
                // no loss in keeping it because it is client code, not bandwidth code or anything
                OnData(connectionID, channelID, recHostID, msg);
                break;

            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Unexpected network event type"); // this should never be called
                break;
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

            case NetOP.OnClientConnect:
                SetPlayerType((Net_OnClientConnect)msg);
                break;

        }

    }
    private void SetPlayerType(Net_OnClientConnect playerType)
    {
        Debug.Log("Setting player to type " + PlayerType.ToType(playerType.playerNum));

        if (playerType.playerNum == 0)
            GameManager.Instance.playerType = PlayerType.PolarBear;
        else if (playerType.playerNum == 1)
            GameManager.Instance.playerType = PlayerType.Penguin;
    }
    #endregion

    #region Send
    public void SendServer(NetMsg msg)
    {
        // All msg types inherit from NetMsg ^^^

        // This is where we hold our data
        byte[] buffer = new byte[BYTE_SIZE];

        // This is where you would crush your data into a byte[]
        //buffer[0] = 255;  // Testing
        BinaryFormatter formatter = new BinaryFormatter(); // before you can write to it, you must create a stream (most effic, mem stream)
        MemoryStream ms = new MemoryStream(buffer); // initialized with buffer since that is the largest that can be written
        formatter.Serialize(ms, msg);   // serialize our message into the memory stream
                                        // works because everything inside of msg is serializable (base class NetMsg is serializable)

        // Send the data, connection type, user, channel type, data, size, error
        NetworkTransport.Send(hostID, connectionId, reliableChannel, buffer, BYTE_SIZE, out error);
    }
    #endregion

    #region TESTING

    #endregion
}
