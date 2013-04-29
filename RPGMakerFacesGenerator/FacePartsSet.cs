using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace RPGMakerFacesGenerator
{
    /// <summary>
    /// A set of related face parts (e.g.: all the eyes models).
    /// </summary>
    public class FacePartsSet : ObservableCollection<FacePart>
    {
        /// <summary>
        /// HSV correction data.
        /// </summary>
        public HSVData HSVData { get; private set; }

        /// <summary>
        /// Whether HSV correction is supported by this set.
        /// </summary>
        public bool HasHSVData { get { return HSVData != null; } }

        /// <summary>
        /// The face part used to represent the empty part (useful for optional parts)
        /// </summary>
        public static FacePart EmptyFacePart = new FacePart(null, 0, 0);

        private void propChanged(string propertyName)
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Loads the contents of this face parts set
        /// </summary>
        /// <param name="hasHSVData">Whether this set supports coloring through HSV controls.</param>
        /// <param name="frag1">First fragment of the face parts of this set.</param>
        /// <param name="frag2">Second fragment of the face parts of this set.</param>
        /// <param name="frag3">Third fragment of the face parts of this set.</param>
        /// <param name="frag4">Fourth fragment of the face parts of this set.</param>
        /// <param name="fragIndex">Index of the fragment changed during the loop loading</param>
        /// <param name="maxFrag">Maximum value for the fragment changed during the loop loading</param>
        /// <param name="allowEmptyFeature">Whether the empty feature is in the set</param>
        public void Load(bool hasHSVData, int frag1, int frag2, int frag3, int frag4, int fragIndex, int maxFrag, bool allowEmptyFeature = false)
        {
            Clear();

            if (hasHSVData && HSVData == null)
            {
                HSVData = new HSVData();
                propChanged("HSVData");
                propChanged("HasHSVData");
            }
            if (!hasHSVData && HSVData != null)
            {
                HSVData = null;
                propChanged("HSVData");
                propChanged("HasHSVData");
            }

            if (allowEmptyFeature)
                Add(EmptyFacePart);
            for (int frag = 0; frag < maxFrag; frag++)
            {
                if (fragIndex == 1)
                    frag1 = frag;
                else if (fragIndex == 2)
                    frag2 = frag;
                else if (fragIndex == 3)
                    frag3 = frag;
                else
                    frag4 = frag;
                Add(new FacePart(HSVData, frag1, frag2, frag3, frag4));
            }
        }
    }
}
