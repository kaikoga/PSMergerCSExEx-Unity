using Silksprite.PSMerger.Compiler.Data;
using Silksprite.PSMerger.Compiler.Filter;
using Silksprite.PSMerger.CSExEx.ScriptUpdater;
using UnityEditor;
using UnityEngine;

namespace Silksprite.PSMerger.CSExEx
{
    public class PSMergerExtensionFilter : IPSMergerFilter
    {
        [InitializeOnLoadMethod]
        static void Register()
        {
            PSMergerFilter.Register(new PSMergerExtensionFilter());
        }

        public int Priority => 10000; // very late

        public JavaScriptCompilerEnvironment Filter(JavaScriptCompilerEnvironment environment, Object unityContext)
        {
            return environment;
        }

        public string PostProcess(string sourceCode, Object unityContext)
        {
            ClusterScriptComponentMergerExtensionBase extBase = unityContext switch
            {
                ItemScriptMerger itemScriptMerger => itemScriptMerger.GetComponent<ItemScriptMergerExtension>(),
                PlayerScriptMerger playerScriptMerger => playerScriptMerger.GetComponent<PlayerScriptMergerExtension>(),
                _ => null
            };
            if (extBase == null)
            {
                return sourceCode;
            } 
            return ApplyGeneratedSourceCode(extBase, sourceCode);
        }

        static string ApplyGeneratedSourceCode(ClusterScriptComponentMergerExtensionBase extBase, string sourceCode)
        {
            return ItemScriptUpdater.ApplyGeneratedSourceCode(extBase, sourceCode);
        }
    }
}
