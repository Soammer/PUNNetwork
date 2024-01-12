using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    [Header("UI组件")]
    public Button StartBtn;
    public Button ExitBtn;

    public Transform MessageContentTranform;

    [Header("挂载物体")]
    public TMP_Text MessageTextPrefab;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        StartBtn.onClick.AddListener(() =>
        {
            StartBtn.gameObject.SetActive(false);
            PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
        });
        ExitBtn.onClick.AddListener(() =>
        {
            LeaveRoom();
        });
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ShowMessage($"{newPlayer.NickName}加入了房间");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ShowMessage($"{otherPlayer.NickName}离开了房间");
    }

    /// <summary>
    /// 离开房间的回调
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Login");
        instance = null;
    }

    public void LeaveRoom()
    {
        Camera.main.GetComponent<CameraController>().Disable();
        //LeaveRoom方法会触发OnLeftRoom回调，并且在进入新的场景后会再次调用OnConnectedToMaster方法
        PhotonNetwork.LeaveRoom();
    }

    public void ShowMessage(string msg)
    {
        Instantiate(MessageTextPrefab, MessageContentTranform).text = msg;
    }
}
