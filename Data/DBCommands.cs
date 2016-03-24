  using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Dream.Data.Demographics;

namespace Dream.Data
{
  /// <summary>
  /// Library of utility functions for creating SqlCommand objects to use against the database.
  /// </summary>
  public static partial class DBCommands
  {
    private static readonly SqlCommand commandTemplate;

    /// <summary>
    /// Static constructor.
    /// </summary>
    static DBCommands()
    {
      commandTemplate = new SqlCommand();
      SqlParameter p = null;

      p = commandTemplate.Parameters.Add("@created", SqlDbType.DateTime);
      p.Direction = ParameterDirection.Output;
      p.IsNullable = false;

      p = commandTemplate.Parameters.Add("@modified", SqlDbType.DateTime);
      p.Direction = ParameterDirection.Output;
      p.IsNullable = false;

      p = commandTemplate.Parameters.Add("@title", SqlDbType.VarChar, 100);
      p.Direction = ParameterDirection.Input;
      p.IsNullable = false;

      p = commandTemplate.Parameters.Add("@caption", SqlDbType.VarChar, 100);
      p.Direction = ParameterDirection.Input;
      p.IsNullable = true;

      p = commandTemplate.Parameters.Add("@isreadonly", SqlDbType.Bit);
      p.Direction = ParameterDirection.Input;
      p.IsNullable = false;

      p = commandTemplate.Parameters.Add("@ispublished", SqlDbType.Bit);
      p.Direction = ParameterDirection.Input;
      p.IsNullable = false;

      p = commandTemplate.Parameters.Add("@texten", SqlDbType.VarChar, 600);
      p.Direction = ParameterDirection.Input;
      p.IsNullable = true;

      p = commandTemplate.Parameters.Add("@textda", SqlDbType.VarChar, 600);
      p.Direction = ParameterDirection.Input;
      p.IsNullable = true;

      p = commandTemplate.Parameters.Add("@params", SqlDbType.Xml);
      p.Direction = ParameterDirection.Input;
      p.IsNullable = false;

    }

    #region Select commands
    /// <summary>
    /// Creates a command for selecting all cataloged projections.
    /// </summary>
    /// <returns>An SqlCommand ready to be associated with a connection and executed. Returns multiple rows.</returns>
    public static SqlCommand SelectProjectionCommand()
    {
      StringBuilder cmdtext = new StringBuilder();
      cmdtext.Append("SELECT ");
      cmdtext.Append("ProjectionID AS DatabaseID, Title, Caption, Revision, ");
      cmdtext.Append("Created, Modified, IsReadOnly, IsPublished, TextEn, TextDa, Parameters ");
      cmdtext.Append("FROM Demographics.Projection");
      SqlCommand cmd = new SqlCommand(cmdtext.ToString());
      return cmd;
    }

    /// <summary>
    /// Creates a command for selecting a single projection from an id.
    /// </summary>
    /// <param name="id">The id of the projection to be selected.</param>
    /// <returns>An SqlCommand to be associated with a connection before execution. Returns a single-row result.</returns>
    public static SqlCommand SelectProjectionCommand(short id)
    {
      SqlCommand cmd = DBCommands.SelectProjectionCommand();
      cmd.CommandText += " WHERE ProjectionID = @projectionid";
      cmd.Parameters.AddWithValue("@projectionid", id).Direction = ParameterDirection.Input;
      return cmd;
    }

    /// <summary>
    /// Creates a command for selecting the forecasts belonging to a projection.
    /// </summary>
    /// <param name="id">The database id of the projection for which forecast information is to be returned.</param>
    /// <returns>An SqlCommand ready for execution once connected to a data source.</returns>
    public static SqlCommand SelectProjectionForecastCommand(short id)
    {
      SqlCommand cmd = DBCommands.SelectForecastCommand();
      cmd.CommandText += " f \n INNER JOIN Demographics.ProjectionForecast pf ON (f.ForecastID = pf.ForecastID)\n";
      cmd.CommandText += "WHERE pf.ProjectionID = @projectionid";
      cmd.Parameters.AddWithValue("@projectionid", id).Direction = ParameterDirection.Input;
      return cmd;
    }

