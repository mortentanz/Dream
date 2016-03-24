using System;
using System.Collections.Generic;
using System.Text;

namespace Dream.Data.Demographics
{
  /// <summary>
  /// Container for information about a mortality estimation used as input for mortality forecasts.
  /// </summary>
  public class MortalityEstimationInfo : EstimationInfo
  {
    #region Constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public MortalityEstimationInfo()
      : base(EstimationType.Mortality)
    {
      sampleseries = new Series();
      sampleseries.Define(1990, series.StartYear - 1);
      saved = false;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="info">The existing MortalityEstimationInfo to be copied.</param>
    public MortalityEstimationInfo(MortalityEstimationInfo info) : base(info) { }
    #endregion

    #region Methods
    /// <summary>
    /// Creates a copy of the current instance.
    /// </summary>
    /// <returns>A new MortalityEstimationInfo object with the same descriptive properties as the current instance.</returns>
    public MortalityEstimationInfo Copy()
    {
      return new MortalityEstimationInfo(this);
    }

    /// <summary>
    /// Creates an identical copy of the current instance.
    /// </summary>
    /// <returns>A new MortalityEstimationInfo object identical to the current instance.</returns>
    internal MortalityEstimationInfo Clone()
    {
      MortalityEstimationInfo mei = new MortalityEstimationInfo(this);
      if (this.CatalogEntry != null)
      {
        mei.CatalogEntry = this.CatalogEntry.Clone();
      }
      mei.saved = this.saved;
      return mei;
    }
    #endregion

  }
}
