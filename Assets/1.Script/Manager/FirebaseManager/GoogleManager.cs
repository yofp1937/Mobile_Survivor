using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using Google;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GoogleManager : MonoBehaviour
{
    [Header("# Main Data")]
    FirebaseApp _firebaseApp;
    FirebaseAuth _firebaseAuth;

    [Header("# Reference Data")]
    public DBManager DBManager;

    void Awake()
    {
        PlayGamesPlatform.Instance.Authenticate(OnPlayGamesAuthentication); // 게임 실행시 구글플레이게임즈 자동 로그인
    }

    public void InitFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == DependencyStatus.Available) {
            _firebaseApp = FirebaseApp.DefaultInstance;
            _firebaseAuth = FirebaseAuth.DefaultInstance;
        } else {
            Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        }
        });
        DBManager.SetDatabase();
    }

    public void LoginPlayGames() // PlayGames 로그인 시도
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(OnPlayGamesAuthentication);
    }

    internal void OnPlayGamesAuthentication(SignInStatus status) // 로그인 진행
    {
        if (status == SignInStatus.Success)
        {
            LobbyManager.instance.TestText.text = "로그인 성공" + "\n" + PlayGamesPlatform.Instance.GetUserId() + "\n" + PlayGamesPlatform.Instance.GetUserDisplayName();
            LobbyManager.instance.LoginBtn.gameObject.SetActive(false);
            PlayGamesPlatform.Instance.RequestServerSideAccess(false, async authCode => { // Firebase에 접속하기위한 토큰 받아오기
                LobbyManager.instance.TestText.text += "\nCode: " + authCode;
                await GameManager.instance.GoogleManager.OnPlayGamesAuthenticatedFinishied(authCode); // 받아온 토큰으로 Firebase 인증 요청
            });
        }
        else
        {
            // Play Games Services와의 연동을 비활성화하거나, 로그인 버튼을 표시합니다.
            // 로그인 버튼을 클릭하면 다음 함수를 호출해야 합니다:
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
            LobbyManager.instance.TestText.text = "자동 로그인 실패";
            LobbyManager.instance.LoginBtn.gameObject.SetActive(true);
        }
    }

    async Task OnPlayGamesAuthenticatedFinishied(string authCode) // Firebase로 token 전송하여 인증 처리
    {
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
        try
        {
            FirebaseUser newUser = await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential);
            LobbyManager.instance.TestText.text += "\n" + $"Firebase 로그인 성공! UID: {newUser.UserId}";
        }
        catch (System.Exception ex)
        {
            LobbyManager.instance.TestText.text += "\n" + $"Firebase 로그인 실패: {ex.Message}";
        }
    }
}
