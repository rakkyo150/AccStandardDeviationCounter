﻿using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace AccStandardDeviationCounter
{
    public class Configuration
    {
        public static Configuration Instance { get; set; }
        public virtual bool SeparateSaber { get; set; } = true;
        public virtual int DecimalPrecision { get; set; } = 2;
        public virtual bool EnableLabel { get; set; } = true;
        public virtual float LabelFontSize { get; set; } = 3f;
        public virtual float FigureFontSize { get; set; } = 4f;
        public virtual float OffsetX { get; set; } = 0f;
        public virtual float OffsetY { get; set; } = 0f;
        public virtual float OffsetZ { get; set; } = 0f;
        public virtual bool IncludeChains { get; set; } = false;
    }


}
