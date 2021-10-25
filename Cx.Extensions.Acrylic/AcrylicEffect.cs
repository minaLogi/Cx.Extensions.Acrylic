using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BEditor.Data;
using BEditor.Data.Primitive;
using BEditor.Data.Property.PrimitiveGroup;
using BEditor.Data.Property;
using BEditor.Drawing;
using BEditor.Drawing.Pixel;
using OpenCvSharp;
using BEditor.Graphics;

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

        [AllowNull]
        public EaseProperty BlurLevel { get; private set; }
        [AllowNull]
        public EaseProperty Alpha { get; private set; }
        //public CheckProperty UsePreset { get; private set; }
        //public SelectorProperty Preset { get; private set; }
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
            Image.Mask(args.Value, ParentImage, 
                new PointF(x, y), 0, false);
            Mat ParentImgM = ToMat(ParentImage);
            Mat imgM = ToMat(args.Value);
            var rect = new Rect((int)((imgM.Width - ParentImgM.Width) /2), 
                (int)((imgM.Height - ParentImgM.Height) /2), ParentImgM.Width, ParentImgM.Height);
            Mat OM = new Mat();
            Cv2.AddWeighted(new Mat(imgM, rect), a, ParentImgM, 1 - a, 0, OM);
            args.Value = ToImage(OM);
            Cv.GaussianBlur(args.Value, new BEditor.Drawing.Size(b, b), 0, 0);
        }
        public override IEnumerable<PropertyElement> GetProperties()
        {
            yield return Coordinate;
            yield return BlurLevel;
            yield return Alpha;
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