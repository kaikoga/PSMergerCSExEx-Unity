using UnityEngine;

namespace Silksprite.PSMerger.CSExEx
{
    [AddComponentMenu("Silksprite/PSMerger/ItemScript Merger Extension", 1000)]
    [RequireComponent(typeof(ItemScriptMerger)), DisallowMultipleComponent]
    public class ItemScriptMergerExtension : ClusterScriptComponentMergerExtensionBase<ItemScriptMerger>
    {
    }
}
