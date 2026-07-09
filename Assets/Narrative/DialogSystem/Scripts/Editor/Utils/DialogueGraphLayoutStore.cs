#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Miemie.DialogSystem.Editor
{
    /// <summary>
    /// 读写 GraphView 节点布局 与运行时逻辑无关
    /// </summary>
    static class DialogueGraphLayoutStore
    {
        const string DatabasePath = DialogueEditorPaths.LayoutAssetPath;

        static DialogueGraphLayoutDatabase database;

        /// <summary>
        /// 尝试读取节点坐标
        /// </summary>
        public static bool TryGetPosition(DialogueGraph graph, DialogueNodeData node, out Vector2 position)
        {
            position = Vector2.zero;
            if (graph == null || node == null)
                return false;

            var entry = GetGraphEntry(graph, create: false);
            if (entry == null)
                return false;

            foreach (var layout in entry.layouts)
            {
                if (layout.nodeId != node.ConfigId || !layout.hasPosition)
                    continue;

                position = layout.position;
                return true;
            }

            return false;
        }

        public static Vector2 GetPosition(DialogueGraph graph, DialogueNodeData node) =>
            TryGetPosition(graph, node, out var position) ? position : Vector2.zero;

        public static void SetPosition(DialogueGraph graph, DialogueNodeData node, Vector2 position)
        {
            if (graph == null || node == null)
                return;

            var entry = GetGraphEntry(graph, create: true);
            foreach (var layout in entry.layouts)
            {
                if (layout.nodeId != node.ConfigId)
                    continue;

                layout.hasPosition = true;
                layout.position = position;
                SaveDatabase();
                return;
            }

            entry.layouts.Add(new NodeLayoutEntry
            {
                nodeId = node.ConfigId,
                hasPosition = true,
                position = position,
            });
            SaveDatabase();
        }

        public static void RemoveNode(DialogueGraph graph, DialogueNodeData node)
        {
            if (graph == null || node == null)
                return;

            var entry = GetGraphEntry(graph, create: false);
            if (entry == null)
                return;

            entry.layouts.RemoveAll(e => e.nodeId == node.ConfigId);
            SaveDatabase();
        }

        /// <summary>
        /// 删除整张图的布局数据
        /// </summary>
        public static void RemoveGraph(DialogueGraph graph)
        {
            if (graph == null)
                return;

            var db = GetDatabase();
            db.graphs.RemoveAll(e => e.graph == graph);
            SaveDatabase();
        }

        public static void ReplaceGraphLayouts(DialogueGraph graph, IEnumerable<(DialogueNodeData node, Vector2 position)> layouts)
        {
            if (graph == null)
                return;

            var entry = GetGraphEntry(graph, create: true);
            entry.layouts.Clear();

            if (layouts != null)
            {
                foreach (var (node, position) in layouts)
                {
                    if (node == null)
                        continue;

                    entry.layouts.Add(new NodeLayoutEntry
                    {
                        nodeId = node.ConfigId,
                        hasPosition = true,
                        position = position,
                    });
                }
            }

            SaveDatabase();
        }

        static GraphLayoutEntry GetGraphEntry(DialogueGraph graph, bool create)
        {
            var db = GetDatabase();
            foreach (var entry in db.graphs)
            {
                if (entry.graph == graph)
                    return entry;
            }

            if (!create)
                return null;

            var created = new GraphLayoutEntry { graph = graph };
            db.graphs.Add(created);
            SaveDatabase();
            return created;
        }

        static DialogueGraphLayoutDatabase GetDatabase()
        {
            if (database != null)
                return database;

            database = AssetDatabase.LoadAssetAtPath<DialogueGraphLayoutDatabase>(DatabasePath);
            if (database != null)
                return database;

            DialogueEditorPaths.EnsureGraphAssetFolder();
            database = ScriptableObject.CreateInstance<DialogueGraphLayoutDatabase>();
            AssetDatabase.CreateAsset(database, DatabasePath);
            AssetDatabase.SaveAssets();
            return database;
        }

        static void SaveDatabase()
        {
            if (database == null)
                return;

            EditorUtility.SetDirty(database);
        }

        public static bool IsDatabaseDirty()
        {
            if (database != null)
                return EditorUtility.IsDirty(database);

            var db = AssetDatabase.LoadAssetAtPath<DialogueGraphLayoutDatabase>(DatabasePath);
            return db != null && EditorUtility.IsDirty(db);
        }

        public static void SaveDatabaseAssets()
        {
            if (database == null)
                database = AssetDatabase.LoadAssetAtPath<DialogueGraphLayoutDatabase>(DatabasePath);

            if (database == null)
                return;

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssetIfDirty(database);
        }
    }
}
#endif
