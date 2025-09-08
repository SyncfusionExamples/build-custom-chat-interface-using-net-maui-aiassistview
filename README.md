# build-custom-chat-interface-using-net-maui-aiassistview
This demo explains about how to create the custom chat interface using .NET MAUI AI AssistView(SfAIAssistView).

## Sample

```xaml

 <local:CustomAssistView x:Name="sfAIAssistView"
                            ShowHeader="{Binding IsHeaderVisible}"
                            HeaderTemplate="{StaticResource headerTemplate}"
                            AssistItems="{Binding AssistItems}"
                            EditorViewTemplate="{StaticResource editorTemplate}"
                            RequestCommand="{Binding AssistViewRequestCommand}"
                            StopRespondingCommand="{Binding StopRespondingCommand}">
        <local:CustomAssistView.ControlTemplate>
            <ControlTemplate>
                <ContentView BindingContext="{TemplateBinding BindingContext}">
                    <ContentView.Content>
                          ...
                    </ContentView.Content>
                </ContentView>
            </ControlTemplate>
        </local:CustomAssistView.ControlTemplate>
 </local:CustomAssistView>

CustomAssistView :

    public class CustomAssistView : Syncfusion.Maui.AIAssistView.SfAIAssistView
    {
        public static readonly BindableProperty AssistChatViewProperty =
       BindableProperty.Create(nameof(AssistChatView), typeof(AssistViewChat), typeof(CustomAssistView));

        public AssistViewChat AssistChatView
        {
            get { return (AssistViewChat)this.GetValue(AssistChatViewProperty); }
            set { this.SetValue(AssistChatViewProperty, value); }
        }
        protected override AssistViewChat CreateAssistChat()
        {
            AssistChatView = base.CreateAssistChat();
            return AssistChatView;
        }
    }

```

## Requirements to run the demo

To run the demo, refer to [System Requirements for .NET MAUI](https://help.syncfusion.com/maui/system-requirements)

## Troubleshooting:
### Path too long exception

If you are facing path too long exception when building this example project, close Visual Studio and rename the repository to short and build the project.

## License

Syncfusion® has no liability for any damage or consequence that may arise from using or viewing the samples. The samples are for demonstrative purposes. If you choose to use or access the samples, you agree to not hold Syncfusion® liable, in any form, for any damage related to use, for accessing, or viewing the samples. By accessing, viewing, or seeing the samples, you acknowledge and agree Syncfusion®'s samples will not allow you seek injunctive relief in any form for any claim related to the sample. If you do not agree to this, do not view, access, utilize, or otherwise do anything with Syncfusion®'s samples.
