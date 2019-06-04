[System.Serializable]
public class Net_DropPiece : NetMsg
{
    // Inharites from NetMsg, passes the data for a new user account
    public Net_DropPiece()
    {
        OP = NetOP.DropPiece;
    }

    public int PeiceType { set; get; }
    public float xPos { set; get; }
    public float yPos { set; get; }
}
