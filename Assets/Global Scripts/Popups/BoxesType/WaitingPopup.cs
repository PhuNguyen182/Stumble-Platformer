using R3;
using System;
using UnityEngine;
using GlobalScripts.Utils;
using TMPro;
using StumblePlatformer.Scripts.Common.Constants;

#if UNITASK_ADDRESSABLE_SUPPORT
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
#endif

public class WaitingPopup : MonoBehaviour
{
    [SerializeField] TMP_Text messageText;

    private Action _onTimeOut;
    private static WaitingPopup _instance;
    private IDisposable _waitDispose;

    public static bool IsPreload { get; set; }

    public static WaitingPopup Setup(bool persistant = true)
    {
        if (_instance == null)
        {
            _instance = Instantiate(Resources.Load<WaitingPopup>(CommonPopupPaths.WaitingBoxPath));
        }

        _instance.gameObject.SetActive(true);
        if (persistant)
            DontDestroyOnLoad(_instance);
        
        return _instance;
    }

#if UNITASK_ADDRESSABLE_SUPPORT
    private static AsyncOperationHandle<GameObject> _opHandle;

    public static async UniTask<WaitingPopup> SetupByAddress(string address, bool isPreload, bool persistant = false)
    {
        WaitingPopup instance = default;
        IsPreload = isPreload;
        bool isValidPath = await AddressablesUtils.IsKeyValid(address);

        if (isValidPath)
        {
            _opHandle = Addressables.LoadAssetAsync<GameObject>(address);
            await _opHandle;

            if (_opHandle.Status == AsyncOperationStatus.Succeeded)
            {
                if (SimplePool.Spawn(_opHandle.Result).TryGetComponent(out instance))
                    instance.gameObject.SetActive(true);
            }

            else Release();
        }

        return instance;
    }

    public static void Release()
    {
        if (_opHandle.IsValid())
            Addressables.Release(_opHandle);
    }
#endif

    public void ShowWaiting()
    {
        PopupController.Instance.isLockEscape = true;
        gameObject.SetActive(true);
    }


    public void HideWaiting(bool isLockEscape = false)
    {
        PopupController.Instance.isLockEscape = isLockEscape;

        gameObject.SetActive(false);

        if (_waitDispose != null)
            _waitDispose.Dispose();
    }

    public void ShowWaiting(float time)
    {
        _onTimeOut = null;
        ShowWaiting();
        TimeOut(time);
    }

    public void ShowWaiting(float time, Action action)
    {
        ShowWaiting();
        _onTimeOut = action;
        TimeOut(time);
    }

    private void TimeOut(float time)
    {
        _waitDispose?.Dispose();
        _waitDispose = Observable.Timer(TimeSpan.FromSeconds(time))
                       .Subscribe(_ =>
                       {
                            HideWaiting();
                            _onTimeOut?.Invoke();
                       });
    }

    private void OnDestroy()
    {
        _waitDispose?.Dispose();
        Release();
    }
}