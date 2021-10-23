using System;
using System.Collections.Generic;
using System.Diagnostics;
using BEditor.Data;
using BEditor.Plugin;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

using BEditor.Data.Property;
using BEditor.Drawing;

namespace Cx.Extensions.Acrylic
{
    public class Plugin : PluginObject
    {
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
        public override SettingRecord Settings { get; set; } = new();
        public static void Register()
        {
            PluginBuilder.Configure<Plugin>()
                .With(CreateEffectMetadata())
                .SetCustomMenu("Acrylic", new ICustomMenu[]
                {
                    new CustomMenu("プリセットの設定",null)
                })
                .Register();
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
    }
}

