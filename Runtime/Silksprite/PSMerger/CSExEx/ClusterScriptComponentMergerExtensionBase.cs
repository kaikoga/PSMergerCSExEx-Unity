using Baxter.ClusterScriptExtensions;
using UnityEngine;

namespace Silksprite.PSMerger.CSExEx
{
    public abstract class ClusterScriptComponentMergerExtensionBase : MonoBehaviour
    {
        [SerializeField] ScriptExtensionField[] extensionFields;

        public ScriptExtensionField[] ExtensionFields => extensionFields;

        public abstract ClusterScriptComponentMergerBase MergerBase { get; }

        public void SetFields(ScriptExtensionField[] fields) => extensionFields = fields;
    }

    public abstract class ClusterScriptComponentMergerExtensionBase<T> : ClusterScriptComponentMergerExtensionBase
        where T : ClusterScriptComponentMergerBase
    {
        public override ClusterScriptComponentMergerBase MergerBase => GetComponent<T>();
    }
}
