﻿#pragma checksum "C:\Users\Gergo\Documents\VisualStudioProjects\Jellyfin\HomeView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "29D6A07F4F969EC33054358B5FEF579EEAB0D5BC1A07D9EBB67CC64C9E40938B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Jellyfin
{
    partial class HomeView : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 0.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private static class XamlBindingSetters
        {
            public static void Set_Windows_UI_Xaml_Controls_ItemsControl_ItemsSource(global::Windows.UI.Xaml.Controls.ItemsControl obj, global::System.Object value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = (global::System.Object) global::Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(global::System.Object), targetNullValue);
                }
                obj.ItemsSource = value;
            }
            public static void Set_Windows_UI_Xaml_Controls_Image_Source(global::Windows.UI.Xaml.Controls.Image obj, global::Windows.UI.Xaml.Media.ImageSource value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = (global::Windows.UI.Xaml.Media.ImageSource) global::Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(global::Windows.UI.Xaml.Media.ImageSource), targetNullValue);
                }
                obj.Source = value;
            }
            public static void Set_Windows_UI_Xaml_Documents_Run_Text(global::Windows.UI.Xaml.Documents.Run obj, global::System.String value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = targetNullValue;
                }
                obj.Text = value ?? global::System.String.Empty;
            }
        };

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 0.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private class HomeView_obj6_Bindings :
            global::Windows.UI.Xaml.IDataTemplateExtension,
            global::Windows.UI.Xaml.Markup.IDataTemplateComponent,
            global::Windows.UI.Xaml.Markup.IXamlBindScopeDiagnostics,
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            IHomeView_Bindings
        {
            private global::Jellyfin.Models.BaseItemDto dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);
            private bool removedDataContextHandler = false;

            // Fields for each control that has bindings.
            private global::System.WeakReference obj6;
            private global::Windows.UI.Xaml.Controls.Image obj7;
            private global::Windows.UI.Xaml.Documents.Run obj8;

            // Static fields for each binding's enabled/disabled state
            private static bool isobj7SourceDisabled = false;
            private static bool isobj8TextDisabled = false;

            public HomeView_obj6_Bindings()
            {
            }

            public void Disable(int lineNumber, int columnNumber)
            {
                if (lineNumber == 27 && columnNumber == 85)
                {
                    isobj7SourceDisabled = true;
                }
                else if (lineNumber == 29 && columnNumber == 34)
                {
                    isobj8TextDisabled = true;
                }
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 6: // HomeView.xaml line 25
                        this.obj6 = new global::System.WeakReference((global::Windows.UI.Xaml.Controls.GridViewItem)target);
                        break;
                    case 7: // HomeView.xaml line 27
                        this.obj7 = (global::Windows.UI.Xaml.Controls.Image)target;
                        break;
                    case 8: // HomeView.xaml line 29
                        this.obj8 = (global::Windows.UI.Xaml.Documents.Run)target;
                        break;
                    default:
                        break;
                }
            }

            public void DataContextChangedHandler(global::Windows.UI.Xaml.FrameworkElement sender, global::Windows.UI.Xaml.DataContextChangedEventArgs args)
            {
                 if (this.SetDataRoot(args.NewValue))
                 {
                    this.Update();
                 }
            }

            // IDataTemplateExtension

            public bool ProcessBinding(uint phase)
            {
                throw new global::System.NotImplementedException();
            }

            public int ProcessBindings(global::Windows.UI.Xaml.Controls.ContainerContentChangingEventArgs args)
            {
                int nextPhase = -1;
                ProcessBindings(args.Item, args.ItemIndex, (int)args.Phase, out nextPhase);
                return nextPhase;
            }

            public void ResetTemplate()
            {
                Recycle();
            }

            // IDataTemplateComponent

            public void ProcessBindings(global::System.Object item, int itemIndex, int phase, out int nextPhase)
            {
                nextPhase = -1;
                switch(phase)
                {
                    case 0:
                        nextPhase = -1;
                        this.SetDataRoot(item);
                        if (!removedDataContextHandler)
                        {
                            removedDataContextHandler = true;
                            (this.obj6.Target as global::Windows.UI.Xaml.Controls.GridViewItem).DataContextChanged -= this.DataContextChangedHandler;
                        }
                        this.initialized = true;
                        break;
                }
                this.Update_((global::Jellyfin.Models.BaseItemDto) item, 1 << phase);
            }

            public void Recycle()
            {
            }

            // IHomeView_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
            }

            public bool SetDataRoot(global::System.Object newDataRoot)
            {
                if (newDataRoot != null)
                {
                    this.dataRoot = (global::Jellyfin.Models.BaseItemDto)newDataRoot;
                    return true;
                }
                return false;
            }

            private delegate void InvokeFunctionDelegate(int phase);
            private global::System.Collections.Generic.Dictionary<string, InvokeFunctionDelegate> PendingFunctionBindings = new global::System.Collections.Generic.Dictionary<string, InvokeFunctionDelegate>();

            private void Invoke_M_getImageUrl_371857150(int phase)
            {
                global::Windows.UI.Xaml.Media.ImageSource result = this.dataRoot.getImageUrl();
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // HomeView.xaml line 27
                    if (!isobj7SourceDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_Image_Source(this.obj7, result, null);
                    }
                }
            }

            private void CompleteUpdate(int phase)
            {
                var functions = this.PendingFunctionBindings;
                this.PendingFunctionBindings = new global::System.Collections.Generic.Dictionary<string, InvokeFunctionDelegate>();
                foreach (var function in functions.Values)
                {
                    function.Invoke(phase);
                }
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::Jellyfin.Models.BaseItemDto obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_M_getImageUrl_371857150(phase);
                        this.Update_Name(obj.Name, phase);
                    }
                }
                this.CompleteUpdate(phase);
            }
            private void Update_M_getImageUrl_371857150(int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    if (!isobj7SourceDisabled)
                    {
                        this.PendingFunctionBindings["M_getImageUrl_371857150"] = new InvokeFunctionDelegate(this.Invoke_M_getImageUrl_371857150); 
                    }
                }
            }
            private void Update_Name(global::System.String obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // HomeView.xaml line 29
                    if (!isobj8TextDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Documents_Run_Text(this.obj8, obj, null);
                    }
                }
            }
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 0.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private class HomeView_obj1_Bindings :
            global::Windows.UI.Xaml.Markup.IDataTemplateComponent,
            global::Windows.UI.Xaml.Markup.IXamlBindScopeDiagnostics,
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            IHomeView_Bindings
        {
            private global::Jellyfin.HomeView dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Windows.UI.Xaml.Controls.ListView obj4;

            // Static fields for each binding's enabled/disabled state
            private static bool isobj4ItemsSourceDisabled = false;

            public HomeView_obj1_Bindings()
            {
            }

            public void Disable(int lineNumber, int columnNumber)
            {
                if (lineNumber == 15 && columnNumber == 144)
                {
                    isobj4ItemsSourceDisabled = true;
                }
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 4: // HomeView.xaml line 15
                        this.obj4 = (global::Windows.UI.Xaml.Controls.ListView)target;
                        break;
                    default:
                        break;
                }
            }

            // IDataTemplateComponent

            public void ProcessBindings(global::System.Object item, int itemIndex, int phase, out int nextPhase)
            {
                nextPhase = -1;
            }

            public void Recycle()
            {
                return;
            }

            // IHomeView_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
            }

            public bool SetDataRoot(global::System.Object newDataRoot)
            {
                if (newDataRoot != null)
                {
                    this.dataRoot = (global::Jellyfin.HomeView)newDataRoot;
                    return true;
                }
                return false;
            }

            public void Loading(global::Windows.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::Jellyfin.HomeView obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_ViewList(obj.ViewList, phase);
                    }
                }
            }
            private void Update_ViewList(global::System.Collections.ObjectModel.ObservableCollection<global::Jellyfin.Models.BaseItemDto> obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // HomeView.xaml line 15
                    if (!isobj4ItemsSourceDisabled)
                    {
                        XamlBindingSetters.Set_Windows_UI_Xaml_Controls_ItemsControl_ItemsSource(this.obj4, obj, null);
                    }
                }
            }
        }
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 0.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // HomeView.xaml line 13
                {
                    this.UsernameTextbox = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 3: // HomeView.xaml line 14
                {
                    global::Windows.UI.Xaml.Controls.Button element3 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)element3).Click += this.Logout_Click;
                }
                break;
            case 4: // HomeView.xaml line 15
                {
                    global::Windows.UI.Xaml.Controls.ListView element4 = (global::Windows.UI.Xaml.Controls.ListView)(target);
                    ((global::Windows.UI.Xaml.Controls.ListView)element4).ItemClick += this.MediaLibraries_SelectionChanged;
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 0.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            switch(connectionId)
            {
            case 1: // HomeView.xaml line 1
                {                    
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)target;
                    HomeView_obj1_Bindings bindings = new HomeView_obj1_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(this);
                    this.Bindings = bindings;
                    element1.Loading += bindings.Loading;
                    global::Windows.UI.Xaml.Markup.XamlBindingHelper.SetDataTemplateComponent(element1, bindings);
                }
                break;
            case 6: // HomeView.xaml line 25
                {                    
                    global::Windows.UI.Xaml.Controls.GridViewItem element6 = (global::Windows.UI.Xaml.Controls.GridViewItem)target;
                    HomeView_obj6_Bindings bindings = new HomeView_obj6_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(element6.DataContext);
                    element6.DataContextChanged += bindings.DataContextChangedHandler;
                    global::Windows.UI.Xaml.DataTemplate.SetExtensionInstance(element6, bindings);
                    global::Windows.UI.Xaml.Markup.XamlBindingHelper.SetDataTemplateComponent(element6, bindings);
                }
                break;
            }
            return returnValue;
        }
    }
}

