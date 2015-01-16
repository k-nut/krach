﻿using System;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect.ViewModels
{
    internal class MapAddViewModel : PropertyChangedBase
    {
        private readonly NoiseRepository repository;
        private Visibility detailVisibility = Visibility.Visible;

        private ObservableCollection<NoiseMeasurementViewModel> measurementsAddedInThisReading =
          new ObservableCollection<NoiseMeasurementViewModel>();

        private ObservableCollection<MeasuringPointViewModel> measuringPoints;
        private NoiseMeasurementViewModel newNoiseMeasurement;
        private MeasuringPointViewModel selectedMeasuringPoint;

        private string showHideContent = "verstecke Details";
        
        private ShellViewModel shellViewModel;


        public MapAddViewModel(NoiseRepository repository, ShellViewModel shellViewModel)
        {
            // TODO: Complete member initialization
            this.repository = repository;
            this.shellViewModel = shellViewModel;
           
            //if (repository.MeasuringWalk != null)
            //{
            MeasuringPoints = new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringWalk);
            //}
            //else
            //{
                //MeasuringPoints = new ObservableCollection<MeasuringPointViewModel>(repository.MeasuringPointViewModels);
            //}
            NewNoiseMeasurement = new NoiseMeasurementViewModel(new NoiseMeasurement
            {
                MeasurementDate = DateTime.Now
            });
            // TODO: Abfangen, wenn es keinerlei Messpunkte gibt
            SelectNextMeasuringPoint();
        }

        public string ShowHideContent
        {
            get { return showHideContent; }
            set
            {
                showHideContent = value;
                NotifyOfPropertyChange(() => ShowHideContent);
            }
        }

        public Visibility DetailVisibility
        {
            get { return detailVisibility; }
            set
            {
                detailVisibility = value;
                NotifyOfPropertyChange(() => DetailVisibility);
            }
        }

        public int TotalMeasuringPoints
        {
            get { return MeasuringPoints.Count(mp => !mp.IsArchived); }
        }

        public int CurrentNumber
        {
            get { return MeasuringPoints.Count(mp => mp.JustMeasured) + 1; }
        }

        public MeasuringPointViewModel SelectedMeasuringPoint
        {
            get { return selectedMeasuringPoint; }
            set
            {
                selectedMeasuringPoint = value;
                selectedMeasuringPoint.IsSelected = true;
                NotifyOfPropertyChange(() => SelectedMeasuringPoint);
            }
        }

        public NoiseMeasurementViewModel NewNoiseMeasurement
        {
            get { return newNoiseMeasurement; }
            set
            {
                newNoiseMeasurement = value;
                NotifyOfPropertyChange(() => NewNoiseMeasurement);
            }
        }

        public bool CanClick
        {
            get { return MeasuringPoints.Any(mp => !mp.JustMeasured && !mp.IsArchived); }
        }

        public ObservableCollection<NoiseMeasurementViewModel> MeasurementsAddedInThisReading
        {
            get { return measurementsAddedInThisReading; }
            set
            {
                measurementsAddedInThisReading = value;
                NotifyOfPropertyChange(() => MeasurementsAddedInThisReading);
            }
        }

        public Visibility AllDone
        {
            get { return MeasuringPoints.Any(mp => !mp.JustMeasured) ? Visibility.Hidden : Visibility.Visible; }
        }

        public ObservableCollection<MeasuringPointViewModel> MeasuringPoints
        {
            get { return measuringPoints; }
            set
            {
                measuringPoints = value;
                NotifyOfPropertyChange(() => MeasuringPoints);
            }
        }

        public void AddNoiseMeasurementToSelectedMeasuringPoint()
        {
            NewNoiseMeasurement.MeasuringPoint = SelectedMeasuringPoint.Model;
            if (!repository.NoiseMeasurements.Contains(NewNoiseMeasurement.Model))
            {
                repository.NoiseMeasurements.Add(NewNoiseMeasurement.Model);
            }


            MeasurementsAddedInThisReading.Add(newNoiseMeasurement);
            NotifyOfPropertyChange(() => MeasurementsAddedInThisReading);

            NoiseMeasurementViewModel oldNewNoiseMearument = newNoiseMeasurement;
            NewNoiseMeasurement = createNewNoiseMeasurementWithStaticValuesFromOldOne(oldNewNoiseMearument);


            SelectedMeasuringPoint.IsSelected = false;
            SelectedMeasuringPoint.JustMeasured = true;
            NotifyOfPropertyChange(() => CanClick);
            NotifyOfPropertyChange(() => AllDone);




            if (MeasuringPoints.Any(mp => !mp.JustMeasured))
            {
                SelectNextMeasuringPoint();
            }
        }

        public void SelectNextMeasuringPoint()
        {
            SelectedMeasuringPoint = MeasuringPoints.First(mp => !mp.JustMeasured && !mp.IsArchived);

            NotifyOfPropertyChange(() => MeasurementsAddedInThisReading);
            NotifyOfPropertyChange(() => MeasuringPoints);
            NotifyOfPropertyChange(() => CurrentNumber);
            NotifyOfPropertyChange(() => IsBackButtonEnabled);
        }

        private NoiseMeasurementViewModel createNewNoiseMeasurementWithStaticValuesFromOldOne(
          NoiseMeasurementViewModel oldNoiseMeasurement)
        {
            return new NoiseMeasurementViewModel(new NoiseMeasurement
            {
                MeasurementDate = oldNoiseMeasurement.MeasurementDate,
                Employee = oldNoiseMeasurement.Employee,
                MinValue = oldNoiseMeasurement.MinValue,
                AverageValue = oldNoiseMeasurement.AverageValue,
                MaxValue = oldNoiseMeasurement.MaxValue
            });
        }


        public void ChangeSelectedMeasuringPoint(object dataContext)
        {
            var measuringPointViewModel = (MeasuringPointViewModel)dataContext;
            MeasuringPoint measuringPoint = measuringPointViewModel.Model;
            if (measuringPointViewModel.IsArchived)
            {
                return;
            }
            try
            {
                NoiseMeasurementViewModel correspondingNoiseMeasurement =
                  measurementsAddedInThisReading.First(maitr => maitr.MeasuringPoint == measuringPoint);
                NewNoiseMeasurement = correspondingNoiseMeasurement;
            }
            catch (InvalidOperationException e)
            {
                NewNoiseMeasurement = createNewNoiseMeasurementWithStaticValuesFromOldOne(NewNoiseMeasurement);
            }
            SetSelectedMeasuringPoint(measuringPointViewModel);
        }

        private void SetSelectedMeasuringPoint(MeasuringPointViewModel measuringPointViewModel)
        {
            SelectedMeasuringPoint.IsSelected = false;
            SelectedMeasuringPoint = measuringPointViewModel;
            NotifyOfPropertyChange(() => IsBackButtonEnabled);
        }

        public void SaveToHub()
        {
            repository.Save();
        }

        public void ToggleDetails()
        {
            if (DetailVisibility == Visibility.Visible)
            {
                DetailVisibility = Visibility.Hidden;
                ShowHideContent = "zeige Details";
            }
            else
            {
                DetailVisibility = Visibility.Visible;
                ShowHideContent = "verstecke Details";
            }
        }

        public void SelectPreviousMeasuringPoint()
        {
            var index = MeasuringPoints.IndexOf(SelectedMeasuringPoint);

            SetSelectedMeasuringPoint(MeasuringPoints[index - 1]);
        }

        public bool IsBackButtonEnabled
        {
            get
            {
                var index = MeasuringPoints.IndexOf(SelectedMeasuringPoint);
                return index > 0;
            }
        }

        public void CancelMeasuring ()
        {
           this.shellViewModel.ShowHomePageScreen();
        }
    }
}