using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

using BEditor.Data;
using BEditor.Data.Primitive;
using BEditor.Data.Property;
using BEditor.Drawing;
using BEditor.Drawing.Pixel;
using BEditor.Plugin;
using Cx.Extensions.Acrylic.Resources;
using BEditor.Graphics;
using BEditor.Media;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/*namespace Cx.Extensions.Acrylic
{
    public class DynamicDialog : DialogProperty
    {
        public DynamicDialog(DialogSettings dialog)
        {

        }

        public DialogSettings Dialog { get; private set; }

        public override IEnumerable<PropertyElement> GetProperties()
        {
            return null;
        }
    }
}
*/