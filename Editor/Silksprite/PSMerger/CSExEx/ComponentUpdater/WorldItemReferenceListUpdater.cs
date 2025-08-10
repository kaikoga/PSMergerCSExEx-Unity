using System.Linq;
using Baxter.ClusterScriptExtensions;
using ClusterVR.CreatorKit.Item.Implements;
using UnityEditor;
using UnityEngine;

namespace Silksprite.PSMerger.CSExEx.ComponentUpdater
{
    public static class WorldItemReferenceListUpdater
    {
        // CSExEx: ClusterScriptComponentMergerExtensionBase を入力に取っているだけ
        public static void Update(ClusterScriptComponentMergerExtensionBase ext)
        {
            var entries = ext.ExtensionFields
                .Where(f => f.Type is ExtensionFieldType.WorldItem && f.ItemReferenceValue != null)
                .Select(f => (Id: f.FieldName, Item: f.ItemReferenceValue))
                .ToArray();

            var component = ext.GetComponent<WorldItemReferenceList>();

            // コンポーネントが要らない場合、削除した状態を正とする
            if (entries.Length == 0)
            {
                if (component != null)
                {
                    Object.DestroyImmediate(component);
                    EditorUtility.SetDirty(ext.gameObject);
                }
                return;
            }

            // 必要なら普通に追加
            if (component == null)
            {
                component = ext.gameObject.AddComponent<WorldItemReferenceList>();
            }

            // public propertyがないので、シリアライズした内容ベースでリストを構築する
            var so = new SerializedObject(component);
            var entriesProperty = so.FindProperty("worldItemReferences");
            entriesProperty.ClearArray();
            for (var i = 0; i < entries.Length; i++)
            {
                entriesProperty.InsertArrayElementAtIndex(i);
                var elem = entriesProperty.GetArrayElementAtIndex(i);
                elem.FindPropertyRelative("id").stringValue = entries[i].Id;
                elem.FindPropertyRelative("item").objectReferenceValue = entries[i].Item;
            }
            so.ApplyModifiedProperties();
        }
    }
}
