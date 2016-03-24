using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Dream.Data.Demographics
{
  /// <summary>
  /// Provides information on how fertility is forecasted.
  /// </summary>
  public class FertilityInfo : ForecastInfo
  {

    #region Fields
    private FertilityEstimationInfo estimation = null;
    private ScaleOption scalemethod = ScaleOption.None;
    private double scalefactor = 1.0;
    private int constantyear = -1;
    #endregion

    #region Supported scale methods
    /// <summary>
    /// Supported methods for scaling fertility rates.
    /// </summary>
    public enum ScaleOption
    {
      /// <summary>
      /// The fertility forecast does not use scaling in its specification (default).
      /// </summary>
      None,
      /// <summary>
      /// Scale by simple application of the scalefactor (default if specification is set to Scaling).
      /// </summary>
      UseScaleFactor,
      /// <summary>
      /// Scale to target total fertility rate.
      /// </summary>
      UseTotalFertilityRate
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor for a reference fertility forecast.
    /// </summary>
    public FertilityInfo() : this(ForecastSpecification.Reference, FlowDetermination.Endogenous) { }

    /// <summary>
    /// Constructor for a specific fertility forecast specification.
    /// </summary>
    /// <param name="specification">A ForecastSpecification value characterizing the forecast.</param>
    /// <param name="determination">A FlowDetermination value (will be Endogenous for Reference).</param>
    public FertilityInfo(ForecastSpecification specification, FlowDetermination determination)
      : base(ForecastType.Fertility)
    {
      this.specification = specification;
      this.determination = determination;
      Reset();
      this.saved = false;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="info">The FertilityInfo instance to copy.</param>
    public FertilityInfo(FertilityInfo info)
      : base(info)
    {
      this.scalemethod = info.scalemethod;
      this.scalefactor = info.scalefactor;
      this.constantyear = info.constantyear;
      if (info.Estimation != null)
      {
        this.Estimation = info.Estimation.Clone();
      }
    }
    #endregion

    #region Properties
    /// <summary>
    /// States the type of fertility forecast specification.
    /// </summary>
    public override ForecastSpecification Specification
    {
      get
      {
        return specification;
      }
      set
      {
        if (specification != value)
        {
          specification = value;
          Reset();
          saved = false;
        }
      }
    }

    /// <summary>
    /// The type of projection flow determination implied by the forecast.
    /// </summary>
    public override FlowDetermination Determination
    {
      get { return determination; }
      set
      {
        if (determination != value)
        {
          if (value == FlowDetermination.Exogenous)
          {
            if (specification == ForecastSpecification.Reference)
            {
              Specification = ForecastSpecification.Scaling;
            }
          }
          determination = value;
          saved = false;
        }
      }
    }

    /// <summary>
    /// A fertility estimation to be used as input for reference specifications. May be null.
    /// </summary>
    [XmlIgnore]
    public FertilityEstimationInfo Estimation
    {
      get { return estimation; }
      set
      {
        if (value == null)
        {
          if (estimation == null) return;
          estimation = null;
        }
        else if (specification == ForecastSpecification.Reference)
        {
          estimation = value;
        }
        else
        {
          specification = ForecastSpecification.Reference;
          Reset();
          estimation = value;
        }
        saved = false;
      }
    }

    /// <summary>
    /// States whether the current state of the fertility forecast information has been written to the database.
    /// </summary>
    [XmlIgnore]
    public override bool Saved
    {
      get
      {
        if (!base.Saved) return false;
        if (specification == ForecastSpecification.Reference)
        {
          return estimation != null && estimation.Saved;
        }
        else
        {
          return true;
        }
      }
    }

    /// <summary>
    /// If Specification is Constant this property states the first year in which fertility is held constant.
    /// </summary>
    public int ConstantYear
    {
      get { return constantyear; }
      set
      {
        if (!series.Includes(value))
        {
          throw new ArgumentException("The year starting constant specification must be within the series.");
        }
        if (constantyear != value)
        {
          if (specification != ForecastSpecification.Constant)
          {
            Specification = ForecastSpecification.Constant;
          }
          constantyear = value;
          saved = false;
        }
      }
    }

    /// <summary>
    /// If Specification is Scaling this property states the scale factor to use.
    /// </summary>
    public double ScaleFactor
    {
      get { return scalefactor; }
      set
      {
        if (value <= 0)
        {
          throw new ArgumentException("Scalefactor must be a positive value.");
        }
        if (scalefactor != value)
        {
          if (specification != ForecastSpecification.Scaling)
          {
            Specification = ForecastSpecification.Scaling;
          }
          scalefactor = value;
          saved = false;
        }
      }
    }

    /// <summary>
    /// Specifies the method used for scaling fertility rates.
    /// </summary>
    public ScaleOption ScaleMethod
    {
      get { return scalemethod; }
      set 
      {
        if (scalemethod != value)
        {
          if (specification != ForecastSpecification.Scaling)
          {
            Specification = ForecastSpecification.Scaling;
          }
          scalemethod = value;
          saved = false;
        }
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Resets to default properties for the current Specification.
    /// </summary>
    public override void Reset()
    {
      switch (specification)
      {
        case ForecastSpecification.Reference:
          Determination = FlowDetermination.Endogenous;
          scalemethod = ScaleOption.None;
          scalefactor = 1.0;
          constantyear = -1;
          break;
        case ForecastSpecification.Constant:
          scalemethod = ScaleOption.None;
          scalefactor = 1.0;
          break;
        case ForecastSpecification.Scaling:
          scalemethod = ScaleOption.UseScaleFactor;
          constantyear = -1;
          break;
      }
    }

    /// <summary>
    /// Validates the state integrity of the current FertilityInfo object.
    /// </summary>
    /// <returns>True if the FertilityInfo is in a valid state, false otherwise.</returns>
    public override bool Validate()
    {
      if (!base.Validate())
      {
        return false;
      }

      if (specification == ForecastSpecification.Constant)
      {
        if (!series.Includes(constantyear) || referenceid == -1) return false;
      }
      else if (specification == ForecastSpecification.Scaling)
      {
        if (scalefactor <= 0 || referenceid == -1) return false;
      }
      else
      {
        if (estimation == null) return false;
        if (!estimation.Validate()) return false;
        try
        {
          Series s = new Series(Estimation.Sample, Estimation.Series);
          if (!s.Includes(series))
          {
            return false;
          }
        }
        catch
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Creates a copy of the current instance.
    /// </summary>
    /// <returns>A new FertilityInfo object with the same descriptive properties as the current instance.</returns>
    public FertilityInfo Copy()
    {
      return new FertilityInfo(this);
    }

    /// <summary>
    /// Creates an identical copy of the current instance.
    /// </summary>
    /// <returns>A new FertilityInfo object identical to the current instance.</returns>
    internal FertilityInfo Clone()
    {
      FertilityInfo info = new FertilityInfo(this);
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