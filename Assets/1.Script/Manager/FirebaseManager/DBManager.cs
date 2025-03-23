using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;
using Firebase.Extensions;

public class DBManager : MonoBehaviour
{
    [Header("# Main Data")]
    DatabaseReference _pointer;

    // User 데이터를 표현하는 클래스 (JSON으로 변환 가능해야 함)
    [Serializable] class UserData // JsonUtility를 사용하려면 Serializable 속성이 필요
    {
        public string userId;
        public int gold;

        public UserData(string userId, int gold)
        {
            this.userId = userId;
            this.gold = gold;
        }
    }

    void Awake()
    {
        Application.logMessageReceived += (condition, stackTrace, type) =>
        {
            Console.WriteLine(condition); // 콘솔에 즉시 출력
        };
    }

    void Start()
    {
        TestDb();
    }

    public void SetDatabase()
    {
        _pointer = FirebaseDatabase.DefaultInstance.RootReference; // Realtime Database 참조 가져오기
    }

    void TestDb()
    {
        Debug.Log("DBManager에서 실행! - pointer: " + _pointer);
        // UserData 객체 생성
        UserData user = new UserData("a", 500);

        // UserData 객체를 JSON 문자열로 변환
        string json = JsonUtility.ToJson(user);

        // Realtime Database에 JSON 데이터 저장
        _pointer.Child("users").Child(user.userId).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("사용자 데이터 저장 성공!");
                    LoadUser(user.userId); // 저장 후 바로 사용자 데이터를 불러오기
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("사용자 데이터 저장 오류: " + task.Exception);
                }
            });
    }

    void LoadUser(string userId)
    {
        Debug.Log("LoadUser 실행");
        _pointer.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value != null)
                {
                    string json = snapshot.GetRawJsonValue();
                    UserData user = JsonUtility.FromJson<UserData>(json);
                    Debug.Log("사용자 ID: " + user.userId + ", 골드: " + user.gold);
                }
                else
                {
                    Debug.LogWarning("사용자를 찾을 수 없습니다!");
                }
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("사용자 데이터 로드 오류: " + task.Exception);
            }
        });
    }
}
