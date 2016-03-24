using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Dream.Data
{
  /// <summary>
  /// A series of years.
  /// </summary>
  public class Series : IEquatable<Series>
  {
    #region Fields
    private int start;
    private int length;
    private bool saved;
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public Series()
    {
      this.start = DateTime.Now.Year - 1;
      this.length = 100;
      saved = false;
    }
    
    /// <summary>
    /// Constructs a series of years from a start year and a length.
    /// </summary>
    /// <param name="startYear">The year that the series starts. Must be positive.</param>
    /// <param name="length">The length in years of the series. Must be positive.</param>
    public Series(int startYear, int length)
    {
      if (startYear < 1)
      {
        throw new ArgumentException("StartYear must be a positive value.", "startYear");
      }
      if (length < 1)
      {
        throw new ArgumentException("The length of the series must be positive.", "length");
      }
      this.start = startYear;
      this.length = length;
      saved = false;
    }

    /// <summary>
    /// Constructs a new Series object from two input series.
    /// </summary>
    /// <param name="first">The first series.</param>
    /// <param name="second">The second series that must continue the first series.</param>
    /// <exception cref="ArgumentNullException">Thrown if any of the supplied arguments are null.</exception>
    /// <exception cref="ArgumentException">Thrown if the second series does not continue the first.</exception>
    public Series(Series first, Series second)
    {
      if (first == null)
      {
        throw new ArgumentException("The first series is required and cannot be null.", "first");
      }
      if (second == null)
      {
        throw new ArgumentException("The second series is required and cannot be null.", "second");
      }
      if (!second.Continues(first))
      {
        throw new ArgumentException("The second series does not continue the first.");
      }
      this.Define(first.StartYear, second.EndYear);
    }
    #endregion

    #region Properties
    /// <summary>
    /// The year that the series starts. Updates the series length.
    /// </summary>
    public int StartYear
    {
      get { return start; }
      set
      {
        if (start != value)
        {
          if (value < 0)
          {
            throw new ArgumentException("The start year must be positive.");
          }
          length += (start - value);
          start = value;
          saved = false;
        }
      }
    }

    /// <summary>
    /// The year that the series ends. Udates the series length.
    /// </summary>
    public int EndYear
    {
      get { return start + length - 1; }
      set
      {
        if (start + length - 1 != value)
        {
          if (value < start)
          {
            throw new ArgumentException("The end year must follow the start year.");
          }
          length = value + 1 - start;
          saved = false;
        }
      }
    }

    /// <summary>
    /// The length of the series. Updates the EndYear.
    /// </summary>
    [XmlIgnore]
    public int Length
    {
      get { return length; }
      set
      {
        if (length != value)
        {
          if (length < 1)
          {
            throw new ArgumentException("The length of the series must be positive.");
          }
          length = value;
          saved = false;
        }
      }
    }

    /// <summary>
    /// States whether the current state of the series is saved to storage.
    /// </summary>
    [XmlIgnore]
    public bool Saved
    {
      get { return saved; }
      internal set { saved = value; }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Creates a copy of the current series of years.
    /// </summary>
    /// <returns></returns>
    public Series Copy()
    {
      return new Series(start, length);
    }

    /// <summary>
    /// Defines or modifies the Series by means of a start and an end year.
    /// </summary>
    /// <param name="startYear">The start year of the series.</param>
    /// <param name="endYear">The end year of the series.</param>
    public void Define(int startYear, int endYear)
    {
      if (startYear > endYear)
      {
        throw new ArgumentException("The endYear must follow the startYear.");
      }
      if (startYear < 1)
      {
        throw new ArgumentException("The startYear must be a positive number.", "startYear");
      }
      if(EndYear < 1)
      {
        throw new ArgumentException("The endYear must be a positive number.", "endYear");
      }

      start = startYear;
      length = endYear + 1 - startYear;
      saved = false;

    }

    /// <summary>
    /// Defines or modifies the series by means of a specified length and indication of whether to change start year.
    /// </summary>
    /// <param name="length">The new length of the series.</param>
    /// <param name="keepStartYear">If true, the current startyear is maintained, otherwise the endyear is maintained.</param>
    public void Define(int length, bool keepStartYear)
    {
      if (length < 1)
      {
        throw new ArgumentException("The length of a Series should be positive.", "length");
      }

      if (keepStartYear)
      {
        this.length = length;
      }
      else
      {
        this.start += (this.length - length);
        this.length = length;
      }
      saved = false;
    }

    /// <summary>
    /// Defines or modifies the series by means of a year, a specified length and an indication whether the year is the start year.
    /// </summary>
    /// <param name="year">The year from which to define the current Series object.</param>
    /// <param name="length">The length of the Series object.</param>
    /// <param name="isStartYear">If true the year parameter is start year, otherwise the year parameter is the end year.</param>
    public void Define(int year, int length, bool isStartYear)
    {

      if (year < 1)
      {
        throw new ArgumentException("The year must be a positive number.", "year");
      }
      if (!isStartYear && year - length < 0)
      {
        throw new ArgumentException("The definition would imply a non-postive start year.");
      }
      if (length < 1)
      {
        throw new ArgumentException("The length of a Series should be positive.", "length");
      }

      if ((isStartYear && this.start == year && this.length == length) || (this.EndYear == year && this.length == length))
      {
        return;
      }

      if (isStartYear)
      {
        this.start = year;
        this.length = length;
      }
      else
      {
        this.start = year - length + 1;
        this.length = length;
      }
      
      saved = false;
    }

    /// <summary>
    /// Gets the year corresponding to an ordinal.
    /// </summary>
    /// <param name="ordinal">The zero-based ordinal of the year.</param>
    /// <returns>The year.</returns>
    public int GetYear(int ordinal)
    {
      if (ordinal < 0 || ordinal >= Length)
      {
        throw new ArgumentOutOfRangeException("The ordinal is negative or larger than the length of the series.");
      }
      return start + ordinal;
    }

    /// <summary>
    /// Gets the zero-based ordinal position of a year in the series.
    /// </summary>
    /// <param name="year">The year for which the ordinal is to be returned.</param>
    /// <returns>The zero-based ordinal position of the year.</returns>
    public int GetOrdinal(int year)
    {
      if (year < start || year > start + length - 1)
      {
        throw new ArgumentOutOfRangeException("The specified year is not contained in the series.");
      }
      return year - start;
    }

    /// <summary>
    /// Provides simple iteration over the years in the series.
    /// </summary>
    /// <returns>An IEnumerable of type int providing simple iteration.</returns>
    public IEnumerable<int> GetYears()
    {
      for (int y = start; y < start + length; y++)
      {
        yield return y;
      }
    }

    /// <summary>
    /// Provides simple iteration over the zero-based ordinal positions of the years in the series.
    /// </summary>
    /// <returns>An IEnumerable of type int providing simple iteration.</returns>
    public IEnumerable<int> GetOrdinals()
    {
      for (int i = 0; i < length; i++)
      {
        yield return i;
      }
    }

    /// <summary>
    /// Determines if the current Series object includes all years of the specified series.
    /// </summary>
    /// <param name="s">The Series object to test for inclusion.</param>
    /// <returns>True if all years in the specified series are included by the current series, false otherwise.</returns>
    public bool Includes(Series s)
    {
      if (s == null)
      {
        throw new ArgumentNullException("Cannot test a null object for inclusion.");
      }
      return (StartYear <= s.StartYear && s.EndYear <= EndYear);
    }

    /// <summary>
    /// Determines if the current Series object includes a specified year.
    /// </summary>
    /// <param name="year">The year to test for inclusion.</param>
    /// <returns>True if the specified year is included by the current series, false otherwise.</returns>
    public bool Includes(int year)
    {
      return (start <= year & year < start + length);
    }

    /// <summary>
    /// Determines if the current Series object follow the specified series.
    /// </summary>
    /// <param name="s">The Series object to test.</param>
    /// <returns>True if all years of the specified Series object follows the end year of the current series, false otherwise.</returns>
    public bool Follows(Series s)
    {
      return (EndYear < s.StartYear);
    }

    /// <summary>
    /// Determines if the current Series object continues the specified series.
    /// </summary>
    /// <param name="s">The Series object to test.</param>
    /// <returns>True if the specified series starts exactly one year after the current series, false otherwise.</returns>
    public bool Continues(Series s)
    {
      return (this.EndYear == s.StartYear - 1);
    }

    /// <summary>
    /// Determines if the current series overlaps with the specified series.
    /// </summary>
    /// <param name="s">The Series object to test.</param>
    /// <returns>True of the specified series include some years in the specified series, false otherwise</returns>
    public bool Overlaps(Series s)
    {
      return (this.EndYear >= s.StartYear || this.StartYear >= s.EndYear);
    }

    #endregion

    #region IEquatable<Series> Members
    /// <summary>
    /// Determines whether the specified object is equal to the current Series.
    /// </summary>
    /// <param name="obj">The object to compare to the current series.</param>
    /// <returns>True if the specified object is equal to the current Series, false otherwise.</returns>
    public override bool Equals(object obj)
    {
      if (obj == null || GetType() != obj.GetType()) return false;
      Series s = (Series)obj;
      return Equals(s);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current Series.
    /// </summary>
    /// <param name="s">The Series object to compare to the current series.</param>
    /// <returns>True if the specified series is equivalent to the current Series, false otherwise.</returns>
    public bool Equals(Series s)
    {
      if (s == null) return false;
      return (start == s.start && length == s.length);
    }

    /// <summary>
    /// Serves as a hash function for Series objects.
    /// </summary>
    /// <returns>A hashcode that may be used by the framework collection class internals.</returns>
    public override int GetHashCode()
    {
      return start ^ length ^ (saved ? 1 : 0);
    }
    #endregion

  }
}
