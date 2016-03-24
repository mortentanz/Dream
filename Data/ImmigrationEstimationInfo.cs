using System;
using System.Collections.Generic;
using System.Text;

namespace Dream.Data.Demographics
{
  /// <summary>
  /// Container for characterization and definition of a migration estimation.
  /// </summary>
  public class ImmigrationEstimationInfo : EstimationInfo
  {

    #region Fields
    private OriginFlags flags = OriginFlags.Default;
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="direction">Requires a </param>
    public ImmigrationEstimationInfo()
      : base(EstimationType.Immigration)
    {
      flags = OriginFlags.Default;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="info">An ImmigrationEstimationInfo object to be copied.</param>
    public ImmigrationEstimationInfo(ImmigrationEstimationInfo info)
      : base(info)
    {
      flags = info.flags;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The origins for which immigration is estimated.
    /// </summary>
    public OriginFlags Origins
    {
      get { return flags; }
      set { throw new NotImplementedException("Support for other than Non Citizen Immigrants is not implemented yet."); }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Creates a copy of the current instance.
    /// </summary>
    /// <returns>A new ImmigrationEstimationInfo object with the same descriptive properties as the current instance.</returns>
    public ImmigrationEstimationInfo Copy()
    {
      return new ImmigrationEstimationInfo(this);
    }

    /// <summary>
    /// Creates an identical copy of the current instance.
    /// </summary>
    /// <returns>A new ImmigrationEstimationInfo object identical to the current instance.</returns>
    internal ImmigrationEstimationInfo Clone()
    {
      ImmigrationEstimationInfo info = new ImmigrationEstimationInfo(this);
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
