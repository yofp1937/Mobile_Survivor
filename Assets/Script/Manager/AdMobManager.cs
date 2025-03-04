using System.Collections;
using System.Collections.Generic;
using System.IO;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdMobManager : MonoBehaviour
{
    private string rewardAdId;
    private string testAdId = "ca-app-pub-3940256099942544/5224354917";
    RewardedAd rewardedAd;

    void Start()
    {
        rewardAdId = ReadAdmobId(); // 보상형 광고 Id 불러오기
        MobileAds.Initialize((InitializationStatus initStatus) => {}); // 광고 SDK 초기화
        LoadRewardedAd(); // 광고 갱신
    }

    // Admob 광고 Id는 공개되면 안되니 직접 읽어오는 방식으로 작성 
    private string ReadAdmobId() // 광고 Id 불러오는 메서드
    {
        // Assets/Security/Admob.txt를 path에 저장
        string path = Path.Combine(Application.dataPath, "Security", "Admob.txt");
        string rewardid = "";

        if(File.Exists(path))
        {
            // rewardid에 Admob에 써있는 rewardId 값을 저장
            rewardid = File.ReadAllText(path).Replace("rewardId=", "").Trim();
        }
        else
        {
            Debug.LogError("Admob.txt not found!");
        }

        return rewardid;
    }

    // 광고는 1시간마다 갱신시켜줘야함
    public void LoadRewardedAd() // 광고 갱신 메서드
    {
        if(rewardedAd != null) // 광고 객체 존재시 파괴
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
        
        var adRequest = new AdRequest.Builder().Build(); // 광고 데이터 생성

        RewardedAd.Load(GameManager.instance.IsDevelopMode() ? testAdId : rewardAdId , adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("광고 생성 실패 "+ error);
                return;
            }
            Debug.Log("광고 갱신 성공");
            rewardedAd = ad;
            RewardAdEventHandlers(rewardedAd);
        });
    }

    public void ShowRewardedAd() // 광고 출력 메서드
    {
        if(rewardedAd != null && rewardedAd.CanShowAd()) // 광고가 준비돼있고 보여줄수 있으면
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("광고 출력 성공");
            });
        }
        else
        {
            LoadRewardedAd(); // 광고 출력 실패시 광고 갱신
        }
    }

    // 이벤트 콜백 수신
    private void RewardAdEventHandlers(RewardedAd ad)
    {
        // 광고가 정상적으로 닫혔을때 수신
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Load RewardAdEventHandlers => OnAdFullScreenContentClosed");
            LoadRewardedAd(); // 광고 갱신
            LobbyManager.instance.mainMenu.GameSpeedUp();
        };
        // 광고가 비정상적으로 종료됐을때 수신
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log("Load RewardAdEventHandlers => OnAdFullScreenContentFailed");
            LoadRewardedAd(); // 광고 갱신
            LobbyManager.instance.mainMenu.GameSpeedReset();
        };
    }
}
