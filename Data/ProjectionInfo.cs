#region Source file version
// $Archive: $
// $Date: $
// $Revision: $ by 
// $Author: $
// 
// $Workfile: $
// $Modtime: $
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Dream.Data.Demographics
{
  /// <summary>
  /// Container for the full characterization of a demographic projection.
  /// </summary>
  public class ProjectionInfo : SeriesInfo
  {
    
    #region Fields : Forecast dependencies
    private BirthInfo birth;
    private ImmigrationInfo immigration;
    private EmigrationInfo emigration;
    private MortalityInfo mortality;
    private FertilityInfo fertility;
    private NaturalizationInfo naturalization;
    private byte bequestmin = 72;
    private byte bequestmax = 76;
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor for projection characterization.
    /// </summary>
    public ProjectionInfo() : base() {}

    /// <summary>
    /// Copy contructor.
    /// </summary>
    /// <param name="info">The ProjectionInfo object to copy.</param>
    public ProjectionInfo(ProjectionInfo info)
      : base(info)
    {
      this.birth = info.birth.Clone();
      this.immigration = info.immigration.Clone();
      this.emigration = info.emigration.Clone();
      this.mortality = info.mortality.Clone();
      this.fertility = info.fertility.Clone();
      this.naturalization = info.naturalization.Clone();
    }
    #endregion

    #region Forecast properties
    /// <summary>
    /// Characterization of the forecast of births and babies.
    /// </summary>
    [XmlIgnore]
    public BirthInfo Birth
    {
      get { return birth; }
      set 
      { 
        birth = value; 
        saved = false; 
      }
    }
    
    /// <summary>
    /// Characterization of the forecasted fertility.
    /// </summary>
    [XmlIgnore]
    public FertilityInfo Fertility
    {
      get { return fertility; }
      set 
      { 
        fertility = value;
        saved = false;
      }
    }

    /// <summary>
    /// Characterization of the forecasted mortality.
    /// </summary>
    [XmlIgnore]
    public MortalityInfo Mortality
    {
      get { return mortality; }
      set 
      { 
        mortality = value;
        saved = false;
      }
    }

    /// <summary>
    /// Characterization of the forecasted immigration.
    /// </summary>
    [XmlIgnore]
    public ImmigrationInfo Immigration
    {
      get { return immigration; }
      set 
      { 
        immigration = value;
        saved = false;
      }
    }

    /// <summary>
    /// Characterization of the forecasted emigration.
    /// </summary>
    [XmlIgnore]
    public EmigrationInfo Emigration
    {
      get { return emigration; }
      set 
      { 
        emigration = value;
        saved = false;
      }
    }

    /// <summary>
    /// Characterization of the forecasted naturalization.
    /// </summary>
    [XmlIgnore]
    public NaturalizationInfo Naturalization
    {
      get { return naturalization; }
      set 
      {
        naturalization = value;
        saved = false;
      }
    }
    #endregion

    #region Runtime properties
    /// <summary>
    /// The youngest age at which elderly persons leave bequests for their descendants.
    /// </summary>
    public byte BequestMinimumAge
    {
      get { return bequestmin; }
      set
      {
        if (bequestmin != value)
        {
          bequestmin = value;
          saved = false;
        }
      }
    }

    /// <summary>
    /// The oldest age at which elderly persons leave bequests for their descendants.
    /// </summary>
    public byte BequestMaximumAge
    {
      get { return bequestmax; }
      set
      {
        if (bequestmax != value)
        {
          bequestmax = value;
          saved = false;
        }
      }
    }
    #endregion

    #region SeriesInfo.Validate() override
    /// <summary>
    /// Validates the state integrity of current object.
    /// </summary>
    /// <returns>True if the projection is fully characterized, false otherwise.</returns>
    public override bool Validate()
    {
      if (!base.Validate())
      {
        return false;
      }

      foreach (ForecastInfo fi in this.GetForecasts())
      {
        if (fi == null || !fi.Validate())
        {
          return false;
        }
      }

      return true;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Provides simple iteration over the forecast information contained in the projection.
    /// </summary>
    /// <returns>An IEnumerator of type ForecastInfo.</returns>
    public IEnumerable<ForecastInfo> GetForecasts()
    {
      yield return birth;
      yield return fertility;
      yield return mortality;
      yield return emigration;
      yield return immigration;
      yield return naturalization;
    }

    /// <summary>
    /// Create a copy of the current ProjectionInfo object.
    /// </summary>
    /// <returns>A new ProjectionInfo object similar to the current instance except for database key.</returns>
    public ProjectionInfo Copy()
    {
      ProjectionInfo pi = new ProjectionInfo(this);
      return pi;
    }
    #endregion

  }
}