using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;

namespace Dream.Preprocessor
{

  /// <summary>
  /// Collects, parses and exposes information about the context in which the preprocessor executes.
  /// </summary>
  /// <remarks>The execution context is a singleton and should be accessed using the static <c>Instance()</c> method.</remarks>
  public class ExecutionContext
  {

    // Singleton
    private ExecutionContext() { m_idirlist = new List<DirectoryInfo>(); }
    private static ExecutionContext m_instance = null;

    // Environment from the CLR
    private DirectoryInfo m_curdir;
    private string m_commandline;

    // Invokation info
    private Module m_rootmodule;
    private ExecutionAction m_action;

    // Directories for invokation
    private DirectoryInfo m_workdir;
    private List<DirectoryInfo> m_idirlist;
    
    // Directories for GAMS system
    private DirectoryInfo m_sysdir;
    private DirectoryInfo m_sysincdir;    
    private DirectoryInfo m_ldir;

    private StringCheckOption m_stringcheck;

    /// <summary>
    /// Obtain a handle for the preprocessor execution context.
    /// </summary>
    /// <returns>The execution context instance (singleton) for the preprocessor.</returns>
    public static ExecutionContext Instance()
    {
      if (m_instance == null)
      {
        m_instance = new ExecutionContext();
        m_instance.m_curdir = new DirectoryInfo(System.Environment.CurrentDirectory);
        m_instance.m_commandline = System.Environment.CommandLine;
        m_instance.m_workdir = m_instance.m_curdir;
        m_instance.m_idirlist.Add(m_instance.m_workdir);
        m_instance.LoadConfiguration();
        m_instance.ParseParams();
      }
      return m_instance;
    }

    /// <summary>
    /// Loads the configuration of the preprocessor from the settings file.
    /// </summary>
    public void LoadConfiguration()
    {
      // TODO: Replace with code querying the Settings of the application.
      //throw new NotImplementedException();

      string gamspath = Properties.Settings.Default.GamsPath;
      if(Directory.Exists(gamspath))
      {
        m_sysdir = new DirectoryInfo(gamspath);
      }

      m_sysdir = new DirectoryInfo(@"C:\Gams");
      m_sysincdir = new DirectoryInfo(@"C:\Gams");
      m_ldir = new DirectoryInfo(@"C:\Gams\inclib");
      return;
    }
    
    /// <summary>
    /// Saves the current configuration options for the preprocessor to the settings file.
    /// </summary>
    public void SaveConfiguration()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// The module stated on the commandline.
    /// </summary>
    public Module RootModule
    {
      get { return m_instance.m_rootmodule; }
    }

    /// <summary>
    /// The intended action of the execution.
    /// </summary>
    public ExecutionAction Action
    { 
      get { return m_action; } 
    }

    /// <summary>
    /// Directory used for processing and generated output. Often implicitly defined by the current directory.
    /// </summary>
    public DirectoryInfo WorkingDirectory
    {
      get { return m_workdir; }
    }

    /// <summary>
    /// Directory containing the input module. Often implicitly defined by the current directory.
    /// </summary>
    public DirectoryInfo InputDirectory
    {
      get { return m_idirlist[0]; }
    }

    public ReadOnlyCollection<DirectoryInfo> InputDirectories
    {
      get { return m_idirlist.AsReadOnly(); }
    }

    /// <summary>
    /// Directory from which the preprocessor was invoked.
    /// </summary>
    public DirectoryInfo CurrentDirectory
    {
      get { return m_curdir; }
    }

    /// <summary>
    /// Library to search for modules on libinclude directives.
    /// </summary>
    public DirectoryInfo LibincludeDirectory
    {
      get { return m_ldir; }
    }

    /// <summary>
    /// Library to search for modules on sysinclude directives.
    /// </summary>
    public DirectoryInfo SysincludeDirectory
    {
      get { return m_sysincdir; }
    }

    /// <summary>
    /// Option for resolving undefined control variable symbols.
    /// </summary>
    public StringCheckOption StringCheckOption
    {
      get { return m_stringcheck; }
      internal set { m_stringcheck = value; }
    }

    /// <summary>
    /// Parses execution parameters from the internal gams parameter file (gmsprmnt.txt), the commandline
    /// and from any parameter file specified on the command line.
    /// </summary>
    private void ParseParams()
    {

      Params cmdparams = new Params(m_commandline);
      m_rootmodule = new Module(new FileInfo(cmdparams.ModuleFilename));
      
      throw new NotImplementedException();

      // Parse commandline
        // If commandline includes an alternative sysdir, search here for parameter file
        // If commandline includes a parameterfile or current directory contain a gamsparm.txt file, read this
      // Parse parameter file

      // Merge options in parameter file with those stated on commandline (commandline takes precedence)

      // Set options of the execution context
      // Populate root module with any control variables declared in execution params (including scope)
      

    }

  }
}
