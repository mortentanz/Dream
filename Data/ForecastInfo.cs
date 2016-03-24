using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Dream.Data.Demographics
{
  /// <summary>
  /// Represents information and runtime parameters for projection forecast components.
  /// </summary>
  public abstract class ForecastInfo : SeriesInfo
  {
    #region Fields
    protected ForecastType type;
    protected ForecastSpecification specification;
    protected FlowDetermination determination;
    protected short referenceid = -1;
    #endregion 

    #region Constructors
    /// <summary>
    /// Default constructor used for deserialization.
    /// </summary>
    protected ForecastInfo() : base() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="type">The type of content stored in the forecast.</param>
    protected ForecastInfo(ForecastType type)
      : base()
    {
      this.type = type;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="info">The ForecastInfo to copy.</param>
    protected ForecastInfo(ForecastInfo info)
      : base(info)
    {
      this.type = info.type;
      this.specification = info.specification;
      this.determination = info.determination;
      this.referenceid = info.referenceid;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The forecast class identification used in the database.
    /// </summary>
    [XmlIgnore]
    public string Class
    {
      get { return type.ToString(); }
    }

    /// <summary>
    /// The type of specification of the forecast.
    /// </summary>
    public abstract ForecastSpecification Specification { get; set; }

    /// <summary>
    /// The type of projection flow determination implied by the forecast.
    /// </summary>
    public abstract FlowDetermination Determination { get; set; }

    /// <summary>
    /// The database id of the reference projection if the forecast is a counterfactual.
    /// </summary>
    public virtual short ReferenceID
    {
      get { return referenceid; }
      internal set 
      { 
        referenceid = value;
        saved = false;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Validates the state integrity of the current ForecastInfo instance.
    /// </summary>
    /// <returns>True if the forecast is characterized and ready for being cataloged, false otherwise.</returns>
    public override bool Validate()
    {
      if(!base.Validate())
      {
        return false;
      }

      if (specification == ForecastSpecification.Constant || specification == ForecastSpecification.Scaling)
      {
        return referenceid != -1;
      }
      
      return true;
    }

    /// <summary>
    /// Resets the ForecastInfo to default settings for the current Specification.
    /// </summary>
    public abstract void Reset();
    #endregion

  }
}
