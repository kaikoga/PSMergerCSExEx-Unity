using System;
using System.Linq;
using Baxter.ClusterScriptExtensions;
using Baxter.ClusterScriptExtensions.Editor.ScriptParser;

namespace Silksprite.PSMerger.CSExEx.ScriptUpdater
{
    public static class FieldReloadUtil
    {
        // CSExEx: インスペクタに表示するために ClusterScriptComponentMergerExtensionBase にセットする用
        public static void ReloadFields(ClusterScriptComponentMergerExtensionBase ext, bool refresh)
        {
            // CSExEx: 全ての入力ソースコードからフィールドを収集する
            // CSExEx: ScriptExtensionField の位置情報は入力ソースコード上の位置情報であり、コード更新の目的には使えないことに注意
            var env = ext.MergerBase.ToCompilerEnvironment();

            var templateCodes = env.AllInputs().Select(input => input.SourceCode).ToArray();
            if (templateCodes.Length == 0)
            {
                ext.SetFields(Array.Empty<ScriptExtensionField>());
            }
            else
            {
                var fields = templateCodes.SelectMany(ExtensionFieldParser.ExtractTargetFields).ToArray();
                foreach (var f in fields)
                {
                    InitializeExtensionFieldValue(f, ext.ExtensionFields, refresh);
                }
                ext.SetFields(fields);
            }
        }
        
        // CSExEx: マージ後のソースコードに対して ItemScriptUpdater から呼び出す用
        // マージ後のソースコードは ClusterScriptComponentMergerExtensionBase に保存しない
        public static ScriptExtensionField[] ParseFields(ClusterScriptComponentMergerExtensionBase ext, string templateCode)
        {
            if (string.IsNullOrEmpty(templateCode))
            {
                return Array.Empty<ScriptExtensionField>();
            }
            else
            {
                var fields = ExtensionFieldParser.ExtractTargetFields(templateCode);
                foreach (var f in fields)
                {
                    InitializeExtensionFieldValue(f, ext.ExtensionFields, false);
                }
                return fields;
            }
        }
        
        private static void InitializeExtensionFieldValue(
            ScriptExtensionField field, ScriptExtensionField[] existingFields, bool refresh)
        {
            if (refresh)
            {
                field.ResetValues();
                return;
            }

            // - 名前と型が同じフィールドの値があれば持ち越す
            // - そうでない場合、スクリプトを参考に初期値が定まる
            var existingField = existingFields.FirstOrDefault(
                ef => ef.FieldName == field.FieldName && ef.Type == field.Type
            );
            
            if (existingField != null)
            {
                field.CopyValues(existingField);
            }
            else
            {
                field.ResetValues();
            }
        }
    }
}
