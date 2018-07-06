using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsOnRaspi_MediaCreatorTool.Classes;

namespace WindowsOnRaspi_MediaCreatorTool.Views
{
    /// <summary>
    /// Interaction logic for SelectAppLanguagePage.xaml
    /// </summary>
    public partial class SelectAppLanguagePage : Page
    {
        // Variables
        private MainWindow window;
        private dynamic langjson;
        private dynamic sourcejson;
        private List<LangListItem> langList = new List<LangListItem>();

        public SelectAppLanguagePage(MainWindow window, dynamic lang)
        {
            InitializeComponent();

            this.window = window;
            this.window.isLockdownMode = false;
            this.window.forceCleanUp = false;
            this.langjson = lang;

            if (langjson != null && langjson.pages.selectAppLangaugePage != null)
            {
                titleTextBlock.Text = langjson.pages.selectAppLangaugePage.title;
                selectLanguageTextBlock.Text = langjson.pages.selectAppLangaugePage.selectALanguageComboBoxLabel;

                quitButton.Content = langjson.common_elements.quit_button;
                continueButton.Content = langjson.common_elements.continue_button;
            }

            refreshLanguages();

            Debug.WriteLine("Windows OS Version: " + Environment.OSVersion.Version.ToString());
        }

        private void refreshLanguages() {
            string path = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            string sourcejsonpath = System.IO.Path.Combine(path, "Data", "Lang", "source.json");
            string sourcejsonstring = null;
            if (File.Exists(sourcejsonpath))
            {
                using (StreamReader r = new StreamReader(sourcejsonpath))
                {
                    sourcejsonstring = r.ReadToEnd();
                    sourcejson = Newtonsoft.Json.JsonConvert.DeserializeObject(sourcejsonstring);

                    for (int i = 0; i < sourcejson.lang_list.Count; i++)
                    {
                        string title = sourcejson.lang_list[i].title;
                        string lang = sourcejson.lang_list[i].lang;
                        string link = sourcejson.lang_list[i].link;
                        langList.Add(new LangListItem(title, lang, link));
                        languageCombobox.Items.Add(langList.ToArray()[i].title);
                    }

                    languageCombobox.SelectedIndex = 0;
                }
            }
        }

        private void quitButton_Click(object sender, RoutedEventArgs e)
        {
            window.Close();
        }

        private void continueButton_Click(object sender, RoutedEventArgs e)
        {
            window.MainFrame.Content = new StartUpPage(window, langjson);
        }

        private void languageCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = (ComboBox)sender;
            var langlistArray = langList.ToArray();
            string path = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            string sourceLangjsonpath = System.IO.Path.Combine(path, "Data", "Lang", langlistArray[combobox.SelectedIndex].link);
            string sourcejsonstring = null;
            if (File.Exists(sourceLangjsonpath))
            {
                using (StreamReader r = new StreamReader(sourceLangjsonpath))
                {
                    sourcejsonstring = r.ReadToEnd();
                    langjson = Newtonsoft.Json.JsonConvert.DeserializeObject(sourcejsonstring);

                    if (langjson != null && langjson.pages.selectAppLangaugePage != null)
                    {
                        window.SetLangJson(langjson);
                        titleTextBlock.Text = langjson.pages.selectAppLangaugePage.title;
                        selectLanguageTextBlock.Text = langjson.pages.selectAppLangaugePage.selectALanguageComboBoxLabel;

                        quitButton.Content = langjson.common_elements.quit_button;
                        continueButton.Content = langjson.common_elements.continue_button;
                    }

                }
            }
        }
    }
}
