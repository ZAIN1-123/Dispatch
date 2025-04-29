using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Navigation;

namespace FinishGoodStock
{
    public static class MarwariNavigator
    {
        private static List<MyStack> myStack = new List<MyStack>();
        private static Frame Navigation { get; set; }

        public static void GoBack()
        {
            Navigation = Utility.mainWindow.MainFrame;
            myStack.Remove(myStack.OrderBy(o => o.Serial).LastOrDefault());

            if (myStack.Count() > 0)
            {
                Utility.mainWindow.MainFrame.Navigate(myStack.OrderBy(o => o.Serial).LastOrDefault().control);
            }
            else
            {
                Navigation.Navigate(null);
            
            }
        }

        public static void Navigate(Page userControl)
        {
            Utility.mainWindow.MainFrame.NavigationService.Navigating += NavigationService_Navigating;

            myStack.Add(new MyStack() { Serial = myStack.Count() + 1, control = userControl, Title = userControl.Name });

            Utility.mainWindow.MainFrame.Navigate(userControl);

        }

        private static void NavigationService_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                e.Cancel = true;
            }
        }

        public static void ClearStack()
        {
            myStack.Clear();
        }

    }

    public class MyStack
    {
        public Page control { get; set; }
        public int Serial { get; set; }
        public string Title { get; set; }
    }
}