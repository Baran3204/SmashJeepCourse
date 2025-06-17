using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public static class AutheticationHandler
{
    public static AuthenticationState AuthenticationState { get; private set; }
    = AuthenticationState.NotAuthenticated;

    public static async UniTask<AuthenticationState> DoAuth(int maxTries = 5)
    {
        if (AuthenticationState == AuthenticationState.Authenticated)
        {
            return AuthenticationState;
        }

        if (AuthenticationState == AuthenticationState.Authenticating)
        {
            Debug.LogWarning("Already Authenticating");
            await Authenticating();
            return AuthenticationState;
        }
        await SignInAnonymouslyAsync(maxTries);

        return AuthenticationState;
    }

    private static async UniTask<AuthenticationState> Authenticating()
    {
        while (AuthenticationState == AuthenticationState.Authenticating || AuthenticationState == AuthenticationState.NotAuthenticated)
        {
            await UniTask.Delay(200);

        }

        return AuthenticationState;
    }

    public static async UniTask SignInAnonymouslyAsync(int maxTries)
    {
        AuthenticationState = AuthenticationState.Authenticating;

        int tries = 0;

        while (AuthenticationState == AuthenticationState.Authenticating && tries < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthenticationState = AuthenticationState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException authenticationException)
            {
                Debug.Log(authenticationException);
                AuthenticationState = AuthenticationState.Error;
            }
            catch (RequestFailedException requestFailedException)
            {
                Debug.Log(requestFailedException);
                AuthenticationState = AuthenticationState.Error;
            }


            tries++;
            await UniTask.Delay(1000);
        }

        if (AuthenticationState != AuthenticationState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in succesfully after {tries} tries");
            AuthenticationState = AuthenticationState.TimeOut;
        }
    }
}
