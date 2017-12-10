﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace NuclearProject
{
    /// <summary>
    /// Логика взаимодействия для EditQuestionWindow.xaml
    /// </summary>
    public partial class EditQuestionWindow : Window
    {
        int id;
        List<DataLoad.RootObject> data;
        DataLoad.RootObject QObject;
        public EditQuestionWindow(int id)
        {
            InitializeComponent();
            data = DataLoad.LoadDataFromJson();
            this.id = id;
            //значения выбранного вопроса
            var editableQuestion = data.Where(question => question.ID == this.id);

            QObject = editableQuestion.First();

            var QTheme = editableQuestion.Select(q => q.Theme).First();
            var QType = editableQuestion.Select(q => q.TestType).First();
            var QComplexity = Int32.Parse(editableQuestion.Select(q => q.Complexity).First());
            var QText = editableQuestion.Select(q => q.Question).First();
            var answers = editableQuestion.Select(q => q.Answers).First(); 
            

            var types = data.Select(question => question.TestType).Distinct();
            var allThemes = data.Select(question => question.Theme).Distinct();
            string answersString = "";
            string correctAnswer = "";

            //устанавливаем дефолтные значения 
            QuestionText.Text = QText;
            foreach (var type in types)
            {
                TestTypes.Items.Add(type);
                TestTypes.SelectedIndex = TestTypes.Items.IndexOf(QType);
            }

            foreach (var theme in allThemes)
            {
                Themes.Items.Add(theme);
                Themes.SelectedIndex = Themes.Items.IndexOf(QTheme);
            }

            answersString = String.Join(";", answers.Select(answer => answer.Text));
            correctAnswer = answers.Where(answer => answer.isRight == 1).Select(answer => answer.Text).First();

            Answers.Text = answersString;
            CorrectAnswer.Text = correctAnswer;
            Complexity.SelectedIndex = QComplexity - 1;

        }

        private void Update_Question_Click(object sender, RoutedEventArgs e)
        {
            //var textBoxText = winBody.Children.OfType<StackPanel>().First();
            //Console.WriteLine(textBoxText);
            //if (textBoxText)
            //{
            //    MessageBox.Show("Проверьте заполнение обязательных полей!", "Ошибка");
            //    return;
            //}

            string testType = TestTypes.Text.Trim();
            string questionText = QuestionText.Text.Trim();
            string theme = Themes.Text.Trim();
            string[] answers = Answers.Text.Trim().Split(';');
            string correctAnswer = CorrectAnswer.Text.Trim();
            string complexity = Complexity.Text;

            var answerList = new List<DataLoad.Answer>();

 
            if (!answers.Contains(correctAnswer))
            {
                MessageBox.Show("Верный ответ не совпадает ни с одним из возможных!", "Ошибка");
                return;
            }

            foreach (string answerText in answers)
            {
                int isRight = 0;
                if (answerText == correctAnswer)
                {
                    isRight = 1;
                }
                answerList.Add(new DataLoad.Answer(answerText, isRight));
            }

            QObject.TestType = testType;
            QObject.Question = questionText;
            QObject.Theme = theme;
            QObject.Complexity = complexity;
            QObject.Answers = answerList;
            DataLoad.SaveDataToJson(data);
            Close();
            MessageBox.Show("Вопрос обновлен!", "Сообщение");
        }

        private void TestTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Themes.Items.Clear();
            string selectedType = TestTypes.SelectedValue.ToString();
            var themes = data.Where(question => question.TestType == selectedType).Select(question => question.Theme).Distinct();
            foreach (var item in themes)
            {
                Themes.Items.Add(item);
            }
        }
    }
}