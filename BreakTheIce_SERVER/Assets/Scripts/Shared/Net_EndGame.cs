[System.Serializable]
public class Net_EndGame : NetMsg
{
    // Inharites from NetMsg, passes the data for a new user account
    public Net_EndGame()
    {
        OP = NetOP.EndGame;
    }

    public bool ended { set; get; }
}
