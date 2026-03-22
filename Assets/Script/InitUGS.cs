using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class InitUGS : MonoBehaviour
{
    async void Awake()
    {
        await UnityServices.InitializeAsync();

        // ✅ เช็คก่อน login
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Debug.Log("UGS Ready");
    }
}