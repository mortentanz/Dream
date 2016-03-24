using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Dream.Data
{
  /// <summary>
  /// Provides the abstract base class for ProjectionInfo, ForecastInfo and EstimationInfo.
  /// </summary>
  public abstract class SeriesInfo
  {

    #region Fields
    protected DBEntry dbentry = null;
    protected Series series;
    protected bool saved = false;
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor used for deserialization.
    /// </summary>
    protected SeriesInfo() 
    {
      series = new Series();
      saved = false;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="info">The SeriesInfo to be copied.</param>
    protected SeriesInfo(SeriesInfo info)
    {
      this.dbentry = info.CatalogEntry.Copy();
      this.series = info.series.Copy();
      this.saved = false;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The DBEntry object containing database catalog information for the series.
    /// </summary>
    [XmlIgnore]
    public DBEntry CatalogEntry
    {
      get { return dbentry; }
      internal set { dbentry = value; }
    }

    /// <summary>
    /// The range of years covered represented as a Series object.
    /// </summary>
    public Series Series
    {
      get { return series; }
      set
      {
        series = value;
        saved = false;
      }
    }

    /// <summary>
    /// States whether the current state of the series information been written to the database.
    /// </summary>
    [XmlIgnore]
    public virtual bool Saved
    {
      get { return dbentry != null && dbentry.Saved && series.Saved && saved; }
      internal set { saved = value; }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Validates the state integrity of the current SeriesInfo object.
    /// </summary>
    /// <returns>True if the series info is ready for being cataloged, false otherwise.</returns>
    public virtual bool Validate()
    {
      return (dbentry != null && dbentry.Validate() && series.StartYear <= DateTime.Now.Year && series.Length <= 500);
    }
    #endregion

  }
}
