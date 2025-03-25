using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Auth;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GoogleManager : MonoBehaviour
{
    #region Singleton
    public static GoogleManager instance;
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

    void Init() // Awake에서 실행
    {
        PlayGamesPlatform.Instance.Authenticate(OnPlayGamesAuthentication); // 게임 실행시 구글플레이게임즈 자동 로그인
    }

    public string GetUserId() { return PlayGamesPlatform.Instance.GetUserId(); } // PlayGames UserId 반환

    public void LoginPlayGames() // PlayGames 로그인 시도
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(OnPlayGamesAuthentication);
    }

    internal void OnPlayGamesAuthentication(SignInStatus status) // 로그인 진행
    {
        if (status == SignInStatus.Success)
        {
            LobbyManager.instance.LoginBtn.gameObject.SetActive(false);
            DBManager.instance.LoginGooglePlay();
            LobbyManager.instance.ShowLoadingPanel(3f);
            PlayGamesPlatform.Instance.RequestServerSideAccess(false, async authCode => { // Firebase에 접속하기위한 토큰 받아오기
            await GoogleManager.instance.OnPlayGamesAuthenticatedFinishied(authCode); // 받아온 토큰으로 Firebase 인증 요청
            });
        }
        else
        {
            // Play Games Services와의 연동을 비활성화하거나, 로그인 버튼을 표시합니다.
            // 로그인 버튼을 클릭하면 다음 함수를 호출해야 합니다:
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
            LobbyManager.instance.LoginBtn.gameObject.SetActive(true);
        }
    }

    async Task OnPlayGamesAuthenticatedFinishied(string authCode) // Firebase로 token 전송하여 인증 처리
    {
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
        try
        {
            FirebaseUser user = await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential);
            // 이 이후에 필드변수에 값 할당하려했는데 null만 넘어감
        }
        catch (System.Exception ex)
        {
            Debug.Log($"Firebase 로그인 실패: {ex.Message}");
        }
    }
}
