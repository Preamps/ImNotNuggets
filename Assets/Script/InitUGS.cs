using Unity.Services.Analytics;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class InitUGS : MonoBehaviour
{
    public static bool IsInitialized { get; private set; } = false;

    async void Awake()
    {
        try
        {
            // 1. เริ่มระบบ Services
            await UnityServices.InitializeAsync();

            // 2. แอบ Login เงียบๆ (เพื่อให้มี ID สำหรับส่ง Analytics)
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            // 3. เริ่มเก็บข้อมูล
            AnalyticsService.Instance.StartDataCollection();

            IsInitialized = true;
            Debug.Log("<color=green>UGS & Analytics Ready!</color>");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Init Failed: " + e.Message);
            // กรณี Error ก็ปล่อยให้เกมไปต่อได้ แต่อาจไม่มีข้อมูลส่งขึ้น Cloud
            IsInitialized = true;
        }
    }
}
