using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
    public string UserID { get; private set; }
    public string UserName { get; private set; }
    
    public int connectCount;

    /// <summary>
    /// The unique connection identifier of this player.
    /// </summary>
    public int connectionId;

    /// <summary>
    /// True if this player is currently connected to the server; false otherwise.
    /// </summary>
    public bool isConnected;

    /// <summary>
    /// The current turn number of this player.
    /// </summary>
    public int numTurn;

    public void SetCredentials(string _userID)
    {
        UserID = _userID;
    }
    public void SetID(string _id)
    {
        UserID = _id;
    }

    public void SetUsername(string _username)
    {
        UserName = _username;
    }

}
