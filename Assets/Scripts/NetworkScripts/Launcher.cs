using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    //客户端的版本号，只有相同的版本号可以互相连接
    public const string GAME_VERSION = "1";

    public void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("尝试连接服务端");
            PhotonNetwork.GameVersion = GAME_VERSION;
            PhotonNetwork.ConnectUsingSettings();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 连接服务端的回调方法
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("连接服务端成功！");

        PhotonNetwork.JoinLobby();
        LoginManager.instance.ShowLoginUI();
    }

    /// <summary>
    /// 加入大厅的回调方法
    /// </summary>
    public override void OnJoinedLobby()
    {
        if(SceneManager.GetActiveScene().name == "Login")
        {
            LoginManager.instance.ShowRoomList();
        }
    }

    /// <summary>
    /// 加入房间的回调方法
    /// </summary>
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public static void JoinGame(string name, string roomID)
    {
        PhotonNetwork.NickName = name;
        PhotonNetwork.JoinOrCreateRoom(roomID, new RoomOptions { MaxPlayers = 4 }, default);
    }

    /// <summary>
    /// 断连警告
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"连接已断开，原因：{cause}");
    }
}
