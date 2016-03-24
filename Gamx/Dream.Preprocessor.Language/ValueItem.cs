using System;
using System.Collections.Generic;
using System.Text;

namespace Dream.Preprocessor.Language
{
  public abstract class ValueItem : Item
  {
    protected List<Domain> domains = null;

    public int Dimensions
    {
      get
      {
        if (domains == null)
          return 0;
        else
          return domains.Count;
      }
    }

    public bool IsScalar
    {
      get { return Dimensions == 0; }
    }

    public void AddDomain(Domain domain)
    {
      if (domains == null)
        domains = new List<Domain>();
      domains.Add(domain);
    }

    public void AddDomains(params Domain[] aDomainlist)
    {
      if (domains == null)
        domains = new List<Domain>(10);
      domains.AddRange(aDomainlist);
    }

  }
}
