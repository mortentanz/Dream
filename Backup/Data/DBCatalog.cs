using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using Dream.Data.Demographics;

namespace Dream.Data
{
  /// <summary>
  /// Provides static methods for querying and maintaining the database catalog of data series and results.
  /// </summary>
  public partial class DBCatalog
  {

    private static DataSet cache = null;

    /// <summary>
    /// The projections in the catalog. Projections cannot be modified using this property.
    /// </summary>
    public static DataTable Projections
    {
      get
      {
        if (cache == null) LoadCache();
        return cache.Tables["Projections"];
      }
    }

    /// <summary>
    /// The forecasts in the catalog. Forecasts cannot be modified using this property.
    /// </summary>
    public static DataTable Forecasts
    {
      get
      {
        if (cache == null) LoadCache();
        return cache.Tables["Forecasts"];
      }
    }

    /// <summary>
    /// The estimations in the catalog. Estimations cannot be modified using this property.
    /// </summary>
    public static DataTable Estimations
    {
      get
      {
        if (cache == null) LoadCache();
        return cache.Tables["Estimations"];
      }
    }

    /// <summary>
    /// Causes the DBCatalog to clear all cached catalog information and release resources.
    /// </summary>
    public static void Reset()
    {
      cache.Dispose();
      cache = null;
    }

