using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;
using Firebase.Extensions;
using Newtonsoft.Json;
using Castle.Core;
using Google.MiniJSON;

public class DBManager : MonoBehaviour
{
    #region Singleton
    public static DBManager instance;
    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    #endregion

    [Header("# Main Data")]
    string _uid;
    DatabaseReference _pointer;

    [Header("# Reference Data")]
    [SerializeField] GoogleManager GoogleManager;

    void Start()
    {
        SetUserData();
    }

    void SetUserData() // 게임 실행시 기본 유저정보 불러오기
    {
        _pointer = FirebaseDatabase.DefaultInstance.RootReference; // Realtime Database 참조 가져오기
        CheckRegistered();
        LoadUserData();
        LoadEquips();
    }

    void CheckRegistered() // DB에 등록돼있는 ID인지 검사
    {
        // DB에 접근할 Id 읽어오기
        _uid = GoogleManager.GetUserId(); // 플레이게임즈 로그인 돼있으면 해당 ID로 DB 접근
        if(_uid == "0" || string.IsNullOrEmpty(_uid)) _uid = GetGUID(); // 로그인 안돼있으면 발급받은 개인 GUID로 DB 접근

        _pointer.Child("UserRegistry").Child(_uid).GetValueAsync().ContinueWithOnMainThread(task => // UserRegistry에 내 id 존재하는지 확인
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Debug.Log(_uid + "님은 UserRegistry에 등록된 사용자입니다.");
                }
                else
                {
                    RegisterUser();
                }
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("UserRegistry 로드 오류: " + task.Exception);
            }
        });
    }

    string GetGUID() // 유저의 고유 식별자 가져옴
    {
        string id = PlayerPrefs.GetString("GUID", null);

        if(string.IsNullOrEmpty(id))
        {
            id = Guid.NewGuid().ToString(); // 고유 식별자 생성
            PlayerPrefs.SetString("GUID", id);
            PlayerPrefs.Save();
        }
        
        return id;
    }

    void RegisterUser() // UserRegistry에 새 유저 등록
    {
        _pointer.Child("UserRegistry").Child(_uid).SetValueAsync(true).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log(_uid + "님 UserRegistry에 사용자 등록 완료");
                CreateUserData();
            }
            else
            {
                Debug.LogError("UserRegistry 등록 실패: " + task.Exception);
            }
        });
    }

    void CreateUserData() // UserRegistry에 등록 후 UserData에도 데이터 생성
    {
        // Gold, SlotCnt 초기 생성
        CreateUserBaseData();

        // Upgrade 초기 생성
        CreateUserUpgradeData();
    }

    public void CreateUserBaseData() // Gold, SlotCnt 생성
    {
        UserData userData = new UserData(0, 15, 0);
        _pointer.Child("UserData").Child(_uid).SetRawJsonValueAsync(JsonUtility.ToJson(userData)).ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                Debug.Log(_uid + "님 UserData 생성 완료");
            }
            else
            {
                Debug.LogError("UserData 생성 실패: " + task.Exception);
            }
        });
        GameManager.instance.Gold = 0;
        LobbyManager.instance.LoadHaveGold();
        GameManager.instance.InventoryManager.SlotCnt = 15;
        LobbyManager.instance.InventoryPanel.Init();
    }

    public void CreateUserUpgradeData() // Upgrade Data 생성
    {
        UpgradeLevelData upgradeData = new UpgradeLevelData();
        upgradeData.ResetData();
        string json = JsonConvert.SerializeObject(upgradeData.dict);
        _pointer.Child("UserData").Child(_uid).Child("UpgradeLevels").SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => 
        {
            if(task.IsCompleted)
            {
                Debug.Log(_uid + "님 UpgradeLevels 생성 완료");
                GameManager.instance.StatusManager.LoadUpgradeLevelInDB(upgradeData);
            }
            else
            {
                Debug.Log("UpgradeLevels 생성 실패: " + task.Exception);
            }
        });
    }

    void LoadUserData()
    {
        // Gold, SlotCnt
        _pointer.Child("UserData").Child(_uid).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if(snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();

                    UserData userData = JsonUtility.FromJson<UserData>(json);

                    GameManager.instance.Gold = userData.Gold;
                    GameManager.instance.InventoryManager.SlotCnt = userData.SlotCnt;
                    LobbyManager.instance.InventoryPanel.Init();
                    GameManager.instance.StatusManager.UpgradeCost = userData.UpgradeCost;
                }
            }
            else
            {
                Debug.LogError("LoadUserData - 데이터를 찾을수 없습니다: " + task.Exception);
            }
        });

        // Upgrade 불러오기
        _pointer.Child("UserData").Child(_uid).Child("UpgradeLevels").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if(snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    UpgradeLevelData upgradeData = new UpgradeLevelData();
                    upgradeData.dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    GameManager.instance.StatusManager.LoadUpgradeLevelInDB(upgradeData);
                }
            }
        });
    }

    public void UpdateUserData(string type, int value) // UserData._uid 하위의 type값 value로 업데이트
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data[type] = value;

        _pointer.Child("UserData").Child(_uid).UpdateChildrenAsync(data).ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                Debug.Log($"{type}값이 {value}로 업데이트됨");
            }
            else
            {
                Debug.LogError("UserData 업데이트 실패" + task.Exception);
            }
        });
    }

    public void UpdateUpgradeDataInUserData(Dictionary<string, object> dict) // UpgradeLevels의 한 컬럼 값만 수정
    {
        _pointer.Child("UserData").Child(_uid).Child("UpgradeLevels").UpdateChildrenAsync(dict).ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                Debug.Log($"{dict.Keys}값이 {dict.Values}로 업데이트됨");
            }
            else
            {
                Debug.LogError("UserData 업데이트 실패" + task.Exception);
            }
        });
    }

    public void SettingUpgradeDataInUserData(string json) // 모든 UpgradeLevels 데이터 수정
    {
        _pointer.Child("UserData").Child(_uid).Child("UpgradeLevels").SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                Debug.Log("UpgradeLevels 초기화됨");
            }
            else
            {
                Debug.LogError("UserData 업데이트 실패" + task.Exception);
            }
        });;
    }

    public void LoginGooglePlay() // GUID -> PlayGames.UserId로 데이터 이전
    {
        string guid = PlayerPrefs.GetString("GUID");
        string playGamesId = GoogleManager.instance.GetUserId();
        
        _pointer.Child("UserData").Child(playGamesId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result.Exists)
            {
                return; // 이미 존재하면 이후 코드 실행하지 않음
            }
            CopyUserData(guid);
        });
    }

    void CopyUserData(string guid)
    {
        string playGamesId = GoogleManager.instance.GetUserId();
        // UserRegistry의 GUID 데이터를 로그인한 PlayGames ID로 복사
        _pointer.Child("UserRegistry").Child(guid).GetValueAsync().ContinueWithOnMainThread(task => 
        {
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if(snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    _pointer.Child("UserRegistry").Child(playGamesId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
                        if(task.IsCompleted)
                        {
                            _pointer.Child("UserRegistry").Child(guid).RemoveValueAsync();
                            Debug.Log("UserRegistry 이전 성공, 이전 데이터 삭제도 성공");
                        }
                    });;
                    Debug.Log("UserRegistry 이전 완료");
                }
            }
        });

        // UpgradeLevels 복사
        _pointer.Child("UserData").Child(guid).Child("UpgradeLevels").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if(snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    _pointer.Child("UserData").Child(playGamesId).Child("UpgradeLevels").SetRawJsonValueAsync(json);
                }
            }
        });

        // Equips 복사
        _pointer.Child("UserData").Child(guid).Child("Equips").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if(snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    _pointer.Child("UserData").Child(playGamesId).Child("Equips").SetRawJsonValueAsync(json);
                }
            }
        });

        // UserData의 GUID 데이터를 로그인한 PlayGames ID로 복사
        _pointer.Child("UserData").Child(guid).GetValueAsync().ContinueWithOnMainThread(task => 
        {
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if(snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    _pointer.Child("UserData").Child(playGamesId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
                        if(task.IsCompleted)
                        {
                            _pointer.Child("UserData").Child(guid).RemoveValueAsync();
                            Debug.Log("UserData 이전 성공, 이전 데이터 삭제도 성공");
                        }
                    });
                }
            }
        });
        // _uid 재설정
        _uid = GoogleManager.instance.GetUserId();
    }

    public void CreateEquipInDB(string GUID, string json) // 아이템 생성시 DB에 저장
    {
        _pointer.Child("UserData").Child(_uid).Child("Equips").Child(GUID).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                Debug.Log("DB에 아이템 저장 완료 - " + GUID);
            }
            else
            {
                Debug.LogError("DB에 아이템 저장 실패");
            }
        });
    }

    public void DeleteEquipInDB(string GUID) // 아이템 판매시 DB에서 제거
    {
        _pointer.Child("UserData").Child(_uid).Child("Equips").Child(GUID).RemoveValueAsync();
    }

    public void UpdateEquipInDB(string GUID, Dictionary<string, object> tuple) // 아이템 강화 또는 착용시 데이터 수정
    {
        _pointer.Child("UserData").Child(_uid).Child("Equips").Child(GUID).UpdateChildrenAsync(tuple).ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                Debug.Log("아이템 정보 수정 완료 - " + GUID);
            }
            else
            {
                Debug.LogError("아이템 정보 수정 실패");
            }
        });
    }

    void LoadEquips() // 장비 불러오기
    {
        _pointer.Child("UserData").Child(_uid).Child("Equips").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if(snapshot.Exists)
                {
                    Debug.Log("장비 불러오기 성공");
                    foreach(DataSnapshot snap in snapshot.Children)
                    {
                        string json = snap.GetRawJsonValue();
                        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                        GameObject equip = new GameObject();
                        equip.transform.parent = GameManager.instance.InventoryManager.transform;
                        equip.AddComponent<Equipment>().SetInfo(data);
                        // GUID 설정
                        equip.GetComponent<Equipment>().GUID = snap.Key;
                    }
                    GameManager.instance.InventoryManager.SortEquipment();
                }
            }
            else
            {
                Debug.LogError("LoadEquips 실패: " + task.Exception);
            }
        });
    }
}
