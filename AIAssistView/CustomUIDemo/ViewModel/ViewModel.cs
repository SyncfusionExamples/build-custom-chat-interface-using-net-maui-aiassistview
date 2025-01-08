using Syncfusion.Maui.AIAssistView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CustomUIDemo
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region Field
        private ObservableCollection<IAssistItem> assistItems;
        private bool _isLoading = true;
        private bool isResponseVisible = false;
        private string _htmlContent;
        private string defaultResponseText = "Please connect to your preferred AI service for real-time queries.";
        internal AzureAIService azureAIService;
        private bool enableSendIcon;
        private bool isAttachmentPopupOpen;
        private IAssistItem requestItem;
        private string inputText;
        private bool isHeaderVisible = true;
        private bool isNewChatVisible = true;
        private bool isHeaderContentVisible = true;
        #endregion

        #region Constructor
        public ViewModel()
        {
            this.azureAIService = new AzureAIService();
            this.assistItems = new ObservableCollection<IAssistItem>();
            this.AssistViewRequestCommand = new Command<object>(ExecuteRequestCommand);
            this.CloseCommand = new Command(CloseContent);
            this.CopyCommand = new Command(ExecuteCopyCommand);
            this.RetryCommand = new Command(ExecuteRetryCommand);
            this.LoadedCommand = new Command(ExecuteLoadCommand);
            this.StopRespondingCommand = new Command(ExecuteStopResponding);

            this.AttachmentButtonCommand = new Command(ShowAttachmentPopup);
            this.SendButtonCommand = new Command(ExecuteSendButtonCommand);
            this.NewChatTappedCommand = new Command<object>(ExecuteNewChatTappedCommand);
        }

        #endregion

        #region Properties
        public ICommand CloseCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand RetryCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand AssistViewRequestCommand { get; set; }

        public ICommand StopRespondingCommand { get; set; }

        public ICommand AttachmentButtonCommand { get; set; }
        public ICommand SendButtonCommand { get; set; }

        public ICommand NewChatTappedCommand { get; set; }
        public ObservableCollection<IAssistItem> AssistItems
        {
            get => this.assistItems;
            set
            {
                this.assistItems = value;
                RaisePropertyChanged(nameof(AssistItems));
            }
        }

        public string InputText
        {
            get
            {
                return this.inputText;
            }
            set
            {
                this.inputText = value;
                RaisePropertyChanged(nameof(InputText));
            }
        }

        public object CurrentRequest
        {
            get; set;
        }

        public bool CancelResponse { get; set; }
        public bool IsResponseVisible
        {
            get => isResponseVisible;
            set
            {
                isResponseVisible = value;
                RaisePropertyChanged(nameof(IsResponseVisible));
            }
        }

        public string HtmlContent
        {
            get => _htmlContent;
            set
            {
                _htmlContent = value;
                RaisePropertyChanged(nameof(HtmlContent));
                RaisePropertyChanged(nameof(TitleText));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                RaisePropertyChanged(nameof(IsLoading));
            }
        }

        public string TitleText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(HtmlContent))
                    return "Syncfusion AI";

                var cleanedResponse = RemoveHtmlTags(HtmlContent);

                cleanedResponse = cleanedResponse.Replace("\n", " , ").Trim();

                var words = cleanedResponse.Split(new[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);

                var firstFewWords = words.Take(3).Aggregate((current, next) => current + " " + next);

                var title = string.IsNullOrWhiteSpace(firstFewWords) ? "Syncfusion AI" : firstFewWords.Trim();

                if (words.Length > 3)
                {
                    title += "...";
                }

                return title;
            }
        }
        
        public bool EnableSendIcon
        {
            get
            {
                return enableSendIcon;
            }
            set
            {
                enableSendIcon = value;
                RaisePropertyChanged(nameof(EnableSendIcon));
            }
        }

        public bool IsAttachmentPopupOpen
        {
            get
            {
                return isAttachmentPopupOpen;
            }
            set
            {

                isAttachmentPopupOpen = value;
                RaisePropertyChanged(nameof(IsAttachmentPopupOpen));
            }
        }

        public bool IsHeaderVisible
        {
            get
            {
                return isHeaderVisible;
            }
            set
            {
                isHeaderVisible = value;
                RaisePropertyChanged(nameof(IsHeaderVisible));
            }
        }

        public bool IsNewChatVisible
        {
            get { return isNewChatVisible; }
            set
            {
                isNewChatVisible = value;
                RaisePropertyChanged(nameof(IsNewChatVisible));
            }
        }

        public bool IsHeaderContentVisible
        {
            get
            {
                return isHeaderContentVisible;
            }
            set
            {
                isHeaderContentVisible = value;
                RaisePropertyChanged(nameof(IsHeaderContentVisible));
            }
        }
        #endregion

        private void ShowAttachmentPopup()
        {
            IsAttachmentPopupOpen = true;
        }
        private void ExecuteStopResponding()
        {
            // logic to handle the Stop Responding action
            this.CancelResponse = true;

            AssistItem responseItem = new AssistItem();
            responseItem.Text = "You canceled the response";
            responseItem.ShowAssistItemFooter = false;
            this.AssistItems.Add(responseItem);
            HtmlContent = "You canceled the response";
            IsLoading = false;
            this.EnableSendIcon = !string.IsNullOrEmpty(this.InputText);
        }
        private async void ExecuteRequestCommand(object obj)
        {
            IsHeaderVisible = false;
            IsResponseVisible = true;
            requestItem = (obj as Syncfusion.Maui.AIAssistView.RequestEventArgs)!.RequestItem;
            await this.GetResult(requestItem).ConfigureAwait(true);
        }

        private void ExecuteLoadCommand(object obj)
        {
            azureAIService = new AzureAIService();
        }

        private async void ExecuteRetryCommand(object obj)
        {
            HtmlContent = string.Empty;
            IsLoading = true;
            requestItem = CurrentRequest as IAssistItem;
            await this.GetResult(requestItem).ConfigureAwait(true);
        }

        private async void ExecuteNewChatTappedCommand(object obj)
        {
            await Task.Delay(100);
            this.IsNewChatVisible = true;
            this.IsHeaderContentVisible = true;
            this.IsHeaderVisible = true;
            this.InputText = string.Empty;
            this.AssistItems.Clear();
        }
        private string GetUserAIPrompt(string userPrompt)
        {
            string userQuery = $"Given User query: {userPrompt}." +
                        $"\nSome conditions need to follow:" +
                        $"\nGive heading of the topic and simplified answer in 4 points with numbered format" +
                        $"\nGive as string alone" +
                        $"\nRemove ** and remove quotes if it is there in the string.";
            return userQuery;
        }

        private async void ExecuteSendButtonCommand()
        {
            IsHeaderVisible = false;
            IsResponseVisible = true;
            await Task.Delay(100);
            var text = this.InputText;
            requestItem = new AssistItem()
            {
                Text = text,
                IsRequested = true
            };

            await Task.Delay(2);
            this.AssistItems.Add(requestItem);
            this.InputText = string.Empty;

            await this.GetResult(requestItem).ConfigureAwait(true);
        }
        private async Task GetResult(object inputQuery)
        {
            IsLoading = true;
            await Task.Delay(3000).ConfigureAwait(true);
            AssistItem request = (AssistItem)inputQuery;
            if (request != null)
            {
                var userAIPrompt = this.GetUserAIPrompt(request.Text);
                string response = await azureAIService!.GetResultsFromAI(request.Text, userAIPrompt).ConfigureAwait(true);
                response = response.Replace("\n", "<br>");

                AssistItem responseItem = new AssistItem() { Text = $"I've created a response for your request '{request.Text}'. Please refer to it and let me know if you need any further edits or changes!.", ShowAssistItemFooter = false };
                if (response.Equals(defaultResponseText))
                {
                    responseItem.Text = defaultResponseText;
                }
                
                if (!CancelResponse)
                {
                    responseItem.RequestItem = inputQuery;
                    CurrentRequest = inputQuery;
                    HtmlContent = response;
                    await Task.Delay(1000);
                    this.AssistItems.Add(responseItem);
                    this.EnableSendIcon = !string.IsNullOrEmpty(this.InputText);
                }
                
                CancelResponse = false;
            }
            IsLoading = false;
        }

        private void CloseContent()
        {
            InputText = string.Empty;
            IsResponseVisible = false;
            IsHeaderContentVisible = false;
            IsHeaderVisible = true;
            IsNewChatVisible = true;
        }

        private async void ExecuteCopyCommand(object obj)
        {
            if (obj is string text)
            {
                text = Regex.Replace(text, "<.*?>|&nbsp;", string.Empty);
                await Clipboard.SetTextAsync(text);
            }
        }

        private string RemoveHtmlTags(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.Replace("<br>", " , ");
            input = Regex.Replace(input, "<.*?>", string.Empty);
            input = input.Replace("&nbsp;", " ");
            input = Regex.Replace(input, @"\s+", " ").Trim();

            return input;
        }


        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
