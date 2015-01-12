using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using KrachConnect.ViewModels;

namespace KrachConnect
{
  public class AppBootstrapper : BootstrapperBase
  {
    public AppBootstrapper()
    {
      Initialize();
      MessageBinder.SpecialValues.Add("$scaledmousex", ctx =>
      {
        var img = ctx.Source as Image;
        var input = ctx.Source as IInputElement;
        var e = ctx.EventArgs as MouseEventArgs;
        return (int) e.GetPosition(input).X;

        // If there is an image control, get the scaled position
        if (img != null && e != null)
        {
          Point position = e.GetPosition(img);
          return (int) (img.Source.Width*(position.X/img.ActualWidth));
        }

        // If there is another type of of IInputControl get the non-scaled position - or do some processing to get a scaled position, whatever needs to happen
        if (e != null && input != null)
          return e.GetPosition(input).X;

        // Return 0 if no processing could be done
        return 0;
      });
      MessageBinder.SpecialValues.Add("$scaledmousey", ctx =>
      {
        var img = ctx.Source as Image;
        var input = ctx.Source as IInputElement;
        var e = ctx.EventArgs as MouseEventArgs;
        return (int) e.GetPosition(input).Y;


        // If there is an image control, get the scaled position
        if (img != null && e != null)
        {
          Point position = e.GetPosition(img);
          return (int) (img.Source.Height*(position.Y/img.ActualHeight));
        }

        // If there is another type of of IInputControl get the non-scaled position - or do some processing to get a scaled position, whatever needs to happen
        if (e != null && input != null)
          return e.GetPosition(input).Y;

        // Return 0 if no processing could be done
        return 0;
      });

      MessageBinder.SpecialValues["$text"] = context =>
      {
        var textBox = (TextBox)context.Source;
        return textBox.Text;
      };
    }



    protected override void OnStartup(object sender, StartupEventArgs e)
    {
      DisplayRootViewFor<ShellViewModel>();
    }
  }
}