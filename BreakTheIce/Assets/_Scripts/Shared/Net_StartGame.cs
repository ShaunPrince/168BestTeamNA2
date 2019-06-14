[System.Serializable]
public class Net_StartGame : NetMsg
{
    // Inharites from NetMsg, passes the data for a new user account
    public Net_StartGame()
    {
        OP = NetOP.StartGame;
    }

    public bool started { set; get; }
}
