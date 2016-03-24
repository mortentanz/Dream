using System;
using System.Collections.Generic;
using System.Text;

namespace Dream.Data.Demographics
{
  public class EmigrationEstimationInfo : EstimationInfo
  {
    #region Fields
    private OriginFlags flags = OriginFlags.None;
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="direction">Requires a </param>
    public EmigrationEstimationInfo()
      : base(EstimationType.Emigration)
    {
      flags = OriginFlags.None;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="info">An EmigrationEstimationInfo object to be copied.</param>
    public EmigrationEstimationInfo(EmigrationEstimationInfo info)
      : base(info)
    {
      flags = info.flags;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The origins for which emigration is estimated.
    /// </summary>
    public OriginFlags Origins
    {
      get { return flags; }
      set { throw new NotImplementedException("Support for orogin specific treatment of emigration is not implemented."); }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Creates a copy of the current instance.
    /// </summary>
    /// <returns>A new EmigrationEstimationInfo object with the same descriptive properties as the current instance.</returns>
    public EmigrationEstimationInfo Copy()
    {
      return new EmigrationEstimationInfo(this);
    }

    /// <summary>
    /// Creates an identical copy of the current instance.
    /// </summary>
    /// <returns>A new EmigrationEstimationInfo object identical to the current instance.</returns>
    internal EmigrationEstimationInfo Clone()
    {
      EmigrationEstimationInfo info = new EmigrationEstimationInfo(this);
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
