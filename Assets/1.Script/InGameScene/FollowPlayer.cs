using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float offsetZ = -10f;

    void Update()
    {
        GameObject player = InGameManager.instance.Player.transform.Find("character").gameObject;

        if(player != null)
        {
            Vector3 newPosition = player.transform.position;
            newPosition.z = offsetZ;
            transform.position = newPosition;
        }

        // Test Code
        if(Input.GetButtonDown("Jump") && GameManager.instance.IsDeveloperMode)
        {
            InGameManager.instance.Player.LevelUp();
        }

        // Test Code - 숫자패드 + 누르면 300초 증가
        if(Input.GetKeyDown(KeyCode.KeypadPlus) && GameManager.instance.IsDeveloperMode)
        {
            GameManager.instance.GameTime += 300;
        }

        // Esc 누르면 일시정지하고 설정창 띄움
        if(Input.GetKeyDown(KeyCode.Escape) && !InGameManager.instance.OnLevelUp && InGameManager.instance.living)
        {
            if(InGameManager.instance.OnSettings) // 설정창이 열려있으면
            {
                InGameManager.instance.SettingPanel.OnClickExit();
            }
            else
            {
                InGameManager.instance.SettingPanel.OnClickSettingPanel();
            }
        }
    }
}
