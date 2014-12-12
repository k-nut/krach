using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using KrachConnect.ViewModels;

namespace KrachConnect
{
  public class AppBootstrapper : BootstrapperBase
  {
    public AppBootstrapper()
    {
      Initialize();
    }
    protected override void OnStartup(object sender, StartupEventArgs e)
    {
      DisplayRootViewFor<ShellViewModel>();
    }
  }
}