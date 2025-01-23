#if UNITY_EDITOR && ODIN_INSPECTOR
using UnityEngine;
using UnityEditor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;

public class AssetsFinderWindow : OdinEditorWindow
{
    [Header("Find By GUID")]
    [Delayed] public string AssetGUID;
    public string ResultPath;
    [PreviewField(65, Sirenix.OdinInspector.ObjectFieldAlignment.Left)]
    public Object FoundAssetByGUID;

    [Header("Find By Path")]
    [Delayed] public string AssetFullPath;
    [PreviewField(65, Sirenix.OdinInspector.ObjectFieldAlignment.Left)]
    public Object FoundAssetByPath;

    [Header("Find By Name")]
    [Delayed] public string AssetLabel;
    [Delayed] public string AssetNamePart;
    [Delayed] public string[] FoldersInSearching;
    public Object[] TargetAssets;

    [MenuItem("Tools/Assets Finder")]
    private static void OpenWindow()
    {
        GetWindow<AssetsFinderWindow>().position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 540);
    }

    [Button]
    [HorizontalGroup(GroupID = "Find Asset")]
    public void FindAssetsByGUID()
    {
        ResultPath = GetAssetPathFromGUID(AssetGUID);
        FoundAssetByGUID = GetAssetByGUID(AssetGUID);
    }

    [Button]
    [HorizontalGroup(GroupID = "Find Asset")]
    public void FindAssetByPath()
    {
        FoundAssetByPath = AssetDatabase.LoadAssetAtPath(AssetFullPath, typeof(Object));
    }

    [Button]
    [HorizontalGroup(GroupID = "Find Asset")]
    public void FindAssetByName()
    {
        if (string.IsNullOrEmpty(AssetNamePart))
            return;

        string filter = "";
        if (!string.IsNullOrEmpty(AssetLabel))
            filter = $"{AssetNamePart} l:{AssetLabel}";
        else
            filter = $"{AssetNamePart}";

        string[] guids = FoldersInSearching.Length > 0 ? AssetDatabase.FindAssets(filter, FoldersInSearching)
                                                       : AssetDatabase.FindAssets(filter);

        TargetAssets = new Object[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            TargetAssets[i] = GetAssetByGUID(guids[i]);
        }
    }

    private Object GetAssetByGUID(string guid)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        return AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
    }

    private string GetAssetPathFromGUID(string guid)
    {
        return AssetDatabase.GUIDToAssetPath(guid);
    }
}
#endif