using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class InitUGS : MonoBehaviour
{
    public static bool IsInitialized { get; private set; } = false;
    private static InitUGS instance;

    async void Awake()
    {
        // 1. ป้องกันการมีสคริปต์ซ้ำใน Scene
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject); // อยู่ยาวๆ จนจบเกม

        try
        {
            // 2. เช็คว่า Init หรือยัง
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                InitializationOptions options = new InitializationOptions();
                options.SetEnvironmentName("production");
                await UnityServices.InitializeAsync(options);
            }

            // 3. เช็คก่อน Login ว่าล็อกอินค้างไว้หรือยัง (แก้ Error ที่คุณเจอ)
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("UGS: Signed in anonymously.");
            }
            else
            {
                Debug.Log("UGS: Already signed in.");
            }

            // 4. ตั้งค่าสถานะเป็น True เพื่อให้ WaveManager ทำงานต่อได้
            IsInitialized = true;
            Debug.Log("<color=green>UGS Ready for Production!</color>");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Init Failed: " + e.Message);
            // กรณี Error ก็ควรปล่อยให้เกมไปต่อได้ (แต่อาจจะไม่มี Analytics)
            IsInitialized = true;
        }
    }
}
