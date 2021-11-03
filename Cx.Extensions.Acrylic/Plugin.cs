using System;
using System.Collections.Generic;
using BEditor.Data;
using BEditor.Plugin;

using System.IO;

namespace Cx.Extensions.Acrylic
{
    public class Plugin : PluginObject
    {
        private SettingRecord _settings;
        public Plugin(PluginConfig config) : base(config)
        { }
        public override string PluginName => "Cx.Extensions.Acrylic";

        // プラグインの説明
        public override string Description => "オブジェクトにアクリル効果を付与します。";

        // プラグインを識別するのId
        // 開発環境では Zero のままで大丈夫ですが、
        // 公開する場合はGUID生成ツールなどで生成したIdを指定してください。
        public override Guid Id { get; } = Guid.Parse("6172cd79-c0f1-4663-b7cb-7ef966703063");

        // プラグインの設定、詳しくは https://github.com/b-editor/BEditor/blob/main/extensions/BEditor.Extensions.AviUtl/EntryPlugin.cs#L379 をご覧ください。
        public override SettingRecord Settings 
        { 
            get
            {
                if (_settings == null)
                {
                    // 設定がnullの場合ファイルから読み込む、
                    // SettingRecord.LoadFromからnullが返ってきたらデフォルトの設定をNewする。
                    _settings = SettingRecord.LoadFrom<PluginSettings>(Path.Combine(BaseDirectory, "settings.json"))
                        ?? new PluginSettings();
                }

                return _settings;
            }
            set
            {
                _settings = value;
                // 設定を保存
                _settings.Save(Path.Combine(BaseDirectory, "settings.json"));
            }
        }
        public static void Register()
        {
            PluginBuilder.Configure<Plugin>().With(CreateEffectMetadata()).Register();
        }

        private static EffectMetadata CreateEffectMetadata()
        {
            var list = new List<EffectMetadata> { EffectMetadata.Create<AcrylicEffect>("アクリル効果") };

            var metadata = new EffectMetadata("Acrylic")
            {
                Children = list
            };

            return metadata;
        }

        public record PluginSettings() : SettingRecord
        {
        }
    }
}

