using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Vivox;
using System;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class VivoxPlayer : MonoBehaviour
{
    [SerializeField]
    public string VoiceChannelName = "Meeting";


     void Start()
    {
       
        
        
    }

    async void InitializeAsync()
    {
        await UnityServices.InitializeAsync();
       // await AuthenticationService.Instance.SignInAnonymouslyAsync();

        await VivoxService.Instance.InitializeAsync();

        VivoxService.Instance.LoggedIn += OnUserLoggedIn;
        VivoxService.Instance.LoggedOut += OnUserLoggedOut;
    }

    public async void LoginToVivoxAsync()
    {
        InitializeAsync();
        
        LoginOptions options = new LoginOptions();
        options.DisplayName = null;
        Debug.Log("DisplayName: "+ options.DisplayName);
        options.EnableTTS = true;
        await VivoxService.Instance.LoginAsync();
        JoinEchoChannelAsync();

    }

    public async void JoinEchoChannelAsync()
    {
        string channelToJoin = "Lobby";
        await VivoxService.Instance.JoinEchoChannelAsync(channelToJoin, ChatCapability.TextAndAudio);
    }


    private async void SendMessageAsync(string channelName="Lobby", string message="Echo channel works")
    {
        VivoxService.Instance.SendChannelTextMessageAsync(channelName, message);
    }

    private void BindSessionEvents(bool doBind)
    {
        VivoxService.Instance.ChannelMessageReceived += onChannelMessageReceived;
    }

    private void onChannelMessageReceived(VivoxMessage message)
    {
        string messageText = message.MessageText;
        string senderID = message.SenderPlayerId;
        string senderDisplayName = message.SenderDisplayName;
        string messageChannel = message.ChannelName;
        Debug.Log("Message back is: "+message.ToString());
    }

    void OnUserLoggedIn()
    {
        if (VivoxService.Instance.IsLoggedIn)
        {
            Debug.Log("Successfully connected to Vivox");
            Debug.Log("Joining voice channel: " + VoiceChannelName);
            VivoxService.Instance.JoinGroupChannelAsync(VoiceChannelName, ChatCapability.TextAndAudio);
        }
        else
        {
            Debug.Log("Cannot sing into Vivox, check your credentials and token settings");
        }
    }

    void OnUserLoggedOut()
    {
        Debug.Log("Disconnecting from voice channel " + VoiceChannelName);
        VivoxService.Instance.LeaveAllChannelsAsync();
        Debug.Log("Disconnecting from Vivox");
        VivoxService.Instance.LogoutAsync();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
