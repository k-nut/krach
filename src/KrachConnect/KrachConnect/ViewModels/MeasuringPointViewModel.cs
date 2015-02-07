using Caliburn.Micro;
using KrachConnect.DomainModelService;

namespace KrachConnect
{
    public class MeasuringPointViewModel : PropertyChangedBase
    {
        private readonly MeasuringPoint m_Model;
        private bool isArchived = false;
        private bool isSelected;
        private bool justMeasured;
        private bool deleted;

        public MeasuringPointViewModel(MeasuringPoint mp)
        {
            m_Model = mp;
        }


        public MeasuringPoint Model
        {
            get { return m_Model; }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                NotifyOfPropertyChange(() => Model);
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public int XPosition
        {
            get { return m_Model.Position.XPosition; }
            set
            {
                m_Model.Position.XPosition = value;
                NotifyOfPropertyChange(() => XPosition);
            }
        }

        public int YPosition
        {
            get { return m_Model.Position.YPosition; }
            set
            {
                m_Model.Position.YPosition = value;
                NotifyOfPropertyChange(() => YPosition);
            }
        }

        public NoiseMapPosition Position
        {
            get { return m_Model.Position; }
            set
            {
                m_Model.Position = value;
                NotifyOfPropertyChange(() => Position);
            }
        }

        public bool IsArchived
        {
            get { return m_Model.IsArchived; }
            set
            {
                m_Model.IsArchived = value;
                NotifyOfPropertyChange(() => IsArchived);
            }
        }

        public string Notes
        {
            get { return m_Model.Notes; }
            set
            {
                m_Model.Notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }

        public string Name
        {
            get { return m_Model.Name; }
            set
            {
                m_Model.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public NoiseMap NoiseMap
        {
            get { return m_Model.Position.NoiseMap; }
            set
            {
                m_Model.Position.NoiseMap = value;
                NotifyOfPropertyChange(() => NoiseMap);
            }
        }


        public bool JustMeasured
        {
            get { return justMeasured; }
            set
            {
                justMeasured = value;
                NotifyOfPropertyChange(() => JustMeasured);
            }
        }

        public bool Deleted
        {
            get { return deleted; }
            set
            {
                deleted = value;
                NotifyOfPropertyChange(() => Deleted);
            }
        }
    }
}