#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class F2LTableEditor : EditorWindow
{
    private F2LTableCreator tableCreator;
    private F2LStateTable currentTable;
    private bool isBuilding = false;

    [MenuItem("CubeSolver/Build F2L Table")]
    static void ShowWindow()
    {
        GetWindow<F2LTableEditor>("F2L Table Builder");
    }

    void OnGUI()
    {
        GUILayout.Label("F2L状态表构建工具", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        if (GUILayout.Button("构建新的F2L状态表", GUILayout.Height(30))) {
            BuildF2LTable();
        }

        EditorGUILayout.Space();

        if (currentTable != null) {
            EditorGUILayout.LabelField("当前表信息:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"构建时间: {currentTable.buildDate}");

            if (GUILayout.Button("保存表到资产")) {
                SaveTableToAsset();
            }
        }

        if (isBuilding) {
            EditorGUILayout.HelpBox("正在构建状态表，请稍候...", MessageType.Info);
        }
    }

    void BuildF2LTable()
    {
        if (tableCreator == null) {
            GameObject tempGO = new GameObject("TempTableCreator");
            tableCreator = tempGO.AddComponent<F2LTableCreator>();
        }

        isBuilding = true;

        // 在编辑器协程中运行，避免界面卡死
        EditorApplication.update += BuildTableAsync;
    }

    void BuildTableAsync()
    {
        EditorApplication.update -= BuildTableAsync;

        try {
            currentTable = tableCreator.CreateTableAsset();
            Debug.Log("状态表构建完成！");
        }
        catch (System.Exception e) {
            Debug.LogError($"构建状态表时出错: {e.Message}");
        }
        finally {
            isBuilding = false;

            // 清理临时对象
            if (tableCreator != null && tableCreator.gameObject != null) {
                DestroyImmediate(tableCreator.gameObject);
            }

            Repaint();
        }
    }

    void SaveTableToAsset()
    {
        if (currentTable == null) {
            Debug.LogError("没有可保存的状态表！");
            return;
        }

        string folderName = "Resources/StateTables";
        string assetRelativePath = $"Assets/{folderName}/F2LStateTables.asset";

        // 若已存在，先删除（避免 CreateAsset 失败或产生冗余）
        if (System.IO.File.Exists(Application.dataPath + "/" + folderName + "/F2LStateTables.asset")) {
            AssetDatabase.DeleteAsset(assetRelativePath);
        }
        else {
            // 确保目录存在
            string targetDir = System.IO.Path.Combine(Application.dataPath, folderName);
            if (!System.IO.Directory.Exists(targetDir)) System.IO.Directory.CreateDirectory(targetDir);
        }

        AssetDatabase.CreateAsset(currentTable, assetRelativePath);
        EditorUtility.SetDirty(currentTable); // 标记“已更改”，确保 Unity 保存其序列化数据
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"状态表已保存到: {assetRelativePath}");
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = currentTable;
    }

}
#endif