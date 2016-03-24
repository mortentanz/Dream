using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Dream.Data.Demographics
{
  /// <summary>
  /// Provides information about how mortality is forecasted.
  /// </summary>
  public class MortalityInfo : ForecastInfo
  {
    
    #region Fields
    private MortalityEstimationInfo estimation = null;
    private ScaleOption scalemethod = ScaleOption.None;
    private double scalefactor = 1.0;
    private int constantyear = -1;
    #endregion

    #region Supported scale methods
    /// <summary>
    /// Supported methods for scaling mortality rates.
    /// </summary>
    public enum ScaleOption
    {
      /// <summary>
      /// The mortality forecast does not use scaling in its specification (default).
      /// </summary>
      None,
      /// <summary>
      /// Scale by simple application of the scalefactor (default if specification is set to Scaling).
      /// </summary>
      UseScaleFactor,
      /// <summary>
      /// Scale to target expected lifetime.
      /// </summary>
      UseLifetimeTarget
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor for a reference mortality forecast. User must set the Estimation property.
    /// </summary>
    public MortalityInfo() : this(ForecastSpecification.Reference) { }

    /// <summary>
    /// Constructor for a specific mortality forecast specification.
    /// </summary>
    /// <param name="specification">A ForecastSpecification value characterizing the forecast.</param>
    public MortalityInfo(ForecastSpecification specification)
      : base(ForecastType.Mortality)
    {
      this.specification = specification;
      Reset();
      this.saved = false;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="info">The MortalityInfo object to copy.</param>
    public MortalityInfo(MortalityInfo info)
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
    /// States the type of mortality forecast specification.
    /// </summary>
    /// <remarks>If Specification is Reference an Estimation is required.</remarks>
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
    /// <remarks>FlowDetermination can only be Endogenous.</remarks>
    public override FlowDetermination Determination
    {
      get
      {
        return determination;
      }
      set
      {
        if (determination != FlowDetermination.Endogenous)
        {
          throw new NotSupportedException("Mortality forecasts only support endogenous determination of flows.");
        }
      }
    }

    /// <summary>
    /// A mortality estimation to be used as input for reference specifications. May be null.
    /// </summary>
    [XmlIgnore]
    public MortalityEstimationInfo Estimation
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
          Specification = ForecastSpecification.Reference;
          estimation = value;
        }
        saved = false;
      }
    }

    /// <summary>
    /// States whether the current state of the mortality forecast information has been written to the database.
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
    /// If Specification is Constant this property states the first year in which mortality is held constant.
    /// </summary>
    public int ConstantYear
    {
      get { return constantyear; }
      set {
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
      set {
        if (value < 0)
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
    /// Specifies the method used for scaling mortality rates.
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
    /// Validates the state integrity of the current MortalityInfo object.
    /// </summary>
    /// <returns>True if the object is in a valid state, false otherwise.</returns>
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
    /// <returns>A new object with the same descriptive properties as the current instance.</returns>
    public MortalityInfo Copy()
    {
      return new MortalityInfo(this);
    }

    /// <summary>
    /// Creates an identical copy of the current instance.
    /// </summary>
    /// <returns>A new object identical to the current instance.</returns>
    internal MortalityInfo Clone()
    {
      MortalityInfo info = new MortalityInfo(this);
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
