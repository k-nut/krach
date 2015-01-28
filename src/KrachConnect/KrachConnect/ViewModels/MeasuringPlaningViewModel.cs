using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;

namespace KrachConnect.ViewModels
{
  internal class MeasuringPlaningViewModel : PropertyChangedBase
  {
    private readonly ShellViewModel shellViewModel;
    private string _searchTerm = "";
    private ObservableCollection<MeasuringPointViewModel> measuringPointViewModels;
    private MeasuringPointViewModel selectedMeasuringPoint;

    public MeasuringPlaningViewModel(NoiseRepository repository, ShellViewModel shellViewModel)
    {
      this.shellViewModel = shellViewModel;
      MeasuringPoints =
        new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringPointViewModels.Where(mp => !mp.IsArchived));
      foreach (MeasuringPointViewModel mp in MeasuringPoints)
      {
        mp.PropertyChanged += IsSelectedChanged;
      }
    }

    public ObservableCollection<MeasuringPointViewModel> MeasuringPoints
    {
      get { return measuringPointViewModels; }
      set
      {
        measuringPointViewModels = value;
        NotifyOfPropertyChange(() => MeasuringPoints);
      }
    }

    public string SearchTerm
    {
      get { return _searchTerm; }
      set
      {
        _searchTerm = value;
        NotifyOfPropertyChange(() => SearchTerm);
        NotifyOfPropertyChange(() => FilteredMeasuringPoints);
      }
    }

    public IEnumerable<MeasuringPointViewModel> FilteredMeasuringPoints
    {
      get
      {
        return SearchTerm != ""
          ? measuringPointViewModels
            .Where(mp => mp.Name.ToLower().Contains(SearchTerm.ToLower()))
          : measuringPointViewModels;
      }
    }

    public bool IsStartButtonEnabled
    {
      get { return MeasuringPoints.Any(mp => mp.IsSelected); }
    }

    private void IsSelectedChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsSelected")
      {
        NotifyOfPropertyChange(() => IsStartButtonEnabled);
      }
    }

    public void Filter(object text)
    {
      SearchTerm = text.ToString();
    }

    public void ToggleSelection(object dataContext)
    {
      var measuringPoint = (MeasuringPointViewModel) dataContext;
      measuringPoint.IsSelected = !measuringPoint.IsSelected;
    }

    public void SelectAll()
    {
      foreach (MeasuringPointViewModel measuringPointViewModel in FilteredMeasuringPoints)
      {
        measuringPointViewModel.IsSelected = true;
      }
    }

    public void DeSelectAll()
    {
      foreach (MeasuringPointViewModel measuringPointViewModel in FilteredMeasuringPoints)
      {
        measuringPointViewModel.IsSelected = false;
      }
    }

    public void Save()
    {
      List<MeasuringPointViewModel> selectedMeasuringPoints = MeasuringPoints.Where(mp => mp.IsSelected).ToList();
      foreach (MeasuringPointViewModel measuringPointViewModel in selectedMeasuringPoints)
      {
        measuringPointViewModel.IsSelected = false;
      }
      shellViewModel.ShowMapScreen(new ObservableCollection<MeasuringPointViewModel>(selectedMeasuringPoints));
    }
  }
}