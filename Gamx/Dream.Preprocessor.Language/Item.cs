using System;
using System.Collections.Generic;
using System.Text;

namespace Dream.Preprocessor.Language
{
  public abstract class Item
  {
    protected string name;
    protected string text = "";
    protected int declarationline;
    protected bool isdefined;
    protected bool isreferenced;

    protected Item() { }

    protected Item(string name, int linenumber)
    {
      this.name = name;
      this.declarationline = linenumber;
    }
    
    protected Item(string name, int linenumber, string text) : this(name, linenumber)
    {
      this.text = text;
    }

    public string Name
    {
      get { return name; }
    }
    
    public string Text
    {
      get { return text; }
    }

    public virtual bool IsDefined
    {
      get { return isdefined; }
      internal set { isdefined = value; }
    }

    public bool IsReferenced
    {
      get { return isreferenced; }
      internal set { isreferenced = value; }
    }

  }
}
