using Caliburn.Micro;
using System.Windows.Controls;

namespace Projekt1.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {


        public ShellViewModel()
        { }

        public void ActivateItem(MenuItem menuItem)
        {
            if (menuItem.Header.ToString() == "Projekt 1")
                this.ActivateItem(IoC.Get<ProjectOneViewModel>());

            if (menuItem.Header.ToString() == "Projekt 2")
                this.ActivateItem(IoC.Get<ProjectTwoViewModel>());

            if (menuItem.Header.ToString() == "Projekt 3")
                this.ActivateItem(IoC.Get<ProjectThreeViewModel>());
        }
    }
}
