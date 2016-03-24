using System;
using System.Collections.Generic;
using System.Text;

namespace Dream.Preprocessor.Language
{
  public class Domain : Item
  {
    private List<Alias> m_aliases;
    private Dictionary<string, Element> m_elements;

    public List<Alias> Aliases
    {
      get { return m_aliases; }
    }

    public Dictionary<string,Element> Elements
    {
      get { return m_elements; }
    }

  }
}
