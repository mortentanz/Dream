using System;
using System.Collections.Generic;
using System.Text;

namespace Dream.Data.Demographics
{
  /// <summary>
  /// Container for information about a fertility estimation used as input for fertility forecasts.
  /// </summary>
  public class FertilityEstimationInfo : EstimationInfo
  {

    #region Constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public FertilityEstimationInfo()
      : base(EstimationType.Fertility)
    {
      sampleseries = new Series();
      sampleseries.Define(1980, series.StartYear - 1);
      saved = false;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="info">The existing FertilityEstimationInfo to be copied.</param>
    public FertilityEstimationInfo(FertilityEstimationInfo info) : base(info) { }
    #endregion

    #region Methods
    /// <summary>
    /// Creates a copy of the current instance.
    /// </summary>
    /// <returns>A new FertilityEstimationInfo object with the same descriptive properties as the current instance.</returns>
    public FertilityEstimationInfo Copy()
    {
      return new FertilityEstimationInfo(this);
    }

    /// <summary>
    /// Creates an identical copy of the current instance.
    /// </summary>
    /// <returns>A new FertilityEstimationInfo object identical to the current instance.</returns>
    internal FertilityEstimationInfo Clone()
    {
      FertilityEstimationInfo info = new FertilityEstimationInfo(this);
      if (this.CatalogEntry != null)
      {
        info.CatalogEntry = this.CatalogEntry.Clone();
      }
      info.saved = this.saved;
      return info;
    }
    #endregion
  }
}
