using System;
using System.Collections.Generic;
using System.Text;

namespace Dream.Preprocessor.Language
{
  /// <summary>
  /// Represents a control variable.
  /// </summary>
  public class CompilerString
  {
    
    // Fields
    private string name;
    private string value = null;
    private StringScope scope;

    /// <summary>
    /// Contruct a new control variable.
    /// </summary>
    /// <param name="name">The name of the control variable.</param>
    /// <param name="value">The value of the control variable.</param>
    /// <param name="scope">The declared scope of the control variable.</param>
    public CompilerString(string name, string value, StringScope scope)
    {
      this.name = name;
      this.value = value;
      this.scope = scope;
    }

    /// <summary>
    /// Construct a new control variable.
    /// </summary>
    /// <param name="name">The name of the control variable.</param>
    /// <param name="value">The value of the control variable.</param>
    public CompilerString(string name, string value) : this(name, value, StringScope.Scoped) { }

    /// <summary>
    /// Construct a new control variable.
    /// </summary>
    /// <param name="name">The name of the control variable.</param>
    /// <param name="scope">The declared scope of the control variable</param>
    public CompilerString(string name, StringScope scope) : this(name, null, scope) { }


    /// <summary>
    /// The name of the control variable.
    /// </summary>
    public string Name
    {
      get { return name; }
      set { name = value; }
    }

    /// <summary>
    /// The value of the control variable. May be null.
    /// </summary>
    public string Value
    {
      get { return value;}
      set { this.value = value; }
    }

    /// <summary>
    /// The scope of the control variable.
    /// </summary>
    public StringScope Scope
    {
      get { return scope; }
      set { scope = value; }
    }

    /// <summary>
    /// States if a value is defined for the control variable.
    /// </summary>
    public bool IsDefined
    {
      get { return (value != null); }
    }
  }
}
