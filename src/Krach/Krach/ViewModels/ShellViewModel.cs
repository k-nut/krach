using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Krach.ViewModels
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