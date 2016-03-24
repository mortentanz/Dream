using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using Dream.Preprocessor.Language;

namespace Dream.Preprocessor
{

  /// <summary>
  /// Represents a GAMS code module.
  /// </summary>
  public class Module
  {
    private FileInfo file;
    private string code;
    private Module parent;
    private List<Module> children;
    private Dictionary<string, CompilerString> controlvariables;
    private Dictionary<string, Domain> sets;
    private Dictionary<string, Parameter> parameters;
    private Dictionary<string, Variable> variables;
    private Dictionary<string, Equation> equations;
    private Dictionary<string, Model> models;

    /// <summary>
    /// Construct a module from a file and and a parent module.
    /// </summary>
    /// <param name="aFile">The file containing the code for the module.</param>
    /// <param name="theParent">The parent module that includes the code of the module.</param>
    private Module(FileInfo aFile, Module theParent)
      : this(aFile)
    {
      this.parent = theParent;
    }

    /// <summary>
    /// Contruct a module that will be the root of the program.
    /// </summary>
    /// <param name="aFile">The file containing the code module.</param>
    public Module(FileInfo aFile) 
    {
      this.file = aFile;
      this.children = new List<Module>();
    }
    
    /// <summary>
    /// The modules included by this module.
    /// </summary>
    public ReadOnlyCollection<Module> IncludedModules
    {
      get { return children.AsReadOnly(); }
    }

    /// <summary>
    /// Adds a new module to the list of child modules.
    /// </summary>
    /// <param name="file">The file containing the code module.</param>
    public void Include(FileInfo aFile)
    {
      children.Add(new Module(aFile,this));
    }


    /// <summary>
    /// Returns the entire code contents of a file.
    /// </summary>
    public string Code
    {
      get 
      {
        if (string.IsNullOrEmpty(this.code))
        {
          try
          {
            using (StreamReader reader = file.OpenText())
            {
              this.code = reader.ReadToEnd();
            }
          }
          catch (IOException ioex)
          {
            throw new PreprocessorException("The module '" + file.FullName + "' could not be read.", ioex);
          }
        }
        return this.code;
      }
    }
  }
}
