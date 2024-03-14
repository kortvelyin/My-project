using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine.UI;

public class authManager : MonoBehaviour
{
    [HideInInspector] public Text logTxt;

    async void Start()
    {
        await UnityServices.InitializeAsync();
    }

    void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
        };

        AuthenticationService.Instance.SignInFailed += (err) => {
            Debug.LogError(err);
        };


        AuthenticationService.Instance.SignedOut += () => {
            Debug.Log("Player signed out.");
        };


        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };

    }


    public async void SignIn()
    {
        await signInAnonymous();
    }

    async Task signInAnonymous()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            print("Sign in Success");
            print("Player Id:" + AuthenticationService.Instance.PlayerId);
           // logTxt.text = "Player id:" + AuthenticationService.Instance.PlayerId;
        }
        catch (AuthenticationException ex)
        {
            print("Sign in failed!!");
            Debug.LogException(ex);
        }

    }
}

    