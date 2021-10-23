using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Reflection;

using BEditor.Data;
using BEditor.Data.Primitive;
using BEditor.Data.Property.PrimitiveGroup;
using BEditor.Data.Property;
using BEditor.Drawing;
using BEditor.Drawing.Pixel;
using Cx.Extensions.Acrylic.Resources;
using BEditor.Graphics;
using BEditor.Media;
using BEditor.Resources;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using System.Drawing;
using System.Drawing.Imaging;
using OpenCvSharp.Extensions;

namespace Cx.Extensions.Acrylic
{
    public class AcrylicEffect : ImageEffect
    {
        public static readonly DirectProperty<AcrylicEffect, Coordinate> CoordinateProperty = EditingProperty.RegisterDirect<Coordinate, AcrylicEffect>(
            nameof(Coordinate),
            owner => owner.Coordinate,
            (owner, obj) => owner.Coordinate = obj,
            EditingPropertyOptions<Coordinate>.Create(new CoordinateMetadata("座標")).Serialize());

        public static readonly DirectProperty<AcrylicEffect, EaseProperty> BlurLevelProperty = EditingProperty.RegisterDirect<EaseProperty, AcrylicEffect>(
            nameof(BlurLevel),
            owner => owner.BlurLevel,
            (owner, obj) => owner.BlurLevel = obj,
            EditingPropertyOptions<EaseProperty>.Create(new EasePropertyMetadata("ぼかし", min: 0)).Serialize());

        public static readonly DirectProperty<AcrylicEffect, EaseProperty> AlphaProperty = EditingProperty.RegisterDirect<EaseProperty, AcrylicEffect>(
            nameof(Alpha),
            owner => owner.Alpha,
            (owner, obj) => owner.Alpha = obj,
            EditingPropertyOptions<EaseProperty>.Create(new EasePropertyMetadata("透明度", max : 100, min : 0)).Serialize());

        public static readonly DirectProperty<AcrylicEffect, CheckProperty> UsePresetProperty = EditingProperty.RegisterDirect<CheckProperty, AcrylicEffect>(
            nameof(UsePreset),
            owner => owner.UsePreset,
            (owner, obj) => owner.UsePreset = obj,
            EditingPropertyOptions<CheckProperty>.Create(new CheckPropertyMetadata("プリセットを使用する")).Serialize());

        public static readonly DirectProperty<AcrylicEffect, SelectorProperty> PresetProperty = EditingProperty.RegisterDirect<SelectorProperty, AcrylicEffect>(
            nameof(Preset),
            owner => owner.Preset,
            (owner, obj) => owner.Preset = obj,
            EditingPropertyOptions<SelectorProperty>.Create(new SelectorPropertyMetadata("プリセットを選択", new string[]
            {
                "test1",
                "test2"
            })).Serialize());

        

        [AllowNull]
        public EaseProperty BlurLevel { get; private set; }
        [AllowNull]
        public EaseProperty Alpha { get; private set; }
        public CheckProperty UsePreset { get; private set; }
        public SelectorProperty Preset { get; private set; }
        [AllowNull]
        public Coordinate Coordinate { get; private set; }
        public override string Name => "アクリル効果";

        public override void Apply(EffectApplyArgs<Image<BGRA32>> args)
        {
            int f = args.Frame;
            Scene scene = Parent.Parent;
            Image<BGRA32> image = new Image<BGRA32>(scene.Width, scene.Height);
            Image<BGRA32> ParentImage = args.Value;
            int b = (int)BlurLevel.GetValue(f);
            float a = Alpha.GetValue(f)/100;
            Coordinate p = GetValue(CoordinateProperty);
            float x = p.X.GetValue(f);
            float y = p.Y.GetValue(f);

            scene.GraphicsContext?.ReadImage(image);
            args.Value = image;
            BEditor.Drawing.Image.Mask(args.Value, ParentImage, 
                new BEditor.Drawing.PointF(x, y), 0, false);
            Mat ParentImgM = ToMat(ParentImage);
            using var imgM = ToMat(args.Value);
            Mat OM = new Mat();
            var rect = new Rect((int)(((imgM.Width - ParentImgM.Width) /2 + x)), 
                (int)(((imgM.Height - ParentImgM.Height) /2 + y)), ParentImgM.Width, ParentImgM.Height);
            Cv2.AddWeighted(ParentImgM, 1 - a, new Mat(imgM, rect), a, 0, OM);
            args.Value = ToImage(OM);
            image = args.Value.MakeBorder(args.Value.Width, args.Value.Height);
            args.Value.Dispose();
            args.Value = image;
            Cv.GaussianBlur(args.Value, new BEditor.Drawing.Size(b, b), 0, 0);
        }
        public override IEnumerable<PropertyElement> GetProperties()
        {
            yield return Coordinate;
            yield return BlurLevel;
            yield return Alpha;
            yield return UsePreset;
            yield return Preset;
        }
        private static unsafe Mat ToMat(Image<BGRA32> image)
        {
            fixed (BGRA32* ptr = image.Data)
            {
                return new Mat(image.Height, image.Width, MatType.CV_8UC4, (IntPtr)ptr);
            }
        }

        private static unsafe Image<BGRA32> ToImage(Mat mat)
        {
            return new Image<BGRA32>(mat.Width, mat.Height, mat.Data);
        }
    }
}