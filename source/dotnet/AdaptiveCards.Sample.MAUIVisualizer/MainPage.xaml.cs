using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Timers;
using AdaptiveCards.Rendering;
using AdaptiveCards.Rendering.MAUI;
using AdaptiveCards.Templating;
using Newtonsoft.Json;
using Style = Microsoft.Maui.Controls.Style;
using Visibility = Microsoft.Maui.Visibility;

namespace AdaptiveCards.Sample.MAUIVisualizer
{
    public partial class MainPage : ContentPage
    {
        private bool _dirty;

        public MainPage()
        {
            InitializeComponent();

            Application.Current.UserAppTheme = AppTheme.Light;


            var hostConfig = RendererHostConfigHelper.GetConfig();
            //var timer = new System.Timers.Timer(TimeSpan.FromSeconds(1));
            //timer.AutoReset = true;
            //timer.Elapsed += Timer_Tick;
            //timer.Start();

            Renderer = new AdaptiveCardRenderer()
            {
                Resources = Resources,
                HostConfig = hostConfig
            };
            CardPayload = "{\"$schema\":\"http://adaptivecards.io/schemas/adaptive-card.json\",\"version\":\"1.3\",\"type\":\"AdaptiveCard\",\"body\":[{\"type\":\"Container\",\"items\":[{\"type\":\"ColumnSet\",\"columns\":[{\"type\":\"Column\",\"width\":\"stretch\",\"items\":[{\"type\":\"TextBlock\",\"size\":\"Medium\",\"weight\":\"Bolder\",\"text\":\"${toUpper(Resource.EngineConfiguration)}\",\"fontType\":\"Monospace\"},{\"type\":\"TextBlock\",\"text\":\"${if(Data.manager.EnginesSupported == 1, '1/4', '1/?')}\",\"isSubtle\":true,\"fontType\":\"Monospace\"}]},{\"type\":\"Column\",\"width\":\"auto\",\"verticalContentAlignment\":\"Center\",\"items\":[{\"type\":\"Image\",\"url\":\"${Images.HelpIcon}\",\"size\":\"Small\"}],\"selectAction\":{\"type\":\"Action.ToggleVisibility\",\"targetElements\":[\"helpText\"]}}]},{\"type\":\"TextBlock\",\"wrap\":true,\"text\":\"${Resource.EngineConfigHelpText}\",\"isVisible\":false,\"id\":\"helpText\",\"separator\":true}],\"bleed\":true,\"style\":\"emphasis\"},{\"type\":\"TextBlock\",\"text\":\"${if(Data.manager.EnginesSupported == 1, Resource.EnginesSupportedOne, Resource.EnginesSupportedTwo)}\",\"wrap\":true,\"$when\":\"${Data.manager.EnginesSupported < 3}\",\"spacing\":\"Large\"},{\"type\":\"Container\",\"items\":[{\"type\":\"TextBlock\",\"text\":\"${Resource.EnginesSupportedThree}\",\"wrap\":true},{\"type\":\"Input.ChoiceSet\",\"choices\":[{\"title\":\"1\",\"value\":\"1\"},{\"title\":\"2\",\"value\":\"2\"},{\"title\":\"3\",\"value\":\"3\"},{\"title\":\"4\",\"value\":\"4\"}],\"style\":\"expanded\",\"id\":\"engines\",\"value\":\"3\"}],\"$when\":\"${Data.manager.EnginesSupported == 3}\",\"spacing\":\"Large\"},{\"type\":\"TextBlock\",\"text\":\"Do all of the installed engines share the same type and model?\",\"$when\":\"${Data.manager.EnginesSupported > 1}\",\"spacing\":\"Large\",\"wrap\":true,\"isVisible\":false},{\"type\":\"ActionSet\",\"isVisible\":false,\"actions\":[{\"type\":\"Action.Submit\",\"title\":\"Same Engine Model\",\"data\":{\"nextCard\":\"engine1SerialCard\",\"engines\":\"${Data.manager.EnginesSupported}\",\"allSame\":true,\"engine1Serial\":\"${if(count(split(Data.manager.serials, ';')) > 1 && !contains(split(Data.manager.serials, ';'), 'master'), split(Data.manager.serials,';')[0], '')}\",\"engine2Serial\":\"${if(count(split(Data.manager.serials, ';')) > 1, split(Data.manager.serials,';')[1], '')}\",\"engine3Serial\":\"${if(count(split(Data.manager.serials, ';')) > 2, split(Data.manager.serials,';')[2], '')}\",\"engine4Serial\":\"${if(count(split(Data.manager.serials, ';')) > 3, split(Data.manager.serials,';')[3], '')}\",\"engine1Type\":\"\",\"engine2Type\":\"\",\"engine3Type\":\"\",\"engine4Type\":\"\",\"engine1Model\":\"\",\"engine2Model\":\"\",\"engine3Model\":\"\",\"engine4Model\":\"\"},\"style\":\"positive\"},{\"type\":\"Action.Submit\",\"title\":\"Multiple Models\",\"data\":{\"nextCard\":\"engine1SerialCard\",\"engines\":\"${Data.manager.EnginesSupported}\",\"allSame\":false,\"engine1Serial\":\"${if(count(split(Data.manager.serials, ';')) > 1 && !contains(split(Data.manager.serials, ';'), 'master'), split(Data.manager.serials,';')[0], '')}\",\"engine2Serial\":\"${if(count(split(Data.manager.serials, ';')) > 1, split(Data.manager.serials,';')[1], '')}\",\"engine3Serial\":\"${if(count(split(Data.manager.serials, ';')) > 2, split(Data.manager.serials,';')[2], '')}\",\"engine4Serial\":\"${if(count(split(Data.manager.serials, ';')) > 3, split(Data.manager.serials,';')[3], '')}\",\"engine1Type\":\"\",\"engine2Type\":\"\",\"engine3Type\":\"\",\"engine4Type\":\"\",\"engine1Model\":\"\",\"engine2Model\":\"\",\"engine3Model\":\"\",\"engine4Model\":\"\"}}],\"$when\":\"${Data.manager.EnginesSupported > 1}\",\"spacing\":\"Large\"},{\"type\":\"ActionSet\",\"$when\":\"${Data.manager.EnginesSupported > 0}\",\"actions\":[{\"type\":\"Action.Submit\",\"$when\":\"${Data.manager.EnginesSupported == 3}\",\"title\":\"${Resource.Next}\",\"data\":{\"nextCard\":\"engine1SerialCard\",\"allSame\":true,\"engine1Serial\":\"${if(count(split(Data.manager.serials, ';')) > 1 && !contains(split(Data.manager.serials, ';')[0], 'master'), split(Data.manager.serials,';')[0], '')}\",\"engine2Serial\":\"${if(count(split(Data.manager.serials, ';')) > 1, split(Data.manager.serials,';')[1], '')}\",\"engine3Serial\":\"${if(count(split(Data.manager.serials, ';')) > 2, split(Data.manager.serials,';')[2], '')}\",\"engine4Serial\":\"${if(count(split(Data.manager.serials, ';')) > 3, split(Data.manager.serials,';')[3], '')}\",\"engine1Type\":\"\",\"engine2Type\":\"\",\"engine3Type\":\"\",\"engine4Type\":\"\",\"engine1Model\":\"\",\"engine2Model\":\"\",\"engine3Model\":\"\",\"engine4Model\":\"\"},\"style\":\"positive\"},{\"type\":\"Action.Submit\",\"$when\":\"${Data.manager.EnginesSupported < 3}\",\"title\":\"${Resource.Next}\",\"data\":{\"nextCard\":\"engine1SerialCard\",\"engines\":\"${Data.manager.EnginesSupported}\",\"allSame\":true,\"engine1Serial\":\"${if(count(split(Data.manager.serials, ';')) > 1 && !contains(split(Data.manager.serials, ';')[0], 'master'), split(Data.manager.serials,';')[0], '')}\",\"engine2Serial\":\"${if(count(split(Data.manager.serials, ';')) > 1, split(Data.manager.serials,';')[1], '')}\",\"engine3Serial\":\"${if(count(split(Data.manager.serials, ';')) > 2, split(Data.manager.serials,';')[2], '')}\",\"engine4Serial\":\"${if(count(split(Data.manager.serials, ';')) > 3, split(Data.manager.serials,';')[3], '')}\",\"engine1Type\":\"\",\"engine2Type\":\"\",\"engine3Type\":\"\",\"engine4Type\":\"\",\"engine1Model\":\"\",\"engine2Model\":\"\",\"engine3Model\":\"\",\"engine4Model\":\"\"},\"style\":\"positive\"},{\"type\":\"Action.Submit\",\"title\":\"${Resource.Cancel}\",\"data\":{\"nextCard\":\"exit\"}}],\"spacing\":\"Large\"}]}";
        }

