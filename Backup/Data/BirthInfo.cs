using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Dream.Data.Demographics
{
  /// <summary>
  /// Provides information on how attributes for births and newborn children are determined.
  /// </summary>
  public class BirthInfo : ForecastInfo
  {
    #region AssumptionFlags
    /// <summary>
    /// BirthForecast elements to be characterized using assumptions, may be combined using bitwise or.
    /// </summary>
    [Flags]
    public enum AssumptionFlags : byte
    {
      /// <summary>
      /// All BirthForecast elements are specified using historic input data.
      /// </summary>
      None = 0,
      
      /// <summary>
      /// The age distribution of mothers should be specified using an assumed distribution.
      /// </summary>
      MotherAge = 1,
      
      /// <summary>
      /// The origin distribution of newborn children should be specified using an assumed distribution.
      /// </summary>
      Origin = 2,
      
      /// <summary>
      /// The naturalization rate of newborn children should be specified using an assumed distribution.
      /// </summary>
      Naturalization = 4,
      
      /// <summary>
      /// The share of newborn children that are boys is explicitly assumed.
      /// </summary>
      BoyShare = 8,
      
      /// <summary>
      /// All BirthForecast elements are specified using assumptions.
      /// </summary>
      Default = MotherAge | Origin | Naturalization | BoyShare
    }
    #endregion

    #region Fields
    private AssumptionFlags assume = AssumptionFlags.Default;
    private double boyshare = 0.513;
    private Series boyshareinput = null;
    private Series motherageinput = null;
    private Series origininput = null;
    private Series naturalizationinput = null;
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public BirthInfo()
      : base(ForecastType.Birth)
    {
      this.assume = AssumptionFlags.Default;
      this.series = new Series(DateTime.Now.Year - 1, 1);
      this.specification = ForecastSpecification.Reference;
      this.determination = FlowDetermination.Endogenous;
      this.saved = false;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="info">The BirthInfo object to copy.</param>
    public BirthInfo(BirthInfo info)
      : base(info)
    {
      this.assume = info.assume;
      this.boyshare = info.boyshare;
      this.boyshareinput = info.boyshareinput;
      this.motherageinput = info.motherageinput;
      this.origininput = info.origininput;
      this.naturalizationinput = info.naturalizationinput;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The type of projection flow determination implied by the forecast. Can only be Endogenous.
    /// </summary>
    public override FlowDetermination Determination
    {
      get { return determination; }
      set
      {
        if (value != FlowDetermination.Endogenous)
        {
          throw new NotSupportedException("Birth forecasts can only be determined endogenously.");
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
        return specification;
      }
      set
      {
        if (value != ForecastSpecification.Reference)
        {
          throw new NotSupportedException("Birth forecast can only be specified as Reference.");
        }
        specification = value;
      }
    }

    /// <summary>
    /// Elements to be characterized using assumptions, may be combined using bitwise or.
    /// </summary>
    public AssumptionFlags AssumedElements
    {
      get { return assume; }
      set
      {
        if (assume != value)
        {
          Reset(value);
          assume = value;
          saved = false;
        }
      }
    }

    /// <summary>
    /// States if the current state of the BirthInfo has been saved to the database.
    /// </summary>
    public override bool Saved
    {
      get
      {
        if (!base.Saved) return false;
        if (boyshareinput != null && !boyshareinput.Saved) return false;
        if (motherageinput != null && !motherageinput.Saved) return false;
        if (origininput != null && !motherageinput.Saved) return false;
        if (naturalizationinput != null && !motherageinput.Saved) return false;
        return true;
      }
    }

    /// <summary>
    /// The share of newborn children that are boys.
    /// </summary>
    public double BoyShare
    {
      get { return boyshare; }
      set
      {
        if (boyshare != value)
        {
          if (AssumptionFlags.BoyShare != (assume & AssumptionFlags.BoyShare))
          {
            AssumedElements = assume ^ AssumptionFlags.BoyShare;
          }
          boyshare = value;
          saved = false;
        }
      }
    }

    /// <summary>
    /// The historic years to use when determining the share of boys in newborn children.
    /// </summary>
    public Series BoyShareInput
    {
      get { return boyshareinput; }
      set
      {
        if (!boyshareinput.Equals(value))
        {
          if (AssumptionFlags.BoyShare == (assume & AssumptionFlags.BoyShare))
          {
            AssumedElements = assume ^ AssumptionFlags.BoyShare;
          }
          boyshareinput = value.Copy();
          saved = false;
        }
      }
    }

    /// <summary>
    /// The historic years to use in calculation of mother age distribution.
    /// </summary>
    public Series MotherAgeInput
    {
      get { return motherageinput; }
      set
      {
        if (!motherageinput.Equals(value))
        {
          if (AssumptionFlags.MotherAge == (assume & AssumptionFlags.MotherAge))
          {
            AssumedElements = assume ^ AssumptionFlags.MotherAge;
          }
          motherageinput = value.Copy();
          saved = false;
        }
      }
    }

    /// <summary>
    /// The historic years to use in calculation of child origin distribution.
    /// </summary>
    public Series OriginInput
    {
      get { return origininput; }
      set
      {
        if (!origininput.Equals(value))
        {
          if (AssumptionFlags.Origin == (assume & AssumptionFlags.Origin))
          {
            AssumedElements = assume ^ AssumptionFlags.Origin;
          }
          origininput = value.Copy();
          saved = false;
        }
      }
    }

    /// <summary>
    /// The historic years to use in calculation of child naturalization rates.
    /// </summary>
    public Series NaturalizationInput
    {
      get { return naturalizationinput; }
      set
      {
        if (!naturalizationinput.Equals(value))
        {
          if (AssumptionFlags.Naturalization == (assume & AssumptionFlags.Naturalization))
          {
            AssumedElements = assume ^ AssumptionFlags.Naturalization;
          }
          naturalizationinput = value.Copy();
          saved = false;
        }
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Validates the state integrity of the current object.
    /// </summary>
    /// <returns>True if the BirthInfo object is in a valid state, false otherwise.</returns>
    public override bool Validate()
    {
      if (!base.Validate()) return false;
      if (boyshare == 0) return false;

      if (AssumptionFlags.BoyShare != (assume & AssumptionFlags.BoyShare) || !ValidateInput(boyshareinput))
      {
        return false;
      }
      if (AssumptionFlags.MotherAge != (assume & AssumptionFlags.MotherAge) || !ValidateInput(motherageinput))
      {
        return false;
      }
      if (AssumptionFlags.Naturalization != (assume & AssumptionFlags.Naturalization) || !ValidateInput(naturalizationinput))
      {
        return false;
      }
      if (AssumptionFlags.Origin != (assume & AssumptionFlags.Origin) || !ValidateInput(origininput))
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Resets to default settings for the current specification.
    /// </summary>
    public override void Reset()
    {
      Reset(AssumptionFlags.Default);
    }

    /// <summary>
    /// Creates a copy of the current instance.
    /// </summary>
    /// <returns>A new object having the same descriptive properties as the current object.</returns>
    public BirthInfo Copy()
    {
      BirthInfo bi = new BirthInfo(this);
      return bi;
    }

    /// <summary>
    /// Creates an identical copy of the current instance.
    /// </summary>
    /// <returns>A new object identical to the current instance.</returns>
    internal BirthInfo Clone()
    {
      BirthInfo bi = new BirthInfo(this);
      if (this.dbentry != null)
      {
        bi.dbentry = this.dbentry.Clone();
      }
      bi.saved = this.saved;
      return bi;
    }
    #endregion

    /// <summary>
    /// Resets the state of the current object for specified alternation of Assumption flags.
    /// </summary>
    /// <param name="flags">The new set of assumption flags.</param>
    private void Reset(AssumptionFlags flags)
    {

      if ((AssumptionFlags.MotherAge & assume) != (AssumptionFlags.MotherAge & flags))
      {
        if (AssumptionFlags.MotherAge == (AssumptionFlags.MotherAge & flags))
        {
          // MotherAge is to be assumed
          motherageinput = null;
        }
        else
        {
          // MotherAge is determined from history, supply defaults....
          if (motherageinput == null)
          {
            motherageinput = GetDefaultInput();
          }
        }
      }

      if ((AssumptionFlags.Origin & assume) != (AssumptionFlags.Origin & flags))
      {
        if (AssumptionFlags.Origin == (AssumptionFlags.Origin & flags))
        {
          // Origin is to be assumed
          origininput = null;
        }
        else
        {
          // Origin is determined from history, supply defaults....
          if (origininput == null)
          {
            origininput = GetDefaultInput();
          }
        }
      }

      if ((AssumptionFlags.Naturalization & assume) != (AssumptionFlags.Naturalization & flags))
      {
        if (AssumptionFlags.Naturalization == (AssumptionFlags.Naturalization & flags))
        {
          // Naturalization is to be assumed
          naturalizationinput = null;
        }
        else
        {
          // Naturalization is determined from history, supply defaults....
          if (naturalizationinput == null)
          {
            naturalizationinput = GetDefaultInput();
          }
        }
      }
      
      if ((AssumptionFlags.BoyShare & assume) != (AssumptionFlags.BoyShare & flags))
      {
        if (AssumptionFlags.BoyShare == (AssumptionFlags.BoyShare & flags))
        {
          // BoyShare is to be assumed
          boyshare = 0.513;
          boyshareinput = null;
        }
        else
        {
          // BoyShare is determined from history, supply defaults....
          if (boyshareinput == null)
          {
            boyshare = 0;
            boyshareinput = GetDefaultInput();
          }
        }
      }
    }

    /// <summary>
    /// Helper method for generating a default input series for any non-assumed element.
    /// </summary>
    /// <returns>A default input series of length five terminating the year before Series.StartYear.</returns>
    private Series GetDefaultInput()
    {
      Series s = new Series();
      s.Define(series.StartYear - 1, 5, false);
      return s;
    }


    /// <summary>
    /// Helper method for validating input series.
    /// </summary>
    /// <param name="s">The input series to validate.</param>
    /// <returns>True if the series ends before the current year minus 1.</returns>
    private bool ValidateInput(Series s)
    {
      return (s != null && series.Follows(s));
    }

  }
}
