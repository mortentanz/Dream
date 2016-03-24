using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Dream.Data.Demographics
{
  /// <summary>
  /// Provides information on how natuaralization is forecasted.
  /// </summary>
  public class NaturalizationInfo : ForecastInfo
  {
    #region Fields
    private Series input = null;
    private bool impute = false;
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public NaturalizationInfo()
      : base(ForecastType.Naturalization)
    {
      this.specification = ForecastSpecification.Reference;
      this.determination = FlowDetermination.Endogenous;
      input = new Series();
      input.Define(series.StartYear - 5, series.StartYear - 1);
      saved = false;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="info">The NaturalizationInfo object to copy.</param>
    public NaturalizationInfo(NaturalizationInfo info)
      : base(info)
    {
      this.input = info.input.Copy();
    }
    #endregion

    #region Overiden properties
    /// <summary>
    /// The type of projection flow determination implied by the forecast. Can only be Endogenous for naturalization.
    /// </summary>
    public override FlowDetermination Determination
    {
      get { return Determination; }
      set
      {
        if (value != FlowDetermination.Endogenous)
        {
          throw new NotSupportedException("Naturalization forecasts only support Endogenous determination of flows.");
        }
      }
    }

    /// <summary>
    /// The type of specification of the forecast. Can only be Reference.
    /// </summary>
    public override ForecastSpecification Specification
    {
      get
      {
        return Specification;
      }
      set
      {
        if (value != ForecastSpecification.Reference)
        {
          throw new NotSupportedException("Naturalization forecast only support Reference specifications.");
        }
        Specification = value;
      }
    }
    #endregion

    #region Properties
    /// <summary>
    /// The series of historic years to use as input for determining naturalization rates.
    /// </summary>
    public Series Input
    {
      get { return input; }
      set
      {
        if (!input.Equals(value))
        {
          if (!series.Follows(input))
          {
            throw new ArgumentException("The series of forecasted naturalization should follow the InputSeries.");
          }
          if (value.StartYear < 1981)
          {
            throw new ArgumentException("The InputSeries should not start before 1981 which is the first available year.");
          }
          input = value;
          saved = false;
        }
      }
    }

    /// <summary>
    /// Determines if the EndYear of the InputSeries is imputed (false by default).
    /// </summary>
    public bool Impute
    {
      get { return impute; }
      set
      {
        if (impute != value)
        {
          impute = value;
          saved = false;
        }
      }
    }

    /// <summary>
    /// States if the current state of the NaturalizationInfo has been saved to the database.
    /// </summary>
    public override bool Saved
    {
      get { return (input != null && input.Saved && base.Saved); }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Validates the state integrity of the current object.
    /// </summary>
    /// <returns>True of the NaturalizationInfo object is in a valid state, false otherwise.</returns>
    public override bool Validate()
    {
      if (!base.Validate()) return false;
      return (input != null && series.Follows(input) && input.StartYear > 1980);      
    }

    /// <summary>
    /// Resets to default settings for the current specification.
    /// </summary>
    public override void Reset()
    {
      input = new Series();
      input.Define(series.StartYear - 5, series.StartYear - 1);
      impute = false;
    }

    /// <summary>
    /// Creates a copy of the current instance.
    /// </summary>
    /// <returns>A new object having the same descriptive properties as the current object.</returns>
    public NaturalizationInfo Copy()
    {
      NaturalizationInfo ni = new NaturalizationInfo(this);
      return ni;
    }

    /// <summary>
    /// Creates an identical copy of the current instance.
    /// </summary>
    /// <returns>A new object identical to the current instance.</returns>
    internal NaturalizationInfo Clone()
    {
      NaturalizationInfo ni = new NaturalizationInfo(this);
      if (this.dbentry != null)
      {
        ni.dbentry = this.dbentry.Clone();
      }
      ni.saved = this.saved;
      return ni;
    }
    #endregion
  }
}