    /// <summary>
    /// Loads information about a projection.
    /// </summary>
    /// <param name="projectionid">The id of the projection to be loaded.</param>
    /// <returns>A ProjectionInfo object.</returns>
    /// <exception cref="SqlException">Thrown if the database query fails, for instance by provision of an invalid projectionid.</exception>
    public static ProjectionInfo LoadProjectionInfo(short projectionid)
    {
      ProjectionInfo info = null;
      DBEntry e;
      SqlXml xml;

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        SqlCommand cmd;
        SqlDataReader r;

        cmd = DBCommands.SelectProjectionCommand(projectionid);
        cmd.Connection = conn;

        conn.Open();

        r = cmd.ExecuteReader(CommandBehavior.SingleRow);
        while (r.Read())
        {
          e = ReadEntry(r);
          xml = r.GetSqlXml(r.GetOrdinal("Parameters"));
          info = DBHelper.XmlDeserialize<ProjectionInfo>(xml);
          info.CatalogEntry = e;
        }
        r.Close();
                
        cmd = DBCommands.SelectProjectionForecastCommand(projectionid);
        cmd.Connection = conn;
        
        r = cmd.ExecuteReader(CommandBehavior.SingleResult);

        while (r.Read())
        {
          e = ReadEntry(r);
          switch (e.Class)
          { 
            case "Mortality":
              info.Mortality = (MortalityInfo)LoadForecastInfo(e.DatabaseID, conn);
              break;
            case "Fertility":
              info.Fertility = (FertilityInfo)LoadForecastInfo(e.DatabaseID, conn);
              break;
            case "Immigration":
              info.Immigration = (ImmigrationInfo)LoadForecastInfo(e.DatabaseID, conn);
              break;
            case "Emigration":
              info.Emigration = (EmigrationInfo)LoadForecastInfo(e.DatabaseID, conn);
              break;
            case "Naturalization":
              info.Naturalization = (NaturalizationInfo)LoadForecastInfo(e.DatabaseID, conn);
              break;
            case "Birth":
              info.Birth = (BirthInfo)LoadForecastInfo(e.DatabaseID, conn);
              break;
            default:
              throw new ApplicationException("An unexpected class of forecast was returned from the database.");
          }
        }
        r.Close();
        conn.Close();
        
        //SqlInt16 refid;
        //SqlInt16 estid;
        //Dictionary<ForecastType, SqlInt16> estids = new Dictionary<ForecastType, SqlInt16>(6);
        //while (r.Read())
        //{
        //  e = ReadEntry(r);
        //  refid = r.GetSqlInt16(r.GetOrdinal("ReferenceID"));
        //  estid = r.GetSqlInt16(r.GetOrdinal("EstimationID"));
        //  xml = r.GetSqlXml(r.GetOrdinal("Parameters"));
        //  switch (e.Class)
        //  {
        //    case "Mortality":
        //      MortalityInfo m = DBHelper.XmlDeserialize<MortalityInfo>(xml);
        //      m.CatalogEntry = e;
        //      m.ReferenceID = (refid.IsNull) ? (short)-1 : refid.Value;
        //      if (estid.IsNull)
        //      {
        //        m.Saved = true;
        //      }
        //      else
        //      {
        //        estids.Add(ForecastType.Mortality, estid);
        //      }
        //      info.Mortality = m;
        //      break;

        //    case "Fertility":
        //      FertilityInfo f = DBHelper.XmlDeserialize<FertilityInfo>(xml);
        //      f.CatalogEntry = e;
        //      f.ReferenceID = (refid.IsNull) ? (short)-1 : refid.Value;
        //      if (estid.IsNull)
        //      {
        //        f.Saved = true;
        //      }
        //      else
        //      {
        //        estids.Add(ForecastType.Fertility, estid);
        //      }
        //      info.Fertility = f;
        //      break;

        //    case "Immigration":
        //      ImmigrationInfo i = DBHelper.XmlDeserialize<ImmigrationInfo>(xml);
        //      i.CatalogEntry = e;
        //      i.ReferenceID = (refid.IsNull) ? (short)-1 : refid.Value;
        //      if (estid.IsNull)
        //      {
        //        i.Saved = true;
        //      }
        //      else
        //      {
        //        estids.Add(ForecastType.Immigration, estid);
        //      }
        //      info.Immigration = i;
        //      break;

        //    case "Emigration":
        //      EmigrationInfo ei = DBHelper.XmlDeserialize<EmigrationInfo>(xml);
        //      ei.CatalogEntry = e;
        //      ei.ReferenceID = (refid.IsNull) ? (short)-1 : refid.Value;
        //      if (estid.IsNull)
        //      {
        //        ei.Saved = true;
        //      }
        //      else
        //      {
        //        estids.Add(ForecastType.Emigration, estid);
        //      }
        //      info.Emigration = ei;
        //      break;

        //    case "Naturalization":
        //      NaturalizationInfo n = DBHelper.XmlDeserialize<NaturalizationInfo>(xml);
        //      n.CatalogEntry = e;
        //      n.Saved = true;
        //      info.Naturalization = n;
        //      break;

        //    case "Birth":
        //      BirthInfo b = DBHelper.XmlDeserialize<BirthInfo>(xml);
        //      b.CatalogEntry = e;
        //      b.Saved = true;
        //      info.Birth = b;
        //      break;

        //    default:
        //      throw new ApplicationException("An unexpected class of forecast was returned from the database.");
        //  }
        //}
        //r.Close();

        //if (estids.ContainsKey(ForecastType.Mortality))
        //{
        //  info.Mortality.Estimation = (MortalityEstimationInfo)LoadEstimationInfo(estids[ForecastType.Mortality].Value, conn);
        //  info.Mortality.Saved = true;
        //}
        //if (estids.ContainsKey(ForecastType.Fertility))
        //{
        //  info.Fertility.Estimation = (FertilityEstimationInfo)LoadEstimationInfo(estids[ForecastType.Fertility].Value, conn);
        //  info.Fertility.Saved = true;
        //}
        //if (estids.ContainsKey(ForecastType.Emigration))
        //{
        //  info.Emigration.Estimation = (EmigrationEstimationInfo)LoadEstimationInfo(estids[ForecastType.Emigration].Value, conn);
        //  info.Emigration.Saved = true;
        //}
        //if (estids.ContainsKey(ForecastType.Immigration))
        //{
        //  info.Immigration.Estimation = (ImmigrationEstimationInfo)LoadEstimationInfo(estids[ForecastType.Immigration].Value, conn);
        //  info.Immigration.Saved = true;
        //}

        //conn.Close();
        //info.Saved = true;
        //return info;        

      }
      info.Saved = true;
      return info;
    }

