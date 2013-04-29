using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Interop;
using System.Windows.Threading;

namespace RPGMakerFacesGenerator
{
    public class ActiveFaceParts: ObservableCollection<FacePart>
    {
        #region male/female

        private FacePart maleBodyPart = null;

        public bool IsMale
        {
            get
            {
                return maleBodyPart != null;
            }
            set
            {
                if (value != IsMale)
                {
                    if (maleBodyPart == null)
                    {
                        maleBodyPart = new FacePart(null, 3, 0);
                        Add(maleBodyPart);
                    }
                    else
                    {
                        Remove(maleBodyPart);
                        maleBodyPart = null;
                    }
                    propChanged("IsMale");
                    propChanged("IsFemale");
                    initializeTops();
                }
            }
        }

        public bool IsFemale
        {
            get
            {
                return !IsMale;
            }
            set
            {
                IsMale = !value;
            }
        }

        #endregion

        #region generic multiselect parts

        private enum Parts
        {
            Eyes,
            Eyebrows,
            Mouth,
            Features,
            ShortHair,
            MediumHair,
            LongHair,
            Ponytails,
            Tops,
            Eyewear,
            Headwear,
            Neckwear,
            Hands,
            Emotion,
            BackAccessory
        }

        private Dictionary<Parts, FacePartsSet> FullSets = new Dictionary<Parts, FacePartsSet>();

        private void initializeParts(Parts part, bool hasHSVData, int frag1, int frag2, int frag3, int frag4, int fragIndex, int maxFrag, bool allowEmptyFeature = false)
        {
            // create the set
            if (!FullSets.ContainsKey(part))
                FullSets[part] = new FacePartsSet();

            // loads the data
            FullSets[part].Load(hasHSVData, frag1, frag2, frag3, frag4, fragIndex, maxFrag, allowEmptyFeature);

            // binds to the current item changing events, so to synchronize
            var defaultView = CollectionViewSource.GetDefaultView(FullSets[part]);
            defaultView.CurrentChanging += new CurrentChangingEventHandler(defaultView_CurrentChanging);
            defaultView.CurrentChanged += new EventHandler(defaultView_CurrentChanged);
        }

        void defaultView_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            foreach (Parts part in Enum.GetValues(typeof(Parts)))
            {
                var defaultView = CollectionViewSource.GetDefaultView(FullSets[part]);
                if (defaultView == sender)
                {
                    Remove((FacePart)defaultView.CurrentItem);
                    break;
                }
            }
        }

        void defaultView_CurrentChanged(object sender, EventArgs e)
        {
            foreach (Parts part in Enum.GetValues(typeof(Parts)))
            {
                var defaultView = CollectionViewSource.GetDefaultView(FullSets[part]);
                if (defaultView == sender)
                {
                    Add((FacePart)defaultView.CurrentItem);
                    break;
                }
            }
        }

        #endregion

        #region eyes

        public FacePartsSet Eyes { get { return FullSets[Parts.Eyes]; } }

        private void initializeEyes()
        {
            initializeParts(Parts.Eyes, true, 2, 1, -1, -1, 3, 12);
        }

        #endregion

        #region eyebrows

        public FacePartsSet Eyebrows { get { return FullSets[Parts.Eyebrows]; } }

        private void initializeEyebrows()
        {
            initializeParts(Parts.Eyebrows, false, 2, 0, -1, -1, 3, 15);
        }

        #endregion

        #region mouth

        public FacePartsSet Mouth { get { return FullSets[Parts.Mouth]; } }

        private void initializeMouth()
        {
            initializeParts(Parts.Mouth, false, 2, 2, -1, -1, 3, 27);
        }

        #endregion

        #region features

        public FacePartsSet Features { get { return FullSets[Parts.Features]; } }

        private void initializeFeatures()
        {
            initializeParts(Parts.Features, false, 2, 3, -1, -1, 3, 13, true);
        }

        #endregion

        #region short hair

        public FacePartsSet ShortHair { get { return FullSets[Parts.ShortHair]; } }

        private void initializeShortHair()
        {
            initializeParts(Parts.ShortHair, true, 1, 0, -1, -1, 3, 31, true);

            CollectionViewSource.GetDefaultView(ShortHair).CurrentChanged += new EventHandler(ShortHair_CurrentChanged);
        }

        void ShortHair_CurrentChanged(object sender, EventArgs e)
        {
            if (ShortHair.Count == 0)
                return;
            if (CollectionViewSource.GetDefaultView(ShortHair) != ShortHair[0])
            {
                CollectionViewSource.GetDefaultView(MediumHair).MoveCurrentToFirst();
                CollectionViewSource.GetDefaultView(LongHair).MoveCurrentToFirst();
            }
        }

