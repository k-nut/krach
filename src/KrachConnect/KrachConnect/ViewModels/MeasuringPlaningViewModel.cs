using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;

namespace KrachConnect.ViewModels
{
  internal class MeasuringPlaningViewModel : PropertyChangedBase
  {
    private readonly NoiseRepository repository;
    private ObservableCollection<MeasuringPointViewModel> measuringPointViewModels;
    private MeasuringPointViewModel selectedMeasuringPoint;
    private string _searchTerm = "";
    private ShellViewModel shellViewModel;

    public MeasuringPlaningViewModel(NoiseRepository repository, ShellViewModel shellviewModel)
    {
      this.repository = repository;
      this.shellViewModel = shellviewModel;
      MeasuringPoints = new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringPointViewModels.Where(mp => !mp.IsArchived));
      foreach (var mp in MeasuringPoints)
      {
        mp.PropertyChanged += IsSelectedChanged;
      }
    }

    private void IsSelectedChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsSelected")
      {
        NotifyOfPropertyChange(() => IsMeasuringPointEnabled);
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

    //TODO: Check if there is a cleaner solution for this (esp. on the xaml side)
    public void Filter(object text)
    {
      SearchTerm = text.ToString();
    }

    public ObservableCollection<MeasuringPointViewModel> FilteredMeasuringPoints
    {
      get { return SearchTerm != "" ? new ObservableCollection<MeasuringPointViewModel>(measuringPointViewModels
        .Where(mp => mp.Name.ToLower().Contains(SearchTerm.ToLower()))) : measuringPointViewModels; }
      set
      {
        measuringPointViewModels = value;
        NotifyOfPropertyChange(() => MeasuringPoints);
      }
    }

    public void ToggleSelection(object dataContext)
    {
      var measuringPoint = (MeasuringPointViewModel) dataContext;
      measuringPoint.IsSelected = !measuringPoint.IsSelected;
    }

    public void SelectAll()
    {
      foreach (var measuringPointViewModel in MeasuringPoints)
      {
        measuringPointViewModel.IsSelected = true;
      }
    }

    public void DeSelectAll()
    {
      foreach (var measuringPointViewModel in MeasuringPoints)
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
      repository.MeasuringWalk = selectedMeasuringPoints;
      shellViewModel.ShowMapScreen();
      }

      public bool IsMeasuringPointEnabled
      {
          get { return MeasuringPoints.Any(mp => mp.IsSelected); }
      }
  }
}