    /// <summary>
    /// Creates a command for selecting the estimations belonging to a forecast.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static SqlCommand SelectForecastEstimationCommand(short id)
    {
      SqlCommand cmd = DBCommands.SelectEstimationCommand();
      cmd.CommandText += " e \n INNER JOIN Demographics.Forecast f ON (e.EstimationID = f.EstimationID)\n";
      cmd.CommandText += "WHERE f.ForecastID = @forecastid";
      cmd.Parameters.AddWithValue("@forecastid", id).Direction = ParameterDirection.Input;
      return cmd;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static SqlCommand SelectForecastCommand()
    {
      StringBuilder cmdtext = new StringBuilder();
      cmdtext.Append("SELECT ");
      cmdtext.Append("ForecastID AS DatabaseID, Class, Title, Caption, Revision, ");
      cmdtext.Append("Created, Modified, IsReadOnly, IsPublished, TextEn, TextDa, Parameters ");
      cmdtext.Append("FROM Demographics.Forecast");
      SqlCommand cmd = new SqlCommand(cmdtext.ToString());
      return cmd;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">The id of the forecast to be selected.</param>
    /// <returns>An SqlCommand ready for execution once connected to a data source.</returns>
    public static SqlCommand SelectForecastCommand(short id)
    {
      SqlCommand cmd = DBCommands.SelectProjectionCommand();
      cmd.CommandText += " WHERE ForecastID = @forecastid";
      cmd.Parameters.AddWithValue("@forecastid", id).Direction = ParameterDirection.Input;
      return cmd;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static SqlCommand SelectEstimationCommand()
    {
      StringBuilder cmdtext = new StringBuilder();
      cmdtext.Append("SELECT ");
      cmdtext.Append("EstimationID AS DatabaseID, Class, Title, Caption, Revision, ");
      cmdtext.Append("Created, Modified, IsReadOnly, IsPublished, TextEn, TextDa, Parameters ");
      cmdtext.Append("FROM Demographics.Estimation");
      SqlCommand cmd = new SqlCommand(cmdtext.ToString());
      return cmd;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static SqlCommand SelectEstimationCommand(short id)
    {
      SqlCommand cmd = DBCommands.SelectEstimationCommand();
      cmd.CommandText += " WHERE EstimationID = @estimationid";
      cmd.Parameters.AddWithValue("@estimationid", id).Direction = ParameterDirection.Input;
      return cmd;
    }

    /// <summary>
    /// Creates a select command for returning all projection result data.
    /// </summary>
    /// <param name="info">A ProjectionInfo object describing the projection.</param>
    /// <returns>An SqlCommand ready for execution once connected to a data source.</returns>
    public static SqlCommand SelectCommand(ProjectionInfo info)
    {
      SqlCommand cmd = commandTemplate.Clone();
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.CommandText = "Demographics.uspGetProjectionData";
      throw new Exception("The method is not yet implemented.");
    }

    /// <summary>
    /// Creates a select command for returning all forecast data.
    /// </summary>
    /// <param name="info">A ForecastInfo object describing the forecast.</param>
    /// <returns>An SqlCommand ready for execution once connected to a data source.</returns>
    public static SqlCommand SelectCommand(ForecastInfo info)
    {
      SqlCommand cmd = commandTemplate.Clone();
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.CommandText = "Demographics.uspGetForecastData";
      throw new Exception("The method is not yet implemented.");
    }

    /// <summary>
    /// Creates a select command for returning all estimation data.
    /// </summary>
    /// <param name="info">An EstimationInfo object describing the estimation.</param>
    /// <returns>An SqlCommand ready for execution once connected to a data source.</returns>
    public static SqlCommand SelectCommand(EstimationInfo info)
    {
      SqlCommand cmd = commandTemplate.Clone();
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.CommandText = "Demographics.uspGetEstimationData";
      throw new Exception("The method is not yet implemented.");
    }

    #endregion

    #region Factory methods for Save commands
    /// <summary>
    /// Creates and prepares an SqlCommand for saving information to the database catalog.
    /// </summary>
    /// <param name="info">A ProjectionInfo object to be saved.</param>
    /// <param name="replace">A Boolean value indicating whether any existing projection with the same title should be replaced.</param>
    /// <returns>An SqlCommand to be connected and executed using ExecuteNonQuery().</returns>
    internal static SqlCommand CreateSaveCommand(ProjectionInfo info, bool replace)
    {
      DBEntry e = info.CatalogEntry;
      SqlCommand cmd = commandTemplate.Clone();
      cmd.CommandType = CommandType.StoredProcedure;

      SqlParameter p = cmd.Parameters.Add("@projectionid", SqlDbType.SmallInt);
      if (e.SaveAction != DBEntry.CatalogAction.Insert)
      {
        cmd.CommandText = "Demographics.uspUpdateProjection";
        p.Direction = ParameterDirection.Input;
        p.Value = e.DatabaseID;
      }
      else
      {
        cmd.CommandText = "Demographics.uspInsertProjection";
        p.Direction = ParameterDirection.Output;

        p = cmd.Parameters.Add("@revision", SqlDbType.TinyInt);
        p.Direction = ParameterDirection.Output;

        p = cmd.Parameters.Add("@replace", SqlDbType.Bit);
        p.Direction = ParameterDirection.Input;
        p.Value = replace;
      }

      cmd.Parameters["@title"].Value = e.Title;
      if (e.Caption == string.Empty)
      {
        cmd.Parameters["@caption"].SqlValue = DBNull.Value;
      }
      else
      {
        cmd.Parameters["@caption"].Value = e.Caption;
      }
      cmd.Parameters["@isreadonly"].Value = e.IsReadOnly;
      cmd.Parameters["@ispublished"].Value = e.IsPublished;
      cmd.Parameters["@texten"].Value = e.TextEn;
      cmd.Parameters["@textda"].Value = e.TextDa;
      cmd.Parameters["@params"].Value = DBHelper.XmlSerialize<ProjectionInfo>(info).Value;

      return cmd;
    }

    /// <summary>
    /// Creates and prepares an SqlCommand for saving information to the database catalog.
    /// </summary>
    /// <param name="info">A ForecastInfo object to be saved.</param>
    /// <param name="replace">A Boolean value indicating whether any existing forecast with the same title should be replaced.</param>
    /// <returns>An SqlCommand to be connected and executed using ExecuteNonQuery().</returns>
    internal static SqlCommand CreateSaveCommand(ForecastInfo info, bool replace)
    {
      DBEntry e = info.CatalogEntry;
      SqlCommand cmd = commandTemplate.Clone();
      cmd.CommandType = CommandType.StoredProcedure;

      SqlParameter p;
      p = cmd.Parameters.Add("@referenceid", SqlDbType.SmallInt);
      p.Direction = ParameterDirection.Input;
      if (info.ReferenceID != -1) p.Value = info.ReferenceID;

      p = cmd.Parameters.Add("@estimationid", SqlDbType.SmallInt);
      p.Direction = ParameterDirection.Input;

      p = cmd.Parameters.Add("@forecastid", SqlDbType.SmallInt);

      if (e.SaveAction != DBEntry.CatalogAction.Insert)
      {
        cmd.CommandText = "Demographics.uspUpdateForecast";
        p.Direction = ParameterDirection.Input;
        p.Value = e.DatabaseID;
      }
      else
      {
        cmd.CommandText = "Demographics.uspInsertForecast";
        p.Direction = ParameterDirection.Output;

        p = cmd.Parameters.Add("@class", SqlDbType.VarChar, 20);
        p.Direction = ParameterDirection.Input;
        p.Value = info.Class;

        p = cmd.Parameters.Add("@revision", SqlDbType.TinyInt);
        p.Direction = ParameterDirection.Output;

        p = cmd.Parameters.Add("@replace", SqlDbType.Bit);
        p.Direction = ParameterDirection.Input;
        p.Value = replace;
      }

      switch (info.Class)
      {
        case "Fertility":
          FertilityInfo fi = (FertilityInfo)info;
          if (fi.Specification == ForecastSpecification.Reference)
          {
            cmd.Parameters["@estimationid"].Value = fi.Estimation.CatalogEntry.DatabaseID;
          }
          cmd.Parameters["@params"].Value = DBHelper.XmlSerialize<FertilityInfo>(fi);
          break;

        case "Mortality":
          MortalityInfo mi = (MortalityInfo)info;
          if (mi.Specification == ForecastSpecification.Reference)
          {
            cmd.Parameters["@estimationid"].Value = mi.Estimation.CatalogEntry.DatabaseID;
          }
          cmd.Parameters["@params"].Value = DBHelper.XmlSerialize<MortalityInfo>(mi);
          break;

        case "Immigration":
          ImmigrationInfo ii = (ImmigrationInfo)info;
          if (ii.Specification == ForecastSpecification.Reference)
          {
            cmd.Parameters["@estimationid"].Value = ii.Estimation.CatalogEntry.DatabaseID;
          }
          cmd.Parameters["@params"].Value = DBHelper.XmlSerialize<ImmigrationInfo>(ii);
          break;

        case "Emigration":
          EmigrationInfo ei = (EmigrationInfo)info;
          if (ei.Specification == ForecastSpecification.Reference)
          {
            cmd.Parameters["@estimationid"].Value = ei.Estimation.CatalogEntry.DatabaseID;
          }
          cmd.Parameters["@params"].Value = DBHelper.XmlSerialize<EmigrationInfo>(ei);
          break;

        case "Naturalization":
          NaturalizationInfo ni = (NaturalizationInfo)info;
          cmd.Parameters["@params"].Value = DBHelper.XmlSerialize<NaturalizationInfo>(ni);
          break;

        case "Birth":
          BirthInfo bi = (BirthInfo)info;
          cmd.Parameters["@params"].Value = DBHelper.XmlSerialize<BirthInfo>(bi);
          break;

      }
      return cmd;
    }

    /// <summary>
    /// Creates and prepares an SqlCommand for saving information to the database catalog.
    /// </summary>
    /// <param name="info">An EstimationInfo object to be saved.</param>
    /// <param name="replace">A Boolean value indicating whether any existing estimation with the same title should be replaced.</param>
    /// <returns>An SqlCommand to be connected and executed using ExecuteNonQuery().</returns>
    internal static SqlCommand CreateSaveCommand(EstimationInfo info, bool replace)
    {
      DBEntry e = info.CatalogEntry;
      SqlCommand cmd = commandTemplate.Clone();
      cmd.CommandType = CommandType.StoredProcedure;

      SqlParameter p = cmd.Parameters.Add("@estimationid", SqlDbType.SmallInt);
      if (e.SaveAction != DBEntry.CatalogAction.Insert)
      {
        cmd.CommandText = "Demographics.uspUpdateEstimation";
        p.Direction = ParameterDirection.Input;
        p.Value = e.DatabaseID;
      }
      else
      {
        cmd.CommandText = "Demographics.uspInsertEstimation";
        p.Direction = ParameterDirection.Output;

        p = cmd.Parameters.Add("@revision", SqlDbType.TinyInt);
        p.Direction = ParameterDirection.Output;

        p = cmd.Parameters.Add("@replace", SqlDbType.Bit);
        p.Direction = ParameterDirection.Input;
        p.Value = replace;
      }

      p = cmd.Parameters.Add("@firstyear", SqlDbType.SmallInt);
      p.Direction = ParameterDirection.Input;

      p = cmd.Parameters.Add("@lastyear", SqlDbType.SmallInt);
      p.Direction = ParameterDirection.Input;

      p = cmd.Parameters.Add("@lastsampleyear", SqlDbType.SmallInt);
      p.Direction = ParameterDirection.Input;

      switch (info.Class)
      {
        case "Fertility":
          FertilityEstimationInfo fei = (FertilityEstimationInfo)info;
          cmd.Parameters["@firstyear"].Value = (short)fei.Series.StartYear;
          cmd.Parameters["@lastyear"].Value = (short)fei.Series.EndYear;
          cmd.Parameters["@lastsampleyear"].Value = (short)fei.Sample.EndYear;
          cmd.Parameters["@params"].Value = DBHelper.XmlSerialize<FertilityEstimationInfo>(fei);
          break;

        case "Mortality":
          MortalityEstimationInfo mei = (MortalityEstimationInfo)info;
          cmd.Parameters["@firstyear"].Value = (short)mei.Series.StartYear;
          cmd.Parameters["@lastyear"].Value = (short)mei.Series.EndYear;
          cmd.Parameters["@lastsampleyear"].Value = (short)mei.Sample.EndYear;
          cmd.Parameters["@params"].Value = DBHelper.XmlSerialize<MortalityEstimationInfo>(mei);
          break;

        case "Immigration":
          ImmigrationEstimationInfo iei = (ImmigrationEstimationInfo)info;
          cmd.Parameters["@firstyear"].Value = (short)iei.Series.StartYear;
          cmd.Parameters["@lastyear"].Value = (short)iei.Series.EndYear;
          cmd.Parameters["@params"].Value = DBHelper.XmlSerialize<ImmigrationEstimationInfo>(iei);
          break;

        case "Emigration":
          EmigrationEstimationInfo eei = (EmigrationEstimationInfo)info;
          cmd.Parameters["@firstyear"].Value = (short)eei.Series.StartYear;
          cmd.Parameters["@lastyear"].Value = (short)eei.Series.EndYear;
          cmd.Parameters["@params"].Value = DBHelper.XmlSerialize<EmigrationEstimationInfo>(eei);
          break;
      }
      return cmd;
    }

    #endregion

    #region Factory method for Define command
    /// <summary>
    /// Creates and prepares an SqlCommand for defining ForecastInfo containment for a projection.
    /// </summary>
    /// <param name="info">A ProjectionInfo object to be defined in the database catalog.</param>
    /// <param name="replace">A Boolean value indicating whether any existing projection definition should be replaced.</param>
    /// <returns>An SqlCommand to be connected and executed once for each contained ForecastInfo using ExecuteNonQuery().</returns>
    internal static SqlCommand CreateDefineCommand(ProjectionInfo info, bool replace)
    {
      SqlCommand cmd = new SqlCommand("Demographics.uspDefineProjectionForecast");
      cmd.CommandType = CommandType.StoredProcedure;
      
      SqlParameter p = cmd.Parameters.Add("@projectionid", SqlDbType.SmallInt);
      p.Direction = ParameterDirection.Input;
      p.Value = info.CatalogEntry.DatabaseID;

      p = cmd.Parameters.Add("@forecastid", SqlDbType.SmallInt);
      p.Direction = ParameterDirection.Input;

      p = cmd.Parameters.Add("@replace", SqlDbType.Bit);
      p.Direction = ParameterDirection.Input;
      p.Value = replace;

      return cmd;
    }
    #endregion

  }
}