        #endregion

        #region medium hair

        public FacePartsSet MediumHair { get { return FullSets[Parts.MediumHair]; } }

        private void initializeMediumHair()
        {
            initializeParts(Parts.MediumHair, true, 1, 1, -1, -1, 3, 17, true);
            CollectionViewSource.GetDefaultView(MediumHair).CurrentChanged += new EventHandler(MediumHair_CurrentChanged);
        }

        void MediumHair_CurrentChanged(object sender, EventArgs e)
        {
            if (MediumHair.Count == 0)
                return;
            if (CollectionViewSource.GetDefaultView(MediumHair) != MediumHair[0])
            {
                CollectionViewSource.GetDefaultView(ShortHair).MoveCurrentToFirst();
                CollectionViewSource.GetDefaultView(LongHair).MoveCurrentToFirst();
            }
        }

        #endregion

        #region long hair

        public FacePartsSet LongHair { get { return FullSets[Parts.LongHair]; } }

        private void initializeLongHair()
        {
            initializeParts(Parts.LongHair, true, 1, 2, -1, -1, 3, 19, true);
            CollectionViewSource.GetDefaultView(LongHair).CurrentChanged += new EventHandler(LongHair_CurrentChanged);
        }

        void LongHair_CurrentChanged(object sender, EventArgs e)
        {
            if (LongHair.Count == 0)
                return;
            if (CollectionViewSource.GetDefaultView(LongHair) != LongHair[0])
            {
                CollectionViewSource.GetDefaultView(ShortHair).MoveCurrentToFirst();
                CollectionViewSource.GetDefaultView(MediumHair).MoveCurrentToFirst();
            }
        }

        #endregion

        #region ponytails

        public FacePartsSet Ponytails { get { return FullSets[Parts.Ponytails]; } }

        private void initializePonytails()
        {
            initializeParts(Parts.Ponytails, true, 1, 3, -1, -1, 3, 23, true);
        }

        #endregion

        #region tops

        public FacePartsSet Tops { get { return FullSets[Parts.Tops]; } }

        private void initializeTops()
        {
            if (IsMale)
                initializeParts(Parts.Tops, true, 0, 5, 0, -1, 4, 13);
            else
                initializeParts(Parts.Tops, true, 0, 5, 1, -1, 4, 13);
        }

        #endregion

        #region eyewear

        public FacePartsSet Eyewear { get { return FullSets[Parts.Eyewear]; } }

        private void initializeEyewear()
        {
            initializeParts(Parts.Eyewear, true, 0, 4, -1, -1, 3, 11, true);
        }

        #endregion

        #region headwear

        public FacePartsSet Headwear { get { return FullSets[Parts.Headwear]; } }

        private void initializeHeadwear()
        {
            initializeParts(Parts.Headwear, true, 0, 3, -1, -1, 3, 25, true);
        }

        #endregion

        #region neckwear

        public FacePartsSet Neckwear { get { return FullSets[Parts.Neckwear]; } }

        private void initializeNeckwear()
        {
            initializeParts(Parts.Neckwear, true, 0, 2, -1, -1, 3, 20, true);
        }

        #endregion

        #region hands

        public FacePartsSet Hands { get { return FullSets[Parts.Hands]; } }

        private void initializeHands()
        {
            initializeParts(Parts.Hands, false, 0, 1, -1, -1, 3, 12, true);
        }

        #endregion

        #region emotion

        public FacePartsSet Emotion { get { return FullSets[Parts.Emotion]; } }

        private void initializeEmotion()
        {
            initializeParts(Parts.Emotion, false, 0, 0, -1, -1, 3, 11, true);
        }

        #endregion

        #region back accessory

        public FacePartsSet BackAccessory { get { return FullSets[Parts.BackAccessory]; } }

        private void initializeBackAccessory()
        {
            initializeParts(Parts.BackAccessory, true, 4, -1, -1, -1, 2, 7, true);
        }

        #endregion

        #region rendering

        private class RenderCommand : ICommand
        {
            ActiveFaceParts parent;

            public RenderCommand(ActiveFaceParts parent)
            {
                this.parent = parent;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged { add { } remove { } }

            public void Execute(object parameter)
            {
                var suggestedFilename = (string)parameter;
                RenderingWindow rw = new RenderingWindow(suggestedFilename);
                rw.DataContext = parent;
                rw.ShowDialog();
            }
        }

