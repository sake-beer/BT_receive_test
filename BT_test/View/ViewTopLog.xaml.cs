using System;
using System.Windows;
using BT_test.Model;
using BT_test.ViewModel;

namespace BT_test.View
{
    public partial class ViewTopLog : Window
    {
        public ViewTopLog(TopModel model)
        {
            DataContext = new ViewModelTopLog(model, LogUpdate);
            InitializeComponent();
        }

        public void LogUpdate()
        {
            Dispatcher.Invoke((Action)(() =>
            {
                TextBoxLog.ScrollToEnd();
            }));
        }
    }
}
