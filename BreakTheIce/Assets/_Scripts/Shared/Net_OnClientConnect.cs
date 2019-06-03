[System.Serializable]
public class Net_OnClientConnect : NetMsg
{
    // Inharites from NetMsg, passes the data for a new user account
    public Net_OnClientConnect()
    {
        OP = NetOP.OnClientConnect;
    }

    public int playerNum { set; get; } // 0 = polar bear, 1 = penguin
}
