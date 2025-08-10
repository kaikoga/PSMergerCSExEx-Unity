using System;
using Baxter.ClusterScriptExtensions;
using Silksprite.PSMerger.CSExEx.ComponentUpdater;

namespace Silksprite.PSMerger.CSExEx.ScriptUpdater
{
    /// <summary>
    /// 単一の<see cref="ScriptableItemExtension"/>に対してコードの更新を行うクラス
    /// </summary>
    public static class ItemScriptUpdater
    {
        /// <summary>
        /// ScriptableItemExtensionの内容に基づいてソースコードを生成し、
        /// ScriptableItemのテキストとして適用する。また、そのときアセットの参照状況も適用する
        /// CSExEx: シーンやアセットには書き出さず、 JavaScriptInput を操作する
        /// </summary>
        public static string ApplyGeneratedSourceCode(ClusterScriptComponentMergerExtensionBase ext, string script)
        {
            // CSExEx: マージ後の位置情報を取得する
            var fields = FieldReloadUtil.ParseFields(ext, script);
            
            // 変数定義の部分を書き換える。後ろ側から書き換えるのはLocationの値が不正になるのを避けるため
            if (!string.IsNullOrEmpty(script))
            {
                for (var i = fields.Length - 1; i >= 0; i--)
                {
                    var field = fields[i];
                    var location = field.FieldDefinedLocation;
                    // CSExEx: PlayerScript に雑に対応する
                    var javaScriptValueLiteral = field.ToJavaScriptValueLiteral();
                    if (ext is PlayerScriptMergerExtension)
                    {
                        javaScriptValueLiteral = javaScriptValueLiteral.Replace("$", "_");
                    } 
                    script = Replace(script, location, $"{field.FieldName} = {javaScriptValueLiteral};");
                }
            }
            
            ItemAudioSetListUpdater.Update(ext);
            HumanoidAnimationListUpdater.Update(ext);
            WorldItemReferenceListUpdater.Update(ext);
            WorldItemTemplateListUpdater.Update(ext);
            ItemMaterialSetListUpdater.Update(ext);

            return script;
        }
        
        // 指定された位置のコードを消して別のテキストに置き換えたスクリプト文字列を生成する。
        // 1回呼ぶごとにScriptの文字数と同程度のallocをする実装になっているが、ホントはもう少しケチりたい…
        private static string Replace(string src, FieldDefinedLocation loc, string value)
        {
            //NOTE: Locationが不正な値じゃない前提で考える & Locationは行番号が1始まりなことに注意
            var lineNumber = 1;
            var startIndex = -1;
            var endIndex = -1;
            
            for (var i = 0; i < src.Length; i++)
            {
                if (startIndex < 0 && lineNumber == loc.startLine)
                {
                    startIndex = i + loc.startColumn;
                }

                if (endIndex < 0 && lineNumber == loc.endLine)
                {
                    endIndex = i + loc.endColumn;
                    break;
                }

                if (src[i] == '\n')
                {
                    lineNumber++;
                }
            }

            if (startIndex < 0 || endIndex < 0)
            {
                throw new InvalidOperationException("Location seems invalid");
            }

            return src.Substring(0, startIndex) + value + src.Substring(endIndex);
        }
    }
}
