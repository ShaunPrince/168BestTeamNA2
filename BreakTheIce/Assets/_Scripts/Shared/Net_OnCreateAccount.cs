[System.Serializable]
public class Net_OnCreateAccount : NetMsg
{
    // Inharites from NetMsg, passes the data for a new user account
    public Net_OnCreateAccount()
    {
        OP = NetOP.OnCreateAccount;
    }

    public byte Success { set; get; }   // 255 pos values, lots of info you can set, like enums/ flags
    public string Information { set; get; }
}
