using UnityEngine;
using UnityEngine.UI;
using GlobalScripts.Utils;
using TMPro;

#if UNITASK_ADDRESSABLE_SUPPORT
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
#endif

public class MessagePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button closeButton;

    private static MessagePopup _instance;
    public static bool IsPreload { get; set; }
    public const string MessagePopupPath = "Popups/Message Popup";

    private void Awake()
    {
        closeButton.onClick.AddListener(() => HideWaiting(false));
    }

    public static MessagePopup Setup(bool persistant = true)
    {
        if (_instance == null)
        {
            _instance = Instantiate(Resources.Load<MessagePopup>(MessagePopupPath));
        }

        _instance.gameObject.SetActive(true);
        if (persistant)
            DontDestroyOnLoad(_instance);

        return _instance;
    }

    public MessagePopup SetMessage(string message)
    {
        messageText.text = message;
        return _instance;
    }

    public MessagePopup ShowCloseButton(bool active = false)
    {
        closeButton.gameObject.SetActive(active);
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
                instance = SimplePool.Spawn(_opHandle.Result).GetComponent<WaitingPopup>();
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

    public MessagePopup ShowWaiting()
    {
        PopupController.Instance.isLockEscape = true;
        gameObject.SetActive(true);
        return _instance;
    }


    public MessagePopup HideWaiting(bool isLockEscape = false)
    {
        PopupController.Instance.isLockEscape = isLockEscape;
        gameObject.SetActive(false);
        return _instance;
    }

    private void OnDestroy()
    {
        Release();
    }
}