    /// <summary>
    /// Loads information about a forecast.
    /// </summary>
    /// <param name="forecastid">The id of the forecast to be loaded.</param>
    /// <returns>A ForecastInfo object that should be upcast to the relevant type.</returns>
    /// <exception cref="SqlException">Thrown if the database query fails, for instance by provision of an invalid forecastid.</exception>
    public static ForecastInfo LoadForecastInfo(short forecastid)
    {
      ForecastInfo info = null;

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        conn.Open();

        info = LoadForecastInfo(forecastid, conn);

        conn.Close();
      }        
      return info;
    }

    /// <summary>
    /// Loads information about a forecast.
    /// </summary>
    /// <param name="forecastid">The id of the forecast to be loaded.</param>
    /// <param name="conn">A SqlConnection to be used.</param>
    /// <returns>A ForecastInfo object that should be upcast to the relevant type.</returns>
    /// <exception cref="SqlException">Thrown if the database query fails, for instance by provision of an invalid forecastid.</exception>
    private static ForecastInfo LoadForecastInfo(short forecastid, SqlConnection conn)
    {
      ForecastInfo info = null;
      DBEntry e;
      SqlXml xml;

      SqlCommand cmd = DBCommands.SelectForecastCommand(forecastid);
      cmd.Connection = conn;

      using (SqlDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow))
      {
        SqlInt16 refid;
        SqlInt16 estid;

        while (r.Read())
        {
          e = ReadEntry(r);
          refid = r.GetSqlInt16(r.GetOrdinal("ReferenceID"));
          estid = r.GetSqlInt16(r.GetOrdinal("EstimationID"));
          xml = r.GetSqlXml(r.GetOrdinal("Parameters"));
          
          switch (e.Class)
          {
            case "Mortality":
              MortalityInfo m = DBHelper.XmlDeserialize<MortalityInfo>(xml);
              m.CatalogEntry = e;
              m.ReferenceID = (refid.IsNull) ? (short)-1 : refid.Value;
              if (estid.IsNull)
              {
                m.Saved = true;
              }
              else
              {
                m.Estimation = (MortalityEstimationInfo)LoadEstimationInfo((short)estid, conn);
                m.Saved = true;
              }
              info = m;
              break;

            case "Fertility":
              FertilityInfo f = DBHelper.XmlDeserialize<FertilityInfo>(xml);
              f.CatalogEntry = e;
              f.ReferenceID = (refid.IsNull) ? (short)-1 : refid.Value;
              if (estid.IsNull)
              {
                f.Saved = true;
              }
              else
              {
                f.Estimation = (FertilityEstimationInfo)LoadEstimationInfo((short)estid, conn);
                f.Saved = true;
              }
              info = f;
              break;

            case "Immigration":
              ImmigrationInfo i = DBHelper.XmlDeserialize<ImmigrationInfo>(xml);
              i.CatalogEntry = e;
              i.ReferenceID = (refid.IsNull) ? (short)-1 : refid.Value;
              if (estid.IsNull)
              {
                i.Saved = true;
              }
              else
              {
                i.Estimation = (ImmigrationEstimationInfo)LoadEstimationInfo((short)estid, conn);
                i.Saved = true;
              }
              info = i;
              break;

            case "Emigration":
              EmigrationInfo ei = DBHelper.XmlDeserialize<EmigrationInfo>(xml);
              ei.CatalogEntry = e;
              ei.ReferenceID = (refid.IsNull) ? (short)-1 : refid.Value;
              if (estid.IsNull)
              {
                ei.Saved = true;
              }
              else
              {
                ei.Estimation = (EmigrationEstimationInfo)LoadEstimationInfo((short)estid, conn);
                ei.Saved = true;
              }
              info = ei;
              break;

            case "Naturalization":
              NaturalizationInfo n = DBHelper.XmlDeserialize<NaturalizationInfo>(xml);
              n.CatalogEntry = e;
              n.Saved = true;
              info = n;
              break;

            case "Birth":
              BirthInfo b = DBHelper.XmlDeserialize<BirthInfo>(xml);
              b.CatalogEntry = e;
              b.Saved = true;
              info = b;
              break;

            default:
              throw new ApplicationException("An unexpected class of forecast was returned from the database.");
          }
        }
        r.Close();
      }

      info.Saved = true;
      return info;
    }

    /// <summary>
    /// Loads information about an estimation.
    /// </summary>
    /// <param name="estimationid">The id of the estimation to be loaded.</param>
    /// <returns>An EstimationInfo object that should be upcasted to the relevant type.</returns>
    /// <exception cref="SqlException">Thrown if the database query fails, for instance by provision of an invalid estimationid.</exception>
    public static EstimationInfo LoadEstimationInfo(short estimationid)
    {
      EstimationInfo info = null;
      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        conn.Open();

        info = LoadEstimationInfo(estimationid, conn);

        conn.Close();
      }

      return info;
    }
    
    /// <summary>
    /// Loads information about an estimation.
    /// </summary>
    /// <param name="estimationid">The id of the estimation to be loaded.</param>
    /// <param name="conn">A SqlConnection to be used.</param>
    /// <returns>An EstimationInfo object that should be upcasted to the relevant type.</returns>
    /// <exception cref="SqlException">Thrown if the database query fails, for instance by provision of an invalid estimationid.</exception>
    private static EstimationInfo LoadEstimationInfo(short estimationid, SqlConnection conn)
    {
      DBEntry e = null;
      SqlXml x = null;
      EstimationInfo info = null;
      
      SqlCommand cmd = DBCommands.SelectEstimationCommand(estimationid);
      cmd.Connection = conn;

      using (SqlDataReader r = cmd.ExecuteReader(CommandBehavior.SingleRow))
      {
        while (r.Read())
        {
          e = ReadEntry(r);
          x = r.GetSqlXml(r.GetOrdinal("Parameters"));
        }
        r.Close();
      }

      switch (e.Class)
      {
        case "Mortality":
          MortalityEstimationInfo m = DBHelper.XmlDeserialize<MortalityEstimationInfo>(x);
          m.CatalogEntry = e;
          m.Saved = true;
          info = m;
          break;

        case "Fertility":
          FertilityEstimationInfo f = DBHelper.XmlDeserialize<FertilityEstimationInfo>(x);
          f.CatalogEntry = e;
          f.Saved = true;
          info = f;
          break;

        case "Immigration":
          ImmigrationEstimationInfo i = DBHelper.XmlDeserialize<ImmigrationEstimationInfo>(x);
          i.CatalogEntry = e;
          i.Saved = true;
          info = i;
          break;

        case "Emigration":
          EmigrationEstimationInfo ei = DBHelper.XmlDeserialize<EmigrationEstimationInfo>(x);
          ei.CatalogEntry = e;
          ei.Saved = true;
          info = ei;
          break;
      }
      return info;
    }

    #region Internal Save methods called by DBStorage
    /// <summary>
    /// Saves the ProjectionInfo information to the database catalog.
    /// </summary>
    /// <param name="info">A ProjectionInfo object characterizing the projection.</param>
    /// <param name="replace">A boolean indicating whether an existing readonly projection with same title is to be replaced.</param>
    internal static void Save(ProjectionInfo info, bool replace)
    {
      if (info.Saved) return;
      if (!info.Validate())
      {
        throw new InvalidOperationException("The specified projection information is not in a valid state.");
      }
      foreach (ForecastInfo fi in info.GetForecasts())
      {
        if (!fi.Saved)
        {
          throw new InvalidOperationException("The specified projection relies on unsaved forecast information.");
        }
      }
      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        SqlCommand savecmd = DBCommands.CreateSaveCommand(info, replace);
        savecmd.Connection = conn;

        SqlCommand definecmd = DBCommands.CreateDefineCommand(info, replace);
        definecmd.Connection = conn;

        conn.Open();
        savecmd.ExecuteNonQuery();
        info.CatalogEntry.DatabaseID = (short)savecmd.Parameters["@projectionid"].Value;
        info.CatalogEntry.Revision = (byte)savecmd.Parameters["@revision"].Value;
        info.CatalogEntry.Created = (DateTime)savecmd.Parameters["@created"].Value;
        info.CatalogEntry.Modified = (DateTime)savecmd.Parameters["@modified"].Value;
        info.CatalogEntry.SaveAction = DBEntry.CatalogAction.None;
        foreach(ForecastInfo fi in info.GetForecasts())
        {
          definecmd.Parameters["@forecastid"].Value = fi.CatalogEntry.DatabaseID;
          definecmd.ExecuteNonQuery();
        }
        conn.Close();
      }
    }

    #region Save forecast info methods

    /// <summary>
    /// Saves the ForecastInfo information to the database catalog.
    /// </summary>
    /// <param name="info">A MortalityInfo object characterizing the forecast.</param>
    /// <param name="replace">A boolean indicating whether an existing readonly forecast with same title is to be replaced.</param>
    internal static void Save(MortalityInfo info, bool replace)
    {
      if (info.Saved) return;
      if (!info.Validate())
      {
        throw new InvalidOperationException("The specified mortality forecast information is not in a valid state.");
      }

      if ((info.Estimation != null) && !info.Estimation.Saved)
      {
        throw new InvalidOperationException("The specified mortality forecast relies on unsaved estimation information.");
      }

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        SqlCommand savecmd = DBCommands.CreateSaveCommand(info, replace);
        savecmd.Connection = conn;

        conn.Open();

        savecmd.ExecuteNonQuery();
        info.CatalogEntry.DatabaseID = (short)savecmd.Parameters["@forecastid"].Value;
        info.CatalogEntry.Revision = (byte)savecmd.Parameters["@revision"].Value;
        info.CatalogEntry.Created = (DateTime)savecmd.Parameters["@created"].Value;
        info.CatalogEntry.Modified = (DateTime)savecmd.Parameters["@modified"].Value;
        info.CatalogEntry.SaveAction = DBEntry.CatalogAction.None;

        conn.Close();
      }
    }

    /// <summary>
    /// Saves the ForecastInfo information to the database catalog.
    /// </summary>
    /// <param name="info">A FertilityInfo object characterizing the forecast.</param>
    /// <param name="replace">A boolean indicating whether an existing readonly forecast with same title is to be replaced.</param>
    internal static void Save(FertilityInfo info, bool replace)
    {
      if (info.Saved) return;
      if (!info.Validate())
      {
        throw new InvalidOperationException("The specified fertility forecast information is not in a valid state.");
      }

      if ((info.Estimation != null) && !info.Estimation.Saved)
      {
        throw new InvalidOperationException("The specified fertility forecast relies on unsaved estimation information.");
      }

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        SqlCommand savecmd = DBCommands.CreateSaveCommand(info, replace);
        savecmd.Connection = conn;

        conn.Open();

        savecmd.ExecuteNonQuery();
        info.CatalogEntry.DatabaseID = (short)savecmd.Parameters["@forecastid"].Value;
        info.CatalogEntry.Revision = (byte)savecmd.Parameters["@revision"].Value;
        info.CatalogEntry.Created = (DateTime)savecmd.Parameters["@created"].Value;
        info.CatalogEntry.Modified = (DateTime)savecmd.Parameters["@modified"].Value;
        info.CatalogEntry.SaveAction = DBEntry.CatalogAction.None;

        conn.Close();
      }
    }

    /// <summary>
    /// Saves the ForecastInfo information to the database catalog.
    /// </summary>
    /// <param name="info">A ImmigrationInfo object characterizing the forecast.</param>
    /// <param name="replace">A boolean indicating whether an existing readonly forecast with same title is to be replaced.</param>
    internal static void Save(ImmigrationInfo info, bool replace)
    {
      if (info.Saved) return;
      if (!info.Validate())
      {
        throw new InvalidOperationException("The specified immigration forecast information is not in a valid state.");
      }

      if ((info.Estimation != null) && !info.Estimation.Saved)
      {
        throw new InvalidOperationException("The specified immigration forecast relies on unsaved estimation information.");
      }

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        SqlCommand savecmd = DBCommands.CreateSaveCommand(info, replace);
        savecmd.Connection = conn;

        conn.Open();

        savecmd.ExecuteNonQuery();
        info.CatalogEntry.DatabaseID = (short)savecmd.Parameters["@forecastid"].Value;
        info.CatalogEntry.Revision = (byte)savecmd.Parameters["@revision"].Value;
        info.CatalogEntry.Created = (DateTime)savecmd.Parameters["@created"].Value;
        info.CatalogEntry.Modified = (DateTime)savecmd.Parameters["@modified"].Value;
        info.CatalogEntry.SaveAction = DBEntry.CatalogAction.None;

        conn.Close();
      }
    }

    /// <summary>
    /// Saves the ForecastInfo information to the database catalog.
    /// </summary>
    /// <param name="info">A EmigrationInfo object characterizing the forecast.</param>
    /// <param name="replace">A boolean indicating whether an existing readonly forecast with same title is to be replaced.</param>
    internal static void Save(EmigrationInfo info, bool replace)
    {
      if (info.Saved) return;
      if (!info.Validate())
      {
        throw new InvalidOperationException("The specified emigration forecast information is not in a valid state.");
      }

      if ((info.Estimation != null) && !info.Estimation.Saved)
      {
        throw new InvalidOperationException("The specified emigration forecast relies on unsaved estimation information.");
      }

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        SqlCommand savecmd = DBCommands.CreateSaveCommand(info, replace);
        savecmd.Connection = conn;

        conn.Open();

        savecmd.ExecuteNonQuery();
        info.CatalogEntry.DatabaseID = (short)savecmd.Parameters["@forecastid"].Value;
        info.CatalogEntry.Revision = (byte)savecmd.Parameters["@revision"].Value;
        info.CatalogEntry.Created = (DateTime)savecmd.Parameters["@created"].Value;
        info.CatalogEntry.Modified = (DateTime)savecmd.Parameters["@modified"].Value;
        info.CatalogEntry.SaveAction = DBEntry.CatalogAction.None;

        conn.Close();
      }
    }

    /// <summary>
    /// Saves the ForecastInfo information to the database catalog.
    /// </summary>
    /// <param name="info">A NaturalizationInfo object characterizing the forecast.</param>
    /// <param name="replace">A boolean indicating whether an existing readonly forecast with same title is to be replaced.</param>
    internal static void Save(NaturalizationInfo info, bool replace)
    {
      if (info.Saved) return;
      if (!info.Validate())
      {
        throw new InvalidOperationException("The specified naturalization forecast information is not in a valid state.");
      }

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        SqlCommand savecmd = DBCommands.CreateSaveCommand(info, replace);
        savecmd.Connection = conn;

        conn.Open();

        savecmd.ExecuteNonQuery();
        info.CatalogEntry.DatabaseID = (short)savecmd.Parameters["@forecastid"].Value;
        info.CatalogEntry.Revision = (byte)savecmd.Parameters["@revision"].Value;
        info.CatalogEntry.Created = (DateTime)savecmd.Parameters["@created"].Value;
        info.CatalogEntry.Modified = (DateTime)savecmd.Parameters["@modified"].Value;
        info.CatalogEntry.SaveAction = DBEntry.CatalogAction.None;

        conn.Close();
      }
    }

    /// <summary>
    /// Saves the ForecastInfo information to the database catalog.
    /// </summary>
    /// <param name="info">A BirthInfo object characterizing the forecast.</param>
    /// <param name="replace">A boolean indicating whether an existing readonly forecast with same title is to be replaced.</param>
    internal static void Save(BirthInfo info, bool replace)
    {
      if (info.Saved) return;
      if (!info.Validate())
      {
        throw new InvalidOperationException("The specified naturalization forecast information is not in a valid state.");
      }

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        SqlCommand savecmd = DBCommands.CreateSaveCommand(info, replace);
        savecmd.Connection = conn;

        conn.Open();

        savecmd.ExecuteNonQuery();
        info.CatalogEntry.DatabaseID = (short)savecmd.Parameters["@forecastid"].Value;
        info.CatalogEntry.Revision = (byte)savecmd.Parameters["@revision"].Value;
        info.CatalogEntry.Created = (DateTime)savecmd.Parameters["@created"].Value;
        info.CatalogEntry.Modified = (DateTime)savecmd.Parameters["@modified"].Value;
        info.CatalogEntry.SaveAction = DBEntry.CatalogAction.None;

        conn.Close();
      }
    }
    #endregion

    #region Save estimation info methods

    /// <summary>
    /// Saves the EstimationInfo information to the database catalog.
    /// </summary>
    /// <param name="info">A MortalityEstimationInfo object characterizing the estimation.</param>
    /// <param name="replace">A boolean indicating whether an existing readonly estimation with same title is to be replaced.</param>
    internal static void Save(MortalityEstimationInfo info, bool replace)
    {
      if (info.Saved) return;
      if (!info.Validate())
      {
        throw new InvalidOperationException("The specified mortality estimation information is not in a valid state.");
      }

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        SqlCommand savecmd = DBCommands.CreateSaveCommand(info, replace);
        savecmd.Connection = conn;

        conn.Open();

        savecmd.ExecuteNonQuery();
        info.CatalogEntry.DatabaseID = (short)savecmd.Parameters["@estimationid"].Value;
        info.CatalogEntry.Revision = (byte)savecmd.Parameters["@revision"].Value;
        info.CatalogEntry.Created = (DateTime)savecmd.Parameters["@created"].Value;
        info.CatalogEntry.Modified = (DateTime)savecmd.Parameters["@modified"].Value;
        info.CatalogEntry.SaveAction = DBEntry.CatalogAction.None;

        conn.Close();
      }
    }
    
    /// <summary>
    /// Saves the EstimationInfo information to the database catalog.
    /// </summary>
    /// <param name="info">A FeritilyEstimationInfo object characterizing the estimation.</param>
    /// <param name="replace">A boolean indicating whether an existing readonly estimation with same title is to be replaced.</param>
    internal static void Save(FertilityEstimationInfo info, bool replace)
    {
      if (info.Saved) return;
      if (!info.Validate())
      {
        throw new InvalidOperationException("The specified fertility estimation information is not in a valid state.");
      }

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        SqlCommand savecmd = DBCommands.CreateSaveCommand(info, replace);
        savecmd.Connection = conn;

        conn.Open();

        savecmd.ExecuteNonQuery();
        info.CatalogEntry.DatabaseID = (short)savecmd.Parameters["@estimationid"].Value;
        info.CatalogEntry.Revision = (byte)savecmd.Parameters["@revision"].Value;
        info.CatalogEntry.Created = (DateTime)savecmd.Parameters["@created"].Value;
        info.CatalogEntry.Modified = (DateTime)savecmd.Parameters["@modified"].Value;
        info.CatalogEntry.SaveAction = DBEntry.CatalogAction.None;

        conn.Close();
      }
    }

    /// <summary>
    /// Saves the EstimationInfo information to the database catalog.
    /// </summary>
    /// <param name="info">A ImmigrationEstimationInfo object characterizing the estimation.</param>
    /// <param name="replace">A boolean indicating whether an existing readonly estimation with same title is to be replaced.</param>
    internal static void Save(ImmigrationEstimationInfo info, bool replace)
    {
      if (info.Saved) return;
      if (!info.Validate())
      {
        throw new InvalidOperationException("The specified Immigration estimation information is not in a valid state.");
      }

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        SqlCommand savecmd = DBCommands.CreateSaveCommand(info, replace);
        savecmd.Connection = conn;

        conn.Open();

        savecmd.ExecuteNonQuery();
        info.CatalogEntry.DatabaseID = (short)savecmd.Parameters["@estimationid"].Value;
        info.CatalogEntry.Revision = (byte)savecmd.Parameters["@revision"].Value;
        info.CatalogEntry.Created = (DateTime)savecmd.Parameters["@created"].Value;
        info.CatalogEntry.Modified = (DateTime)savecmd.Parameters["@modified"].Value;
        info.CatalogEntry.SaveAction = DBEntry.CatalogAction.None;

        conn.Close();
      }
    }

    /// <summary>
    /// Saves the EstimationInfo information to the database catalog.
    /// </summary>
    /// <param name="info">A EmigrationEstimationInfo object characterizing the estimation.</param>
    /// <param name="replace">A boolean indicating whether an existing readonly estimation with same title is to be replaced.</param>
    internal static void Save(EmigrationEstimationInfo info, bool replace)
    {
      if (info.Saved) return;
      if (!info.Validate())
      {
        throw new InvalidOperationException("The specified Emigration estimation information is not in a valid state.");
      }

      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        SqlCommand savecmd = DBCommands.CreateSaveCommand(info, replace);
        savecmd.Connection = conn;

        conn.Open();

        savecmd.ExecuteNonQuery();
        info.CatalogEntry.DatabaseID = (short)savecmd.Parameters["@estimationid"].Value;
        info.CatalogEntry.Revision = (byte)savecmd.Parameters["@revision"].Value;
        info.CatalogEntry.Created = (DateTime)savecmd.Parameters["@created"].Value;
        info.CatalogEntry.Modified = (DateTime)savecmd.Parameters["@modified"].Value;
        info.CatalogEntry.SaveAction = DBEntry.CatalogAction.None;

        conn.Close();
      }
    }
    #endregion

    #endregion

    #region Private helper methods
    /// <summary>
    /// Reads a DBEntry from a cached SqlDataReader.
    /// </summary>
    /// <param name="row">A SqlDataReader object to be represented as a DBEntry.</param>
    /// <returns>A DBEntry object wrapping the field values stored in the SqlDataReader.</returns>
    private static DBEntry ReadEntry(SqlDataReader r)
    {
      return new DBEntry("X");
      // TODO: Implement method for creating a DBEntry from a SqlDataReader
    }

    /// <summary>
    /// Reads a DBEntry from a cached DataRow.
    /// </summary>
    /// <param name="row">A DataRow object to be represented as a DBEntry.</param>
    /// <returns>A DBEntry object wrapping the field values stored in the DataRow.</returns>
    private static DBEntry ReadEntry(DataRow row)
    {
      short databaseid = (short)row["DatabaseID"];
      string title = (string)row["Title"];
      string classname = row.Table.Columns.Contains("Class") ? (string)row["Class"] : string.Empty;
      string caption = !row.IsNull("Caption") ? (string)row["Caption"] : string.Empty;
      Byte revision = (byte)row["Revision"];
      DateTime created = (DateTime)row["Created"];
      DateTime modified = (DateTime)row["Modified"];
      bool isreadonly = (bool)row["IsReadOnly"];
      bool ispublished = (bool)row["IsPublished"];
      string texten = (string)row["TextEn"];
      string textda = (string)row["TextDa"];

      DBEntry e = new DBEntry(
        databaseid, title, classname, caption, revision, 
        created, modified, isreadonly, ispublished, texten, textda
      );
      e.SaveAction = DBEntry.CatalogAction.None;
      return e;
    }
    #endregion

    #region Caching of catalog information
    private static void LoadCache()
    {
      cache = new DataSet();
      DataTable projections = cache.Tables.Add("Projections");
      DataTable forecasts = cache.Tables.Add("Forecasts");
      DataTable estimations = cache.Tables.Add("Estimations");

      SqlCommand cmd = null;
      SqlDataReader r = null;
      using (SqlConnection conn = DBHelper.CreateConnection())
      {
        conn.Open();
        cmd = DBCommands.SelectProjectionCommand();
        cmd.Connection = conn;
        r = cmd.ExecuteReader(CommandBehavior.SingleResult);
        projections.Load(r);

        cmd = DBCommands.SelectForecastCommand();
        cmd.Connection = conn;
        r = cmd.ExecuteReader(CommandBehavior.SingleResult);
        forecasts.Load(r);

        cmd = DBCommands.SelectEstimationCommand();
        cmd.Connection = conn;
        r = cmd.ExecuteReader(CommandBehavior.SingleResult);
        estimations.Load(r);
      }

      DataColumn[] key = new DataColumn[0];
      foreach (DataTable table in cache.Tables)
      {
        key[0] = table.Columns[0];
        table.PrimaryKey = key;
      }
    }

    #endregion
  }
}
