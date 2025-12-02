#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class OLLTableEditor : EditorWindow
{
    private OLLTableCreator tableCreator;
    private OLLStateTable currentTable;
    private bool isBuilding = false;

    [MenuItem("CubeSolver/Build OLL Table")]
    static void ShowWindow()
    {
        GetWindow<OLLTableEditor>("OLL Table Builder");
    }

    void OnGUI()
    {
        GUILayout.Label("OLL状态表构建工具", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        if (GUILayout.Button("构建新的OLL状态表", GUILayout.Height(30))) {
            BuildOLLTable();
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

    void BuildOLLTable()
    {
        if (tableCreator == null) {
            GameObject tempGO = new GameObject("TempTableCreator");
            tableCreator = tempGO.AddComponent<OLLTableCreator>();
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
        // 构建目标文件夹的完整系统路径
        string targetDir = System.IO.Path.Combine(Application.dataPath, folderName);

        // 确保目标目录存在
        if (!System.IO.Directory.Exists(targetDir)) {
            System.IO.Directory.CreateDirectory(targetDir);
        }

        // 构建资产在项目中的相对路径
        string assetRelativePath = $"Assets/{folderName}/OLLStateTable.asset";

        // 使用AssetDatabase创建资产
        AssetDatabase.CreateAsset(currentTable, assetRelativePath);
        AssetDatabase.SaveAssets(); // 保存项目范围的变更[citation:2]
        AssetDatabase.Refresh();

        Debug.Log($"状态表已保存到: {assetRelativePath}");

        // 可选：在Project窗口中高亮显示新创建的资源
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = currentTable;
    }
}
#endif