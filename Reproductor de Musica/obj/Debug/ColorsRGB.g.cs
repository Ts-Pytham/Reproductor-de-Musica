﻿#pragma checksum "..\..\ColorsRGB.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9ABE4D9C2500E33B24CA2B5C6C2CA17C875E52DD8F4FEE0B260A9A074682C964"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using Reproductor_de_Musica;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Reproductor_de_Musica {
    
    
    /// <summary>
    /// ColorsRGB
    /// </summary>
    public partial class ColorsRGB : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\ColorsRGB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WrapPanel WrapPanel_Principal;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\ColorsRGB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider R;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\ColorsRGB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider G;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\ColorsRGB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider B;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\ColorsRGB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextBox_R;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\ColorsRGB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextBox_G;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\ColorsRGB.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextBox_B;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Reproductor de Musica;component/colorsrgb.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ColorsRGB.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\ColorsRGB.xaml"
            ((Reproductor_de_Musica.ColorsRGB)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.WrapPanel_Principal = ((System.Windows.Controls.WrapPanel)(target));
            
            #line 11 "..\..\ColorsRGB.xaml"
            this.WrapPanel_Principal.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.WrapPanel_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 12 "..\..\ColorsRGB.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonMinimize_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 15 "..\..\ColorsRGB.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonX_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.R = ((System.Windows.Controls.Slider)(target));
            
            #line 19 "..\..\ColorsRGB.xaml"
            this.R.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.Slider_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.G = ((System.Windows.Controls.Slider)(target));
            
            #line 20 "..\..\ColorsRGB.xaml"
            this.G.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.Slider_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.B = ((System.Windows.Controls.Slider)(target));
            
            #line 21 "..\..\ColorsRGB.xaml"
            this.B.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.Slider_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.TextBox_R = ((System.Windows.Controls.TextBox)(target));
            
            #line 23 "..\..\ColorsRGB.xaml"
            this.TextBox_R.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextBox_TextChanged);
            
            #line default
            #line hidden
            
            #line 23 "..\..\ColorsRGB.xaml"
            this.TextBox_R.KeyDown += new System.Windows.Input.KeyEventHandler(this.TextBox_KeyDown);
            
            #line default
            #line hidden
            return;
            case 9:
            this.TextBox_G = ((System.Windows.Controls.TextBox)(target));
            
            #line 24 "..\..\ColorsRGB.xaml"
            this.TextBox_G.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextBox_TextChanged);
            
            #line default
            #line hidden
            
            #line 24 "..\..\ColorsRGB.xaml"
            this.TextBox_G.KeyDown += new System.Windows.Input.KeyEventHandler(this.TextBox_KeyDown);
            
            #line default
            #line hidden
            return;
            case 10:
            this.TextBox_B = ((System.Windows.Controls.TextBox)(target));
            
            #line 25 "..\..\ColorsRGB.xaml"
            this.TextBox_B.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextBox_TextChanged);
            
            #line default
            #line hidden
            
            #line 25 "..\..\ColorsRGB.xaml"
            this.TextBox_B.KeyDown += new System.Windows.Input.KeyEventHandler(this.TextBox_KeyDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

