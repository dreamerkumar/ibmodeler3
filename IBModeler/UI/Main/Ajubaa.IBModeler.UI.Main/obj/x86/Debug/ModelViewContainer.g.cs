﻿#pragma checksum "..\..\..\ModelViewContainer.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9A53D55D7CB59BEE1DA236F4AB51EB92"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Ajubaa.XamlModelViewer._3DTools;
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


namespace Ajubaa.IBModeler.UI.Main {
    
    
    /// <summary>
    /// ModelContainer
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class ModelContainer : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\..\ModelViewContainer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid ParentGrid;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\ModelViewContainer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LeftGrid;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\ModelViewContainer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock CurrentSmoothenCountMsg;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\ModelViewContainer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Decrease;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\ModelViewContainer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock SmootheningCountDisplay;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\ModelViewContainer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Increase;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\..\ModelViewContainer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock SmoothenHelpText;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\ModelViewContainer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SmoothenButton;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\..\ModelViewContainer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddSkin;
        
        #line default
        #line hidden
        
        
        #line 148 "..\..\..\ModelViewContainer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock AddSkinTextBox;
        
        #line default
        #line hidden
        
        
        #line 150 "..\..\..\ModelViewContainer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AdjustSkinButton;
        
        #line default
        #line hidden
        
        
        #line 195 "..\..\..\ModelViewContainer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Ajubaa.XamlModelViewer._3DTools.Trackport3D _trackPort;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Ajubaa.IBModeler.UI.Main;component/modelviewcontainer.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ModelViewContainer.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ParentGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.LeftGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.CurrentSmoothenCountMsg = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.Decrease = ((System.Windows.Controls.Button)(target));
            
            #line 79 "..\..\..\ModelViewContainer.xaml"
            this.Decrease.Click += new System.Windows.RoutedEventHandler(this.DecreaseClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.SmootheningCountDisplay = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.Increase = ((System.Windows.Controls.Button)(target));
            
            #line 86 "..\..\..\ModelViewContainer.xaml"
            this.Increase.Click += new System.Windows.RoutedEventHandler(this.IncreaseClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.SmoothenHelpText = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.SmoothenButton = ((System.Windows.Controls.Button)(target));
            
            #line 100 "..\..\..\ModelViewContainer.xaml"
            this.SmoothenButton.Click += new System.Windows.RoutedEventHandler(this.Smoothen);
            
            #line default
            #line hidden
            return;
            case 9:
            this.AddSkin = ((System.Windows.Controls.Button)(target));
            
            #line 147 "..\..\..\ModelViewContainer.xaml"
            this.AddSkin.Click += new System.Windows.RoutedEventHandler(this.AddSkinButtonClicked);
            
            #line default
            #line hidden
            return;
            case 10:
            this.AddSkinTextBox = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 11:
            this.AdjustSkinButton = ((System.Windows.Controls.Button)(target));
            
            #line 150 "..\..\..\ModelViewContainer.xaml"
            this.AdjustSkinButton.Click += new System.Windows.RoutedEventHandler(this.AdjustSkin);
            
            #line default
            #line hidden
            return;
            case 12:
            this._trackPort = ((Ajubaa.XamlModelViewer._3DTools.Trackport3D)(target));
            return;
            case 13:
            
            #line 246 "..\..\..\ModelViewContainer.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SaveAsXaml);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 252 "..\..\..\ModelViewContainer.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SaveAs3DS);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 317 "..\..\..\ModelViewContainer.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Regenerate);
            
            #line default
            #line hidden
            return;
            case 16:
            
            #line 322 "..\..\..\ModelViewContainer.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Troubleshoot);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