        public ICommand Render { get; private set; }

        #endregion

        #region save and load

        public void SaveTo(StreamWriter outputStream)
        {
            outputStream.Write("Sex:");
            outputStream.WriteLine(IsMale ? "0" : "1");
            foreach (var part in FullSets.Keys)
            {
                var faceParts = FullSets[part];
                var facePart = (FacePart)CollectionViewSource.GetDefaultView(faceParts).CurrentItem;
                if (facePart == null)
                    facePart = FacePartsSet.EmptyFacePart;
                outputStream.Write(part.ToString());
                outputStream.Write(':');
                outputStream.Write(facePart.Frag1);
                outputStream.Write(':');
                outputStream.Write(facePart.Frag2);
                outputStream.Write(':');
                outputStream.Write(facePart.Frag3);
                outputStream.Write(':');
                outputStream.Write(facePart.Frag4);
                var hsv = facePart.HSVData;
                outputStream.Write(':');
                outputStream.Write(hsv == null ? 0 : hsv.HShift);
                outputStream.Write(':');
                outputStream.Write(hsv == null ? 0 : hsv.SFactor);
                outputStream.Write(':');
                outputStream.Write(hsv == null ? 0 : hsv.VFactor);
                outputStream.WriteLine();
            }
        }

        public void LoadFrom(StreamReader inputStream)
        {
            for (; ; )
            {
                var line = inputStream.ReadLine();
                if (line == null)
                    break;
                line = line.Trim();
                var pieces = line.Split(':');

                if (pieces[0] == "Sex")
                    IsMale = pieces[1].Trim() == "0";
                else
                {
                    var frag1 = int.Parse(pieces[1]);
                    var frag2 = int.Parse(pieces[2]);
                    var frag3 = int.Parse(pieces[3]);
                    var frag4 = int.Parse(pieces[4]);

                    var part = (Parts)Enum.Parse(typeof(Parts), pieces[0]);
                    var set = FullSets[part];
                    var defaultView = CollectionViewSource.GetDefaultView(set);
                    int i = 0;
                    foreach (FacePart facepart in defaultView)
                    {
                        if (facepart.Frag1 == frag1 &&
                            facepart.Frag2 == frag2 &&
                            facepart.Frag3 == frag3 &&
                            facepart.Frag4 == frag4)
                            defaultView.MoveCurrentToPosition(i);
                        if (facepart.HSVData != null &&
                            defaultView.CurrentItem != FacePartsSet.EmptyFacePart)
                        {
                            facepart.HSVData.HShift = float.Parse(pieces[5]);
                            facepart.HSVData.SFactor = float.Parse(pieces[6]);
                            facepart.HSVData.VFactor = float.Parse(pieces[7]);
                        }
                        i++;
                    }
                }
            }
        }

        #endregion

        public static FacePart EmptyFacePart = new FacePart(null, 0, 0);

        public ICollectionView SortedFaceParts { get; private set; }

        private void propChanged(string name)
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(name));
        }

        public ActiveFaceParts()
        {
            // add base body parts
            Add(new FacePart(null, 3, 1));
            initialize();
            Reset();
            SortedFaceParts = CollectionViewSource.GetDefaultView(this);
            SortedFaceParts.SortDescriptions.Add(new SortDescription("Frag1", ListSortDirection.Descending));
            SortedFaceParts.SortDescriptions.Add(new SortDescription("Frag2", ListSortDirection.Descending));
            SortedFaceParts.SortDescriptions.Add(new SortDescription("Frag3", ListSortDirection.Descending));
            SortedFaceParts.SortDescriptions.Add(new SortDescription("Frag4", ListSortDirection.Descending));
            Render = new RenderCommand(this);
        }

        public void Reset()
        {
            IsMale = false;
            foreach (var part in FullSets.Keys)
            {
                CollectionViewSource.GetDefaultView(FullSets[part]).MoveCurrentToFirst();
                var hsvData = FullSets[part][1].HSVData; // 0 may be the empty face part
                if (hsvData != null)
                {
                    hsvData.HShift = 0;
                    hsvData.SFactor = 1;
                    hsvData.VFactor = 1;
                }
            }
        }

        private void initialize()
        {
            initializeEyes();
            initializeEyebrows();
            initializeMouth();
            initializeFeatures();
            initializeShortHair();
            initializeMediumHair();
            initializeLongHair();
            initializePonytails();
            initializeTops();
            initializeEyewear();
            initializeHeadwear();
            initializeNeckwear();
            initializeHands();
            initializeEmotion();
            initializeBackAccessory();
        }
    }
}
