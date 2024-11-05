using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace ExamBank
{
    /// <summary>
    /// 布尔值转换为可见性
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        ///当界面的绑定到DataContext中的属性发生变化时，会调用该方法，将绑定的bool值转换为界面需要的Visibility类型的值
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        ///当界面的Visibility值发生变化时，会调用该方法，将Visibility类型的值转换为bool值返回给绑定到DataContext中的属性
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Visibility v)
            {
                return false;
            }

            if (v == Visibility.Visible)
            {
                return true;
            }
            else
                return false;
        }
    }

    public class Question
    {

        public required string question { get; set; }
        public required string answer { get; set; }
        public bool isAnswerVisible { get; set; } = true;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> logs = new ObservableCollection<string>();
        private string filePath = "questions.json"; // 可以根据需要设置文件路径和名称  

        public MainWindow()
        {
            InitializeComponent();
            LogListBox.ItemsSource = logs;

            // 示例日志记录  
            Log("Application started.");
            load_data();
        }

        private void load_data()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    ObservableCollection<Question>? Items = JsonConvert.DeserializeObject<ObservableCollection<Question>>(json);
                    if (Items != null && Items.Count > 0)
                    {
                        foreach (Question q in Items)
                        {
                            lstQuestions.Items.Add(q);
                        }
                    }
                    
                    Log($"Data loaded successfully!");
                }
                catch (Exception ex)
                {
                    Log($"Error loading data: {ex.Message}");
                }
            }
        }

        private void Log(string message)
        {
            logs.Add(DateTime.Now.ToString("HH:mm:ss") + " - " + message);
            scroll_newest(LogListBox);
        }

        private void scroll_newest(ListBox lstbox)
        {
            lstbox.ScrollIntoView(lstbox.Items[lstbox.Items.Count - 1]);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtQuestion.Text))
            {
                Log($"Add: question:{txtQuestion.Text} answer:{txtAnswer.Text}");
                lstQuestions.Items.Add(new Question {question= txtQuestion.Text,answer= txtAnswer.Text });
                txtQuestion.Clear();
                txtAnswer.Clear();
                scroll_newest(lstQuestions);
            }
        }

        private void lstQuestions_mouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstQuestions.SelectedItem != null)
            {
                Question item = (Question)lstQuestions.SelectedItem;
                item.isAnswerVisible = !item.isAnswerVisible; // change visibility
                lstQuestions.Items.Refresh();
                //MessageBox.Show($"ind:{lstQuestions.Items.IndexOf(lstQuestions.SelectedValue)}");
                Log($"Double Clicked: question:{item.question} answer:{item.answer}");
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (lstQuestions.SelectedItems != null)
            {
                Log($"Del: {lstQuestions.SelectedValue}");
                lstQuestions.Items.Remove(lstQuestions.SelectedItem);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (lstQuestions.Items != null)
            {

                try
                {
                    string json = JsonConvert.SerializeObject(lstQuestions.Items, Formatting.Indented);
                    File.WriteAllText(filePath, json);

                    Log("Data saved successfully!");
                }
                catch (Exception ex)
                {
                    Log($"Error saving data: {ex.Message}");
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lstQuestions.Items != null)
            {

                try
                {
                    string json = JsonConvert.SerializeObject(lstQuestions.Items, Formatting.Indented);
                    File.WriteAllText(filePath, json);

                    Log("Data saved successfully!");
                }
                catch (Exception ex)
                {
                    Log($"Error saving data: {ex.Message}");
                }
            }
        }
    }
}