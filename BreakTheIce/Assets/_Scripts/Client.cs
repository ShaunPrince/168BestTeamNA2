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
    private string SERVER_IP = "127.0.0.1"; // while on local host debuging
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
        // set server ip from lobby scene
        SERVER_IP = PassedIPvalue.Instance.Server_IP;

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
                PlayerSetUp((Net_OnClientConnect)msg);
                break;

            case NetOP.DropPiece:
                // Should only be recieving this type if client is penguin
                SpawnDroppedPiece((Net_DropPiece)msg);
                break;

            case NetOP.PenguinMove:
                UpdatePlayerMovementForPolarBear((Net_PenguinMove)msg);
                break;

            case NetOP.StartGame:
                StartGameForPenguin((Net_StartGame)msg);
                break;

        }

    }
    private void PlayerSetUp(Net_OnClientConnect playerType)
    {
        if (playerType.playerNum == PlayerType.PolarBear)
        {
            GameManager.Instance.playerType = PlayerType.PolarBear; // set player type
        }
        else if (playerType.playerNum == PlayerType.Penguin)
        {
            GameManager.Instance.playerType = PlayerType.Penguin;
        }
        else
        {
            Debug.Log("Error in PlayerSetUp() in Client.cs");
        }

        Debug.Log("Setting player to type " + PlayerType.ToType(GameManager.Instance.playerType));

        GameManager.Instance.SetCamera();   // set camera
    }
    private void SpawnDroppedPiece(Net_DropPiece dpMsg)
    {
        if (GameManager.Instance.playerType == PlayerType.Penguin)
        {
            Debug.Log(string.Format("Spawning {0} piece at ({1}, {2})", PieceType.ToType(dpMsg.PieceType), dpMsg.xPos, dpMsg.yPos));
            GameManager.Instance.UpdateDropForPenguin(dpMsg.PieceType, dpMsg.xPos, dpMsg.yPos);
        }
        else
        {
            Debug.Log(string.Format("Should not have recieved a DropPiece msg on PolarBear, error in SpawnDroppedPiece in Client.cs"));
        }
    }
    private void UpdatePlayerMovementForPolarBear(Net_PenguinMove pmMsg)
    {
        if (GameManager.Instance.playerType == PlayerType.PolarBear)
        {
            Debug.Log(string.Format("Updating penguin position to ({0}, {1}, {2})", pmMsg.xPos, pmMsg.yPos, pmMsg.zPos));
            GameManager.Instance.UpdatePenguinMovement(pmMsg.xPos, pmMsg.yPos, pmMsg.zPos);
        }
        else
        {
            Debug.Log(string.Format("Should not have recieved an update move msg on Penguin, error in UpdatePlayerMovementForPolarBear in Client.cs"));
        }
    }
    private void StartGameForPenguin(Net_StartGame sgMsg)
    {
        if (GameManager.Instance.playerType == PlayerType.Penguin)
        {
            Debug.Log("Starting the Game");
            GameManager.Instance.SetStartGame(sgMsg.started);
        }
        else
        {
            Debug.Log(string.Format("Should not have recieved a StartGame msg on PolarBear, error in StartGameForPenguin in Client.cs"));
        }
    }
    private void EndGameForPolarbear(Net_EndGame egMsg)
    {
        if (GameManager.Instance.playerType == PlayerType.PolarBear)
        {
            Debug.Log("Ending Game");
            GameManager.Instance.SetGameEnded(egMsg.ended);
        }
        else
        {
            Debug.Log(string.Format("Should not have recieved an EndGame msg on Penguin, error in EndGameForPolarbear in Client.cs"));
        }
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
    public void SendPieceDropped(PieceType.PType pieceDropped, float xPos, float yPos)
    {
        Net_DropPiece dp = new Net_DropPiece();

        dp.PieceType = pieceDropped;
        dp.xPos = xPos;
        dp.yPos = yPos;

        SendServer(dp);
    }
    public void SendPenguinMove(float xpos, float ypos, float zpos)
    {
        Net_PenguinMove pm = new Net_PenguinMove();

        pm.xPos = xpos;
        pm.yPos = ypos;
        pm.zPos = zpos;

        SendServer(pm);
    }
    public void SendGameStarted(bool started)
    {
        Net_StartGame sg = new Net_StartGame();

        sg.started = true;

        SendServer(sg);
    }
    public void SendGameEnded(bool ended)
    {
        Net_EndGame eg = new Net_EndGame();

        eg.ended = true;

        SendServer(eg);
    }
    #endregion

    #region TESTING

    #endregion
}
