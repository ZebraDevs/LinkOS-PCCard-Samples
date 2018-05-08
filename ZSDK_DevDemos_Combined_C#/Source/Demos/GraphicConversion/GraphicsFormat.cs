/***********************************************
 * CONFIDENTIAL AND PROPRIETARY 
 * 
 * The source code and other information contained herein is the confidential and exclusive property of
 * ZIH Corp. and is subject to the terms and conditions in your end user license agreement.
 * This source code, and any other information contained herein, shall not be copied, reproduced, published, 
 * displayed or distributed, in whole or in part, in any medium, by any means, for any purpose except as
 * expressly permitted under such license agreement.
 * 
 * Copyright ZIH Corp. 2018
 * 
 * ALL RIGHTS RESERVED
 ***********************************************/

using System;
using System.Reflection;
using Zebra.Sdk.Card.Enumerations;
using Zebra.Sdk.Card.Graphics.Enumerations;

namespace DeveloperDemo_Card_Desktop.Demos.GraphicConversion {

    class GraphicsFormatAttribute : Attribute {

        public string DisplayName { get; private set; }
        public MonochromeConversion MonochromeConversion { get; private set; }
        public PrintType PrintType { get; private set; }

        internal GraphicsFormatAttribute(string displayName, MonochromeConversion monochromeConversion, PrintType printType) {
            DisplayName = displayName;
            MonochromeConversion = monochromeConversion;
            PrintType = printType;
        }
    }

    public static class GraphicsFormats {
        public static string GetDisplayName(this GraphicsFormat graphicsFormat) {
            return GetAttr(graphicsFormat).DisplayName;
        }
        public static MonochromeConversion GetMonochromeConversion(this GraphicsFormat graphicsFormat) {
            return GetAttr(graphicsFormat).MonochromeConversion;
        }
        public static PrintType GetPrintType(this GraphicsFormat graphicsFormat) {
            return GetAttr(graphicsFormat).PrintType;
        }

        private static GraphicsFormatAttribute GetAttr(GraphicsFormat graphicsFormat) {
            return (GraphicsFormatAttribute)Attribute.GetCustomAttribute(ForValue(graphicsFormat), typeof(GraphicsFormatAttribute));
        }

        private static MemberInfo ForValue(GraphicsFormat graphicsFormat) {
            return typeof(GraphicsFormat).GetField(Enum.GetName(typeof(GraphicsFormat), graphicsFormat));
        }
    }

    public enum GraphicsFormat {
        [GraphicsFormat("Gray: Halftone 8x8", MonochromeConversion.HalfTone_8x8, PrintType.GrayDye)]
        GrayHalfTone8x8,

        [GraphicsFormat("Gray: Halftone 6x6", MonochromeConversion.HalfTone_6x6, PrintType.GrayDye)]
        GrayHalfTone6x6,

        [GraphicsFormat("Gray: Diffusion", MonochromeConversion.Diffusion, PrintType.GrayDye)]
        GrayDiffusion,

        [GraphicsFormat("Mono: Halftone 8x8", MonochromeConversion.HalfTone_8x8, PrintType.MonoK)]
        MonoHalfTone8x8,

        [GraphicsFormat("Mono: Halftone 6x6", MonochromeConversion.HalfTone_6x6, PrintType.MonoK)]
        MonoHalfTone6x6,

        [GraphicsFormat("Mono: Diffusion", MonochromeConversion.Diffusion, PrintType.MonoK)]
        MonoDiffusion,

        [GraphicsFormat("Color", MonochromeConversion.None, PrintType.Color)]
        Color
    }
}
