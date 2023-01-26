using System.Windows;
using BT_test.Model;
using BT_test.View;

namespace BT_test
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            TopModel model = new TopModel();


            ViewTopLog viewTopLog = new ViewTopLog(model);



        }

    }
}
