using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Data;
using System.ComponentModel;

namespace RPGMakerFacesGenerator
{
    public class SavedColors: ObservableCollection<HSVData>
    {
        public SavedColors()
        {
            Save = new SaveCommand(this);
            Load = new LoadCommand(this);
            load();
        }

        private void load()
        {
            var savedColors = Properties.Settings.Default.SavedColors;
            if (string.IsNullOrEmpty(savedColors))
                for (int i = 0; i < 9; i++)
                    Add(new HSVData() { HShift = 0, SFactor = 1, VFactor = 1 });
            else
            {
                var parts = savedColors.Split(':');
                for (int i = 0; i+2 < parts.Length; i += 3)
                    Add(new HSVData()
                    {
                        HShift = float.Parse(parts[i]),
                        SFactor = float.Parse(parts[i + 1]),
                        VFactor = float.Parse(parts[i + 2])
                    });
            }
            foreach(var hsvData in this)
                hsvData.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(hsvData_PropertyChanged);
        }

        void hsvData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            save();
        }

        private void save()
        {
            var sb = new StringBuilder();
            foreach (var hsvData in this)
            {
                sb.Append(hsvData.HShift.ToString());
                sb.Append(':');
                sb.Append(hsvData.SFactor.ToString());
                sb.Append(':');
                sb.Append(hsvData.VFactor.ToString());
                sb.Append(':');
            }
            Properties.Settings.Default.SavedColors = sb.ToString();
            Properties.Settings.Default.Save();
        }

        private abstract class BaseCommand : ICommand
        {
            private SavedColors savedColors;
            protected ICollectionView DefaultView;

            public BaseCommand(SavedColors savedColors)
            {
                this.savedColors = savedColors;
                DefaultView = CollectionViewSource.GetDefaultView(savedColors);
                DefaultView.CurrentChanged += new EventHandler(defaultView_CurrentChanged);
            }

            void defaultView_CurrentChanged(object sender, EventArgs e)
            {
                var handler = CanExecuteChanged;
                if (handler != null)
                    handler(this, new EventArgs());
            }

            public bool CanExecute(object parameter)
            {
                return DefaultView.CurrentItem != null;
            }

            public event EventHandler CanExecuteChanged;

            public abstract void Execute(object parameter);
        }

        private class SaveCommand : BaseCommand
        {
            public SaveCommand(SavedColors savedColors)
                : base(savedColors)
            {
            }

            public override void Execute(object parameter)
            {
                var hsvData = (HSVData)parameter;
                var selectedEntry = (HSVData)DefaultView.CurrentItem;
                selectedEntry.HShift = hsvData.HShift;
                selectedEntry.SFactor = hsvData.SFactor;
                selectedEntry.VFactor = hsvData.VFactor;
            }
        }

        public ICommand Save { get; private set; }

        private class LoadCommand : BaseCommand
        {
            public LoadCommand(SavedColors savedColors)
                : base(savedColors)
            {
            }

            public override void Execute(object parameter)
            {
                var hsvData = (HSVData)parameter;
                var selectedEntry = (HSVData)DefaultView.CurrentItem;
                hsvData.HShift = selectedEntry.HShift;
                hsvData.SFactor = selectedEntry.SFactor;
                hsvData.VFactor = selectedEntry.VFactor;
            }
        }

        public ICommand Load { get; private set; }
    }
}
