[System.Serializable]
public class Net_CreateAccount : NetMsg
{
    // Inharites from NetMsg, passes the data for a new user account
    public Net_CreateAccount()
    {
        OP = NetOP.CreateAccount;
    }

    public string Username { set; get; }
    public string Password { set; get; }
    public string Email { set; get; }
}
