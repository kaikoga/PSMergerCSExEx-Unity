using UnityEngine;

namespace Silksprite.PSMerger.CSExEx
{
    [AddComponentMenu("Silksprite/PSMerger/PlayerScript Merger Extension", 1001)]
    [RequireComponent(typeof(PlayerScriptMerger)), DisallowMultipleComponent]
    public class PlayerScriptMergerExtension : ClusterScriptComponentMergerExtensionBase<PlayerScriptMerger>
    {
    }
}
