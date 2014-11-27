using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Caliburn.Metro.Demo.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Screen, IShell
    {
        public void Close()
        {
            this.TryClose();
        }

    }
}