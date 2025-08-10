using System.Linq;
using Baxter.ClusterScriptExtensions;
using ClusterVR.CreatorKit.Item;
using ClusterVR.CreatorKit.Item.Implements;
using UnityEditor;
using UnityEngine;

namespace Silksprite.PSMerger.CSExEx.ComponentUpdater
{
    /// <summary>
    /// <see cref="ScriptableItemExtension"/> の内容から、アイテムにアタッチすべきItemAudioSetListを更新するクラス。
    /// 注意として、このクラスはリストの内容を完全に書き換え、手作業で指定したAudioClip情報は保全しない。
    /// </summary>
    public static class ItemAudioSetListUpdater
    {
        // CSExEx: ClusterScriptComponentMergerExtensionBase を入力に取っているだけ
        public static void Update(ClusterScriptComponentMergerExtensionBase ext)
        {
            var audioClipElements = ext.ExtensionFields
                .Where(f => f.Type is ExtensionFieldType.AudioClip && f.AudioClipValue != null)
                .Select(f => (Id: f.FieldName, Clip: f.AudioClipValue, Loop: f.BoolValue))
                .ToArray();

            var component = ext.GetComponent<ItemAudioSetList>();

            // コンポーネントが要らない場合、削除した状態を正とする
            if (audioClipElements.Length == 0)
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
                component = ext.gameObject.AddComponent<ItemAudioSetList>();
            }

            var itemAudioSets = audioClipElements
                .Select(elem => new ItemAudioSet(elem.Id, elem.Clip, elem.Loop))
                .ToArray();
            component.Construct(itemAudioSets);
        }
    }
}