        public AdaptiveCardRenderer Renderer { get; set; }

        private void Timer_Tick(object? sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_dirty)
            {
                _dirty = false;
                RenderCard();
            }
        }

        public string CardPayload
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        private void RenderCard()
        {
            cardError.Children.Clear();
            cardGrid.Opacity = 0.65;

            string expandedPayload = CardPayload;

            try
            {
                // don't throw error, but should affect work flow and performance.
                // transformer -> has to have errors
                var template = new AdaptiveCardTemplate(CardPayload);

                // Example usage:
                Object host = new
                {
                    widgetSize = "small",
                    hostTheme = "dark"
                };

                //var context = new EvaluationContext
                //{
                //    Root = templateData,
                //    Host = host
                //};

                // Create a data binding context, and set its $root property to the
                // data object to bind the template to
                // var context = new ACData.EvaluationContext();
                // context.$root = {
                //     "name": "Mickey Mouse"
                // };

                //expandedPayload = template.Expand(context);
            }

            catch (Exception e)
            {
                // if an exception thrown, we parse and render cards as it is
                ShowError(e);
                expandedPayload = CardPayload;
            }

            try
            {
                AdaptiveCardParseResult parseResult = AdaptiveCard.FromJson(expandedPayload);

                AdaptiveCard card = parseResult.Card;

                /*
                if (!_stylesAdded)
                {
                    // Example on how to override the Action Positive and Destructive styles
                    Style positiveStyle = new Style(typeof(Button));
                    positiveStyle.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Green));
                    Style otherStyle = new Style(typeof(Button));
                    otherStyle.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Yellow));
                    otherStyle.Setters.Add(new Setter(Button.ForegroundProperty, Brushes.Red));

                    Renderer.Resources.Add("Adaptive.Action.positive", positiveStyle);
                    Renderer.Resources.Add("Adaptive.Action.other", otherStyle);

                    _stylesAdded = true;
                }
                */

                RenderedAdaptiveCard renderedCard = Renderer.RenderCard(card);
                // TODO: should we have an option to render fallback card instead of exception?

                // Wire up click handler
                renderedCard.OnAction += OnAction;

                renderedCard.OnMediaClicked += OnMediaClick;

                cardGrid.Opacity = 1;
                cardGrid.Children.Clear();
                cardGrid.Children.Add(renderedCard.FrameworkElement);

                // Report any warnings
                var allWarnings = parseResult.Warnings.Union(renderedCard.Warnings);
                foreach (var warning in allWarnings)
                {
                    ShowWarning(warning.Message);
                }
            }
            catch (AdaptiveRenderException ex)
            {
                var fallbackCard = new Label
                {
                    Text = ex.CardFallbackText ?? "Sorry, we couldn't render the card"
                };

                cardGrid.Children.Add(fallbackCard);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void OnAction(RenderedAdaptiveCard sender, AdaptiveActionEventArgs e)
        {
            if (e.Action is AdaptiveOpenUrlAction openUrlAction)
            {
                Process.Start(openUrlAction.Url.AbsoluteUri);
            }
            else if (e.Action is AdaptiveShowCardAction showCardAction)
            {
                // Action.ShowCard can be rendered inline automatically, or in "popup" mode
                // If the Host Config is set to Popup mode, then the app needs to show it
                if (Renderer.HostConfig.Actions.ShowCard.ActionMode == ShowCardActionMode.Popup)
                {
                    //var dialog = new ShowCardWindow(showCardAction.Title, showCardAction, Resources);
                    //dialog.Owner = this;
                    //dialog.ShowDialog();
                }
            }
            else if (e.Action is AdaptiveSubmitAction submitAction)
            {
                var inputs = sender.UserInputs.AsJson();

                // Merge the Action.Submit Data property with the inputs
                inputs.Merge(submitAction.Data);

                this.DisplayAlert("SubmitAction", JsonConvert.SerializeObject(inputs, Formatting.Indented), "OK");
                //MessageBox.Show(this, JsonConvert.SerializeObject(inputs, Formatting.Indented), "SubmitAction");
            }
            else if (e.Action is AdaptiveExecuteAction executeAction)
            {
                var inputs = sender.UserInputs.AsJson();

                // Merge the Action.Execute Data property with the inputs
                inputs.Merge(executeAction.Data);

                this.DisplayAlert("ExecuteAction",
                    JsonConvert.SerializeObject(inputs, Formatting.Indented) + "\nverb: " + executeAction.Verb, "OK");
                //MessageBox.Show(this, JsonConvert.SerializeObject(inputs, Formatting.Indented) + "\nverb: " + executeAction.Verb, "ExecuteAction");
            }
        }

        private void OnMediaClick(RenderedAdaptiveCard sender, AdaptiveMediaEventArgs e)
        {
            this.DisplayAlert("Host received a Media", JsonConvert.SerializeObject(e.Media), "OK");
            //MessageBox.Show(this, JsonConvert.SerializeObject(e.Media), "Host received a Media");
        }

        private void ShowWarning(string message)
        {
            var textBlock = new Label
            {
                Text = "WARNING: " + message,
                //Style = Resources["Warning"] as Style
            };
            var button = new Frame { Content = textBlock };
            cardError.Children.Add(button);
        }

        private void ShowError(Exception err)
        {
            var textBlock = new Label
            {
                Text = err.Message + "\nSource : " + err.Source,
                //Style = Resources["Error"] as Style
            };
            var button = new Frame { Content = textBlock };
            //button.GestureRecognizers.Add(new TapGestureRecognizer(){Command = new Command(Button_Click)});
            cardError.Children.Add(button);

            var iPos = err.Message.IndexOf("line ");
            if (iPos > 0)
            {
                iPos += 5;
                var iEnd = err.Message.IndexOf(",", iPos);

                var line = 1;
                if (int.TryParse(err.Message.Substring(iPos, iEnd - iPos), out line))
                {
                    if (line == 0) line = 1;
                    iPos = err.Message.IndexOf("position ");
                    if (iPos > 0)
                    {
                        iPos += 9;
                        iEnd = err.Message.IndexOf(".", iPos);
                        var position = 0;
                        //if (int.TryParse(err.Message.Substring(iPos, iEnd - iPos), out position))
                            //_errorLine = textBox.Document.GetLineByNumber(Math.Min(line, textBox.Document.LineCount));
                    }
                }
            }
        }

        private void _OnMissingInput(object sender, MissingInputEventArgs args)
        {
            DisplayAlert("Error", "Required input is missing.", "OK");
            //MessageBox.Show("Required input is missing.");
            args.FrameworkElement.Focus();
        }

        //private void Button_Click()
        //{
        //    if (_errorLine != null)
        //        textBox.Select(_errorLine.Offset, _errorLine.Length);
        //}

        //private void loadButton_Click(object sender, RoutedEventArgs e)
        //{
        //    string cardPayload;
        //    OpenFileDialogForJson(out cardPayload);
        //    CardPayload = cardPayload;
        //}

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var binding = new CommandBinding(NavigationCommands.GoToPage, GoToPage, CanGoToPage);
        //    // Register CommandBinding for all windows.
        //    CommandManager.RegisterClassCommandBinding(typeof(Window), binding);
        //}


        //private void GoToPage(object sender, ExecutedRoutedEventArgs e)
        //{
        //    if (e.Parameter is string)
        //    {
        //        var name = e.Parameter as string;
        //        if (!string.IsNullOrWhiteSpace(name))
        //            Process.Start(name);
        //    }
        //}

        //private void CanGoToPage(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = true;
        //}

        //private async void viewImage_Click(object sender, RoutedEventArgs e)
        //{
        //    var supportsInteractivity = Renderer.HostConfig.SupportsInteractivity;

        //    try
        //    {
        //        this.IsEnabled = false;

        //        //Disable interactivity to remove inputs and actions from the image
        //        Renderer.HostConfig.SupportsInteractivity = false;

        //        var renderedCard = await Renderer.RenderCardToImageAsync(AdaptiveCard.FromJson(CardPayload).Card, false);
        //        using (var imageStream = renderedCard.ImageStream)
        //        {
        //            new ViewImageWindow(renderedCard.ImageStream).Show();
        //        }
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Failed to render image");
        //    }
        //    finally
        //    {
        //        Renderer.HostConfig.SupportsInteractivity = supportsInteractivity;
        //        this.IsEnabled = true;
        //    }
        //}

        //private void speak_Click(object sender, RoutedEventArgs e)
        //{
        //    var result = AdaptiveCard.FromJson(CardPayload);
        //    var card = result.Card;

        //}

        //private string FixSSML(string speak)
        //{
        //    var sb = new StringBuilder();
        //    sb.AppendLine("<speak version=\"1.0\"");
        //    sb.AppendLine(" xmlns =\"http://www.w3.org/2001/10/synthesis\"");
        //    sb.AppendLine(" xml:lang=\"en-US\">");
        //    sb.AppendLine(speak);
        //    sb.AppendLine("</speak>");
        //    return sb.ToString();
        //}

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            _dirty = true;
        }

        //private void toggleOptions_Click(object sender, RoutedEventArgs e)
        //{
        //    hostConfigEditor.Visibility = hostConfigEditor.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        //}

        public AdaptiveHostConfig HostConfig
        {
            get => Renderer.HostConfig;
            set
            {
                hostConfigerror.Children.Clear();
                Renderer.HostConfig = value;
                _dirty = true;
                if (value != null)
                {
                    var props = value.GetType()
                        .GetRuntimeProperties()
                        .Where(p => typeof(AdaptiveConfigBase).IsAssignableFrom(p.PropertyType));

                    foreach (var x in value.AdditionalData)
                    {
                        var textBlock = new Label
                        {
                            Text = $"Unknown property {x.Key}",
                            Style = Resources["Warning"] as Style
                        };
                        hostConfigerror.Children.Add(textBlock);
                    }
                }
            }
        }

        //private void hostConfigs_Selected(object sender, RoutedEventArgs e)
        //{
        //    HostConfig = AdaptiveHostConfig.FromJson(File.ReadAllText((string)((ComboBoxItem)hostConfigs.SelectedItem).Tag));
        //    hostConfigEditor.SelectedObject = Renderer.HostConfig;
        //}

        //private void loadConfig_Click(object sender, RoutedEventArgs e)
        //{
        //    var dlg = new OpenFileDialog();
        //    dlg.DefaultExt = ".json";
        //    dlg.Filter = "Json documents (*.json)|*.json";
        //    var result = dlg.ShowDialog();
        //    if (result == true)
        //    {
        //        HostConfig = AdaptiveHostConfig.FromJson(File.ReadAllText(dlg.FileName));
        //    }
        //}

        //private void saveConfig_Click(object sender, RoutedEventArgs e)
        //{
        //    var dlg = new SaveFileDialog();
        //    dlg.DefaultExt = ".json";
        //    dlg.Filter = "Json documents (*.json)|*.json";
        //    var result = dlg.ShowDialog();
        //    if (result == true)
        //    {
        //        var json = JsonConvert.SerializeObject(Renderer.HostConfig, Formatting.Indented);
        //        File.WriteAllText(dlg.FileName, json);
        //    }
        //}

        //private void HostConfigEditor_OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        //{
        //    _dirty = true;
        //}

        //private void XceedCheckBox_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    //Renderer.UseDefaultElementRenderers();
        //    _dirty = true;
        //}

        //private void XceedCheckBox_Checked(object sender, RoutedEventArgs e)
        //{
        //    //Renderer.UseXceedElementRenderers();
        //    _dirty = true;
        //}

        //private void templateData_Added(object sender, EventArgs e)
        //{
        //    var textEditor = sender as TextEditor;
        //    templateData = textEditor.Text;
        //    _dirty = true;
        //}

        //private void OpenFileDialogForJson(out string output)
        //{
        //    var dlg = new OpenFileDialog();
        //    dlg.DefaultExt = ".json";
        //    dlg.Filter = "Json documents (*.json)|*.json";
        //    output = "";
        //    if (dlg.ShowDialog() == true)
        //    {
        //        output = File.ReadAllText(dlg.FileName).Replace("\t", "  ");
        //        _dirty = true;
        //    }
        //}

        //private void loadTemplateDataButton_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialogForJson(out templateData);
        //    if (templateData.Length == 0)
        //    {
        //        templateData = null;
        //    }
        //    templateDataTextBox.Text = templateData;
        //}


        private void ReloadButton_OnClicked(object? sender, EventArgs e)
        {
            RenderCard();
        }
    }

}
