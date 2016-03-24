using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Dream.Data
{
  /// <summary>
  /// Represents a versioned database entry in the database.
  /// </summary>
  public class DBEntry
  {
    #region
    public enum CatalogAction
    {
      None,
      Insert,
      Update
    }
    #endregion


    #region Fields
    private CatalogAction action = CatalogAction.None;
    private short id = -1;
    private string title;
    private string classname = String.Empty;
    private byte revision = 0;
    private DateTime created;
    private DateTime modified;
    private string caption = String.Empty;
    private string texten = "Description is pending";
    private string textda = "Beskrivelse mangler";
    private bool isreadonly = false;
    private bool ispublished = false;

    #endregion

    #region Constructors
    /// <summary>
    /// Constructor used by data access layer.
    /// </summary>
    /// <param name="databaseId">The database surrogate key.</param>
    /// <param name="title">The title.</param>
    /// <param name="caption">The caption (may be null).</param>
    /// <param name="classname">The class name (may be null).</param>
    /// <param name="revision">The revision number.</param>
    /// <param name="created">Date that the entry was created.</param>
    /// <param name="modified">Date that the entry was last modified.</param>
    /// <param name="isReadOnly">Readonly flag.</param>
    /// <param name="isPublished">Published flag.</param>
    /// <param name="textEn">Description in English (may be specified as null).</param>
    /// <param name="textDa">Description in Danish (may be specified as null).</param>
    internal DBEntry(
      short databaseId, string title, string classname, string caption, byte revision, 
      DateTime created, DateTime modified, bool isReadOnly, bool isPublished, string textEn, string textDa
    )
    {
      this.id = databaseId;
      this.title = title;
      this.caption = (caption != null) ? caption : string.Empty;
      this.revision = revision;
      this.created = created;
      this.modified = modified;
      this.isreadonly = isReadOnly;
      this.ispublished = isPublished;
      this.texten = (textEn != null) ? textEn : string.Empty;
      this.textda = (textDa != null) ? textDa : string.Empty;
      this.action = CatalogAction.None;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="title">The title to use in the database entry.</param>
    public DBEntry(string title)
    {
      this.title = title;
      created = DateTime.Now;
      ResetIdentity();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="dbentry">A DatabaseEntry object to copy.</param>
    /// <remarks>Intended for creating in-memory copies of database entries.</remarks>
    public DBEntry(DBEntry dbEntry)
      : this(dbEntry.title)
    {
      this.created = dbEntry.created;
      this.modified = DateTime.Now;
      this.caption = dbEntry.caption;
      this.texten = dbEntry.texten;
      this.textda = dbEntry.textda;
      this.isreadonly = false;
      this.ispublished = false;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The numeric identifier for the series as used in the database (-1 if the series is not yet stored).
    /// </summary>
    public short DatabaseID
    {
      get { return id; }
      internal set { id = value; }
    }

    /// <summary>
    /// The title for the series.
    /// </summary>
    public string Title
    {
      get { return title; }
      set
      {
        if (ispublished) throw new InvalidOperationException("The cataloged entry is published and cannot be changed.");
        if ((value != string.Empty && value != null) || value.Length <= 100)
        {
          title = value;
          action = (action == CatalogAction.Insert) ? CatalogAction.Insert : CatalogAction.Update;
        }
        else
        {
          throw new ArgumentException("A title must be specified and must be shorter than 100 characters.");
        }
      }
    }

    /// <summary>
    /// The class of the described entity, May be empty or null.
    /// </summary>
    public string Class
    {
      get { return classname; }
      internal set { classname = value; }
    }

    /// <summary>
    /// The revision of the series.
    /// </summary>
    public byte Revision
    {
      get { return revision; }
      internal set { revision = value; }
    }

    /// <summary>
    /// The date and time that the series was added to the database.
    /// </summary>
    public DateTime Created
    {
      get { return created; }
      internal set { created = value; }
    }

    /// <summary>
    /// The date and time that the series was last modified in the database.
    /// </summary>
    public DateTime Modified
    {
      get { return modified; }
      internal set { modified = value; }
    }

    /// <summary>
    /// Brief description of the series in English. Must be shorter than 600 characters.
    /// </summary>
    public string TextEn
    {
      get { return texten; }
      set
      {
        if (texten == value) return;
        if (ispublished) throw new InvalidOperationException("The cataloged entry is published and cannot be changed.");
        if (value.Length <= 600)
        {
          texten = value;
          action = (action == CatalogAction.Insert) ? CatalogAction.Insert : CatalogAction.Update;
        }
        else
        {
          throw new ArgumentException("The TextEn entry must be shorter than 600 characters.");
        }
      }
    }

    /// <summary>
    /// Brief description of the series in Danish. Must be shorter than 600 characters.
    /// </summary>
    public string TextDa
    {
      get { return textda; }
      set
      {
        if (textda == value) return;
        if (ispublished) throw new InvalidOperationException("The cataloged entry is published and cannot be changed.");
        if (value.Length <= 600)
        {
          textda = value;
          action = (action == CatalogAction.Insert) ? CatalogAction.Insert : CatalogAction.Update;
        }
        else
        {
          throw new ArgumentException("The TextDa entry string must be shorter than 600 characters.");
        }
      }
    }

    /// <summary>
    /// The caption used internally for identifying the series.
    /// </summary>
    /// <remarks>An ArgumentException is thrown if the caption is set to an invalid value.</remarks>
    public string Caption
    {
      get { return caption; }
      set
      {
        if (caption == value) return;
        if (ispublished) throw new InvalidOperationException("The cataloged entry is published and cannot be changed.");
        if (ValidateCaption(value))
        {
          caption = value;
          action = (action == CatalogAction.Insert) ? CatalogAction.Insert : CatalogAction.Update;
        }
        else
        {
          throw new ArgumentException("The specified caption is not valid.");
        }
      }
    }

    /// <summary>
    /// States whether the series is marked as readonly in the database.
    /// </summary>
    public bool IsReadOnly
    {
      get { return isreadonly; }
      set
      {
        if (isreadonly == value || ispublished) return;
        isreadonly = value;
        action = (action == CatalogAction.Insert) ? CatalogAction.Insert : CatalogAction.Update;
      }
    }

    /// <summary>
    /// States whether the series is marked as published in the database.
    /// </summary>
    /// <remarks>A series is marked as published if it participated in the creation of a published projection.</remarks>
    public bool IsPublished
    {
      get { return ispublished; }
      set
      {
        if (ispublished || ispublished == value) return;
        ispublished = value;
        isreadonly = value;
        action = (action == CatalogAction.Insert) ? CatalogAction.Insert : CatalogAction.Update;
      }
    }

    /// <summary>
    /// States the action required against the database catalog for saving this entry.
    /// </summary>
    public CatalogAction SaveAction
    {
      get { return action; }
      internal set { action = value; }
    }

    /// <summary>
    /// States if the current DBEntry is stored in the database.
    /// </summary>
    public bool Saved
    {
      get { return (action == CatalogAction.None); }
      internal set { action = CatalogAction.None; }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Helper method for validating captions against DREAM system conventions.
    /// </summary>
    /// <param name="caption">The caption to be validated.</param>
    /// <returns>True if the caption conforms to system conventions, false otherwise.</returns>
    private bool ValidateCaption(string caption)
    {
      Regex filerex = new Regex("([^\\s\\\\/:*?<>|]+)");
      return (filerex.Matches(caption).Count == 1);
    }

    /// <summary>
    /// Creates an identical copy of the current DBEntry instance.
    /// </summary>
    /// <returns>A new DBEntry object identical to the current instance.</returns>
    internal DBEntry Clone()
    {
      DBEntry e = new DBEntry(
        id, title, classname, caption, revision, 
        created, modified, isreadonly, ispublished, texten, textda
      );
      e.action = this.action;
      return e;
    }

    /// <summary>
    /// Creates a copy of the current DBEntry instance.
    /// </summary>
    /// <returns>A new DBEntry object with the same descriptive properties as the current instance.</returns>
    public DBEntry Copy()
    {
      DBEntry e = new DBEntry(this);
      return e;
    }

    /// <summary>
    /// Validates the state integrity of the current DBEntry object.
    /// </summary>
    /// <returns>True if the DBEntry is in a state allowing for insertion in the database, false otherwise.</returns>
    public bool Validate()
    {
      return true;
    }

    /// <summary>
    /// Helper method resetting DatabaseID and Revision properties.
    /// </summary>
    private void ResetIdentity()
    {
      id = -1;
      revision = 0;
      modified = DateTime.Now;
      action = CatalogAction.Insert;
    }
    #endregion
  }
}