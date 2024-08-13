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
            CardPayload = "{\"$schema\":\"http://adaptivecards.io/schemas/adaptive-card.json\",\"type\":\"AdaptiveCard\",\"version\":\"1.3\",\"body\":[{\"type\":\"ActionSet\",\"actions\":[{\"type\":\"Action.Submit\",\"title\":\"Autopilot Features\",\"data\":{\"selectedWizard\":\"AutoPilotTrackpad\",\"request\":\"AutoPilotTrackpadEnabled\"},\"iconUrl\":\"data:image/png;base64,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\"},{\"type\":\"Action.Submit\",\"title\":\"Configure Engines\",\"data\":{\"selectedWizard\":\"ConfigureEnginesSummary\",\"request\":\"EnginesInstalled;EngineData;EnginesSupported\"},\"iconUrl\":null},{\"type\":\"Action.Submit\",\"title\":\"Configure Gateways\",\"data\":{\"selectedWizard\":\"ConfigureMechanicalGauges\",\"request\":\"Gateways\"},\"iconUrl\":null},{\"type\":\"Action.Submit\",\"title\":\"Configure Helms\",\"data\":{\"selectedWizard\":\"ConfigureHelms\",\"request\":\"HelmCount\"},\"iconUrl\":null},{\"type\":\"Action.Submit\",\"title\":\"Configure Tanks\",\"data\":{\"selectedWizard\":\"ConfigureTanks\",\"request\":\"TankData;EnginesInstalled\"},\"iconUrl\":null},{\"type\":\"Action.Submit\",\"title\":\"Cruise\",\"data\":{\"selectedWizard\":\"Cruise\",\"request\":\"CruiseEnabled\"},\"iconUrl\":null},{\"type\":\"Action.Submit\",\"title\":\"Optional Engine Sensors\",\"data\":{\"selectedWizard\":\"OptionalSensors\",\"request\":\"EngineData\"},\"iconUrl\":null},{\"type\":\"Action.Submit\",\"title\":\"Stationkeeping\",\"data\":{\"selectedWizard\":\"Stationkeeping\",\"request\":\"StationkeepingEnabled\"},\"iconUrl\":null},{\"type\":\"Action.Submit\",\"title\":\"Steering Angle Source\",\"data\":{\"selectedWizard\":\"SteeringAngleSource\",\"request\":\"SteeringAngleSource\"},\"iconUrl\":null},{\"type\":\"Action.Submit\",\"title\":\"Tab Source\",\"data\":{\"selectedWizard\":\"TabSource\",\"request\":\"TabSource\"},\"iconUrl\":null}]}]}";
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
