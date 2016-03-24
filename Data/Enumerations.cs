using System;
using System.Collections.Generic;
using System.Text;

namespace Dream.Data.Demographics
{
  /// <summary>
  /// Types of forecasted projection components.
  /// </summary>
  public enum ForecastType
  {
    Birth,
    Fertility,
    Mortality,
    Immigration,
    Emigration,
    Naturalization
  }

  /// <summary>
  /// Types of estimated forecast components.
  /// </summary>
  public enum EstimationType
  {
    /// <summary>
    /// Inference based estimation of fertility rates.
    /// </summary>
    Fertility,

    /// <summary>
    /// Inference based estimation of mortality rates.
    /// </summary>
    Mortality,

    /// <summary>
    /// Inference or assumption based estimation of immigration.
    /// </summary>
    Immigration,

    /// <summary>
    /// Inference or assumption based estimation of emigration.
    /// </summary>
    Emigration
  }

  /// <summary>
  /// Specification types applying to forecasts.
  /// </summary>
  public enum ForecastSpecification
  {
    /// <summary>
    /// The forecast is a baseline specification or is identical to the reference projection (default).
    /// </summary>
    Reference,
    /// <summary>
    /// The forecast is specified using reference input data but held constant from a specified year.
    /// </summary>
    Constant,
    /// <summary>
    /// The forecast is specified by application of a scalefactor to the reference input data.
    /// </summary>
    Scaling
  }

  /// <summary>
  /// Types of calculation to be used for determining projection flows.
  /// </summary>
  public enum FlowDetermination
  {
    /// <summary>
    /// Flows are to be determined using exogenously given levels.
    /// </summary>
    Exogenous,
    /// <summary>
    /// Flows are to be determined endogenously using specified rates or frequencies.
    /// </summary>
    Endogenous
  }

  /// <summary>
  /// Origin flag enumeration specifiying origin and often used combinations of origins.
  /// </summary>
  [Flags]
  public enum OriginFlags
  {
    /// <summary>
    /// No origins are flagged.
    /// </summary>
    None = 0,

    #region Basic origin values
    /// <summary>
    /// Danish nationals from less developed countries with foreign citizenship.
    /// </summary>
    DanesLessNonCitizens = 1,

    /// <summary>
    /// Danish nationals from less developed countries with Danish citizenship.
    /// </summary>
    DanesLessCitizens = 2,

    /// <summary>
    /// Danish nationals from more developed countries with foreign citizenship.
    /// </summary>
    DanesMoreNonCitizens = 4,

    /// <summary>
    /// Danish nationals from more developed countries with Danish citizenship.
    /// </summary>
    DanesMoreCitizens = 8,

    /// <summary>
    /// Immigrants from less developed countries with foreign citizenship.
    /// </summary>
    ImmigrantsLessNonCitizens = 16,

    /// <summary>
    /// Immigrants from less developed countries with Danish citizenship.
    /// </summary>
    ImmigrantsLessCitizens = 32,

    /// <summary>
    /// Immigrants from more developed countries with foreign citizenship.
    /// </summary>
    ImmigrantsMoreNonCitizens = 64,

    /// <summary>
    /// Immigrants from more developed countries with Danish citizenship.
    /// </summary>
    ImmigrantsMoreCitizens = 128,

    /// <summary>
    /// Descendants from less developed countries with foreign citizenship.
    /// </summary>
    DescendantsLessNonCitizens = 256,

    /// <summary>
    /// Descendants from less developed countries with Danish citizenship.
    /// </summary>
    DescendantsLessCitizens = 512,

    /// <summary>
    /// Descendants from more developed countries with foreign citizenship.
    /// </summary>
    DescendantsMoreNonCitizens = 1024,

    /// <summary>
    /// Descendants from more developed countries with Danish citizenship.
    /// </summary>
    DescendantsMoreCitizens = 2048,
    #endregion

    #region Origin combinations
    /// <summary>
    /// All Danish nationals from more developed countries.
    /// </summary>
    DanesMore = DanesMoreNonCitizens | DanesMoreCitizens,

    /// <summary>
    /// All Danish nationals
    /// </summary>
    Danes = DanesMore | DanesLessCitizens | DanesLessNonCitizens,

    /// <summary>
    /// Immigrants from less developed countries.
    /// </summary>
    ImmigrantsLess = ImmigrantsLessNonCitizens | ImmigrantsLessCitizens,

    /// <summary>
    /// Immigrants from more developed countries.
    /// </summary>
    ImmigrantsMore = ImmigrantsMoreNonCitizens | ImmigrantsMoreCitizens,

    /// <summary>
    /// Immigrants with foreign citizenship.
    /// </summary>
    ImmigrantsNonCitizens = ImmigrantsLessNonCitizens | ImmigrantsMoreNonCitizens,

    /// <summary>
    /// Descendants with foreign citizenship.
    /// </summary>
    DescendantsNonCitizens = DescendantsLessNonCitizens | DescendantsMoreNonCitizens,

    /// <summary>
    /// Origins with foreign citizenship.
    /// </summary>
    NonCitizens = DanesLessNonCitizens | DanesMoreNonCitizens | ImmigrantsNonCitizens | DescendantsNonCitizens,

    /// <summary>
    /// All origins.
    /// </summary>
    All = Danes | ImmigrantsLess | ImmigrantsMore | DescendantsNonCitizens | DescendantsLessCitizens | DescendantsMoreCitizens,

    /// <summary>
    /// Default.
    /// </summary>
    Default = ImmigrantsNonCitizens
    #endregion
  }

}
