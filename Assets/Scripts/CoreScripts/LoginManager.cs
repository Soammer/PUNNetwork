using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviourPunCallbacks
{
    public static LoginManager instance;

    [Header("UI组件")]
    public TMP_InputField usernameField;
    public TMP_InputField roomIDField;
    public GameObject RoomListGo;

    public Transform gridLayout;
    public Button Startbtn;
    public TMP_Text MessageText;

    [Header("挂载资源")]
    public GameObject RoomNameBtnPrefab;

    //逻辑存储
    public Dictionary<string, GameObject> RoomNameDic = new();

    public void Start()
    {
        instance = this;
        Startbtn.onClick.AddListener(() =>
        {
            if(usernameField.text == string.Empty)
            {
                MessageText.text += "\n用户名不可为空！";
                return;
            }
            if(roomIDField.text.Length < 2 || roomIDField.text.Length > 8)
            {
                MessageText.text += "\n房间号过长或过短！";
                return;
            }
            instance = null;
            Launcher.JoinGame(usernameField.text, roomIDField.text);
        });
#if UNITY_EDITOR
        usernameField.text = "Test";
        roomIDField.text = "Test";
        Debug.Log("自动填入成功");
#endif
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //不能遍历过程中删除元素，通过缓存记录需要删除的元素
        List<RoomInfo> BeRemoveList = new();
        foreach(var room in roomList)
        {
            if (RoomNameDic.TryGetValue(room.Name, out GameObject roomBtn))
            {
                if(room.PlayerCount == 0)
                {
                    Destroy(roomBtn);
                    RoomNameDic.Remove(room.Name);
                    BeRemoveList.Add(room);
                }
                else
                {
                    roomBtn.GetComponentInChildren<TMP_Text>().text = $"{room.Name}({room.PlayerCount})";
                }
            }
            else
            {
                if(room.PlayerCount != 0)
                {
                    GameObject newRoomBtn = Instantiate(RoomNameBtnPrefab, gridLayout);
                    string roomName = room.Name;
                    newRoomBtn.GetComponentInChildren<TMP_Text>().text = $"{roomName}({room.PlayerCount})";
                    newRoomBtn.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        roomIDField.text = roomName;
                    });
                    RoomNameDic.Add(roomName, newRoomBtn);
                }
            }
        }
        foreach(var room in BeRemoveList)
        {
            roomList.Remove(room);
        }
    }

    public void ShowLoginUI()
    {
        transform.Find("LoginUI").gameObject.SetActive(true);
        MessageText.text += "\n连接成功！";
    }

    public void ShowRoomList()
    {
        MessageText.text += "\n加入全局大厅成功！";
        RoomListGo.SetActive(true);
    }
}
