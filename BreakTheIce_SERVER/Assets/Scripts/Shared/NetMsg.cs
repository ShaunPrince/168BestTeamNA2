/// <summary>
/// Everything in this class will be put on the road, transfered between client and server
/// Therefore it needs to be very light weight, nothing more then it needs 
/// using binary formatter -> translate to binary reader/ writter for more efficiency
/// Never actually use NetMsg, should only ever call inheritaded versions of it
/// 
/// Cannot pass Vector3 because it is not serializable
/// only standard types
/// </summary>

// Operation code definitions
public static class NetOP
{
    public const int None = 0;

    public const int OnClientConnect = 1;   // Server responding to clients connect -> which player they are

    public const int DropPiece = 2;
    public const int OnDropPiece = 3;

}

[System.Serializable]
public abstract class NetMsg
{
    public byte OP { set; get; } // Operation Code: single number that lets you know what the message is

    public NetMsg()
    {
        OP = NetOP.None;
    }
}
