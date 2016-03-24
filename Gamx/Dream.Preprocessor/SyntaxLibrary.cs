using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Dream.Preprocessor
{
  public static class SyntaxLibrary
  {

    private static Regex paramtokenrex;
    

    private static Regex pathrex;


    static SyntaxLibrary() 
    {
      paramtokenrex = new Regex("[-/]{2}[a-z0-9_]+|[^\"\\s=-]+|\"[^\"]+\"",RegexOptions.IgnoreCase);
    }

    public static Regex ParamTokens
    {
      get { return paramtokenrex; }
    }

  }
}
