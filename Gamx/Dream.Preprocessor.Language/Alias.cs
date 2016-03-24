using System;
using System.Collections.Generic;
using System.Text;

namespace Dream.Preprocessor.Language
{
  public class Alias
  {
    
    private string name;
    private Domain refersto;

    /// <summary>
    /// Construct a new <c>Alias</c> object.
    /// </summary>
    /// <param name="name">The name used for the <c>Alias</c>.</param>
    /// <param name="aSet">A <c>Set</c> that the <c>Alias</c> should refer to.</param>
    public Alias(string name, Domain aDomain)
    {
      this.refersto = aDomain;
      this.name = name;
    }
    
    /// <summary>
    /// The <c>Set</c> that the <c>Alias</c> refers to.
    /// </summary>
    public Domain RefersTo
    {
      get { return refersto; }
    }

    /// <summary>
    /// The name of the <c>Alias</c>.
    /// </summary>
    public string Name
    {
      get { return name; }
    }
  }
}
