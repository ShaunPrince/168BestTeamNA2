[System.Serializable]
public class Net_PenguinMove : NetMsg
{
    // Inharites from NetMsg, passes the data for a new user account
    public Net_PenguinMove()
    {
        OP = NetOP.PenguinMove;
    }

    public float xPos { set; get; }
    public float yPos { set; get; } // for jumping
    public float zPos { set; get; }
}
