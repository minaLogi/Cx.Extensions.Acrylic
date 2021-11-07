using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BEditor.Data;
using BEditor.Data.Primitive;
using BEditor.Data.Property;
using BEditor.Drawing;
using BEditor.Drawing.Pixel;
using BEditor.Graphics;
using System.Linq;
using System;

namespace Cx.Extensions.Acrylic
{
    public class AcrylicEffect : ImageEffect
    {
        public static readonly DirectProperty<AcrylicEffect, EaseProperty> BlurLevelProperty = EditingProperty.RegisterDirect<EaseProperty, AcrylicEffect>(
            nameof(BlurLevel),
            owner => owner.BlurLevel,
            (owner, obj) => owner.BlurLevel = obj,
            EditingPropertyOptions<EaseProperty>.Create(new EasePropertyMetadata("ぼかし", min: 0)).Serialize());

        public static readonly DirectProperty<AcrylicEffect, EaseProperty> AlphaProperty = EditingProperty.RegisterDirect<EaseProperty, AcrylicEffect>(
            nameof(Alpha),
            owner => owner.Alpha,
            (owner, obj) => owner.Alpha = obj,
            EditingPropertyOptions<EaseProperty>.Create(new EasePropertyMetadata("不透明度", max: 100, min: 0)).Serialize());

        [AllowNull]
        public EaseProperty BlurLevel { get; private set; }
        [AllowNull]
        public EaseProperty Alpha { get; private set; }
        [AllowNull]
        public override string Name => "アクリル効果";

        public override void Apply(EffectApplyArgs<Image<BGRA32>> args)
        {
        }

        public override void Apply(EffectApplyArgs<IEnumerable<Texture>> args)
        {
            args.Value = args.Value.SelectMany(i => Selector(i, args));
        }
        private IEnumerable<Texture> Selector(Texture texture, EffectApplyArgs args)
        {
            var p = Parent.Parent;
            var f = args.Frame;
            var b = BlurLevel.GetValue(f);
            Image<BGRA32> image = texture.ToImage();
            Image<BGRA32> BgTexture = new Image<BGRA32>(p.Width, p.Height);
            p.GraphicsContext.ReadImage(BgTexture);
            var x = texture.Transform.Position.X;
            var y = texture.Transform.Position.Y;
            Cv.GaussianBlur(BgTexture, new Size((int)b,(int)b), 0, 0);
            var resizedimg = image;
            resizedimg = resizedimg.Resize((int)Math.Abs(texture.Transform.Scale.X*texture.Width),
                (int)Math.Abs(texture.Transform.Scale.Y*texture.Height), Quality.High);
            BgTexture.Mask(resizedimg, new PointF(x, y), 0, false);
            resizedimg.Dispose();
            image.SetOpacity(Alpha.GetValue(f)/100);
            var result = new Texture[2];
            var bg = Texture.FromImage(BgTexture);
            var im = Texture.FromImage(image);
            var tt = texture.Transform;
            im.Transform = tt;
            result[0] = bg;
            result[1] = im;
            return result;
        }
        public override IEnumerable<PropertyElement> GetProperties()
        {
            yield return BlurLevel;
            yield return Alpha;
        }
    }
}