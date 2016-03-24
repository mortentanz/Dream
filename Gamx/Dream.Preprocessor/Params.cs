using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Dream.Preprocessor.Language;

// Should retrieve GAMS system directory and configuration from a settings file
// Commandline options takes precedence over GAMS settings file (gmsprmNT.txt)
namespace Dream.Preprocessor
{

  /// <summary>
  /// Wraps a GAMS commandline wrt. options affecting the execution context of the macro compiler.
  /// </summary>
  public class Params
  {

    private bool m_parsecommandline = true;
    private string m_params = string.Empty;

    private string m_module = string.Empty;
    private Dictionary<string, string> m_options = new Dictionary<string, string>();
    private List<string> m_userflags = new List<string>();
    private Dictionary<string, string> m_scopedvars = new Dictionary<string,string>();
    private Dictionary<string, string> m_localvars = new Dictionary<string, string>();
    private Dictionary<string, string> m_globalvars = new Dictionary<string,string>();

    /// <summary>
    /// Construct an object wrapping execution parameters from a commandline string.
    /// </summary>
    /// <param name="commandline">The commandline string supplied.</param>
    public Params(string commandline)
    {
      m_params = commandline;
      Parse();
    }

    /// <summary>
    /// Construct an object wrapping execution parameters from a parameter file. 
    /// </summary>
    /// <param name="paramfile">The parameter file to wrap.</param>
    public Params(FileInfo paramfile)
    {
      try
      {
        StringBuilder sb = new StringBuilder();
        string line = string.Empty;
        using (StreamReader reader = paramfile.OpenText())
        {
          while (!reader.EndOfStream)
          {
            line = reader.ReadLine().Trim();
            if (!(line.StartsWith("*") || line == string.Empty))
            {
              sb.Append(line).Append(" ");
            }
          }
        }
        m_params = sb.ToString();
        m_parsecommandline = false;
        Parse();
      }
      catch (IOException ioex)
      {
        throw new PreprocessorException("Failed to read parameter file.", ioex);
      }
    }

    /// <summary>
    /// The module file name.
    /// </summary>
    public string ModuleFilename
    {
      get { return m_module; }
    }

    /// <summary>
    /// Parses the execution parameters supplied in the constructor.
    /// </summary>
    private void Parse()
    {

      Regex tokenrex = SyntaxLibrary.ParamTokens;
      MatchCollection tokens = tokenrex.Matches(m_params);
      
      int i=0;
      string token;
      while(i < tokens.Count)
      {
        token = tokens[i].Value;
        
      }

/*
      if (commandstring == null || commandstring == string.Empty)
      {
        parsemessage = "No commandline";
        return false;
      }

      string subject = commandstring;
      int startpos = 0;
      int endpos = subject.Length;
      while (startpos < endpos)
      {

      }
*/
    }


  }
}

