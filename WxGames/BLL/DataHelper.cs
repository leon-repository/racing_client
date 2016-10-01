using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Data.SQLite;

namespace BLL
{
    /// <summary>
    /// 1，支持多数据库：添加数据库，请修改ConnString枚举，修改webConfig即可
    /// 2，支持事务操作，
    /// A:使用GetConn()，获取连接对象SQLiteConnection，用它生成SQLiteTransaction，并将参数传递给带事务的方法。使用TryCatch包围代码，发生异常Rollback即可
    /// 事例代码：
    ///        DataHelper data = new DataHelper(ConnString.clwebConnectionString.ToString());
    ///        using (SQLiteConnection conn = data.GetConn())
    ///        {
    ///            conn.Open();
    ///            SQLiteTransaction tran = conn.BeginTransaction();
    ///            try
    ///            {
    ///                 //事务操作...
    ///                 tran.Commit();
    ///             }
    ///             catch (Exception ex)
    ///             {
    ///                 tran.Rollback();
    ///                 Log.WriteLog(ex);
    ///                 throw ex;
    ///             }
    ///        }
    /// B:使用ExecuteSqlTran
    /// 3，包括常用方法，增删改查，单表操作(带事务和不带事务)，请使用Insert,Update,Delete,First,GetList
    /// 4，join查询操作，请使用 GetListNonTable
    /// </summary>
    public class DataHelper
    {
        private string connectionString = "";

        private DataHelper() { }
        public DataHelper(string connString)
        {
            this.connectionString = connString;
        }

        #region 带事务的方法

        public SQLiteConnection GetConn()
        {
            SQLiteConnection SQLiteConn = new SQLiteConnection(connectionString);
            return SQLiteConn;
        }

        public int ExecuteSql(string SQLString, SQLiteParameter[] cmdParms, SQLiteConnection conn, SQLiteTransaction tran)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            try
            {
                PrepareCommand(cmd, conn, tran, SQLString, cmdParms);
                int rows = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return rows;
            }
            catch
            {
                throw;
            }
        }

        public DataTable Query(string SQLString, SQLiteConnection conn, SQLiteTransaction tran)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLiteDataAdapter command = new SQLiteDataAdapter(SQLString, conn);
                if (tran != null) command.SelectCommand.Transaction = tran;
                command.Fill(dt);
            }
            catch
            {
                throw;
            }
            return dt;
        }


        public int ExecuteSql(string SQLString, SQLiteConnection conn, SQLiteTransaction tran)
        {
            SQLiteCommand cmd = new SQLiteCommand(SQLString, conn, tran);
            try
            {
                int rows = cmd.ExecuteNonQuery();
                return rows;
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
        }

        private SQLiteDataReader ExecuteReader(string SQLString, SQLiteParameter[] cmdParms, SQLiteTransaction tran)
        {
            SQLiteConnection connection = tran.Connection;
            SQLiteCommand cmd = new SQLiteCommand();
            try
            {
                PrepareCommand(cmd, connection, tran, SQLString, cmdParms);
                SQLiteDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据主键返回一条记录
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="pkField">字段列表</param>
        /// <returns></returns>
        public T First<T>(List<KeyValuePair<string, object>> pkField, string prefix, SQLiteTransaction tran) where T : class
        {
            if (pkField == null)
            {
                throw new ArgumentException("请设置表主键");
            }

            //生成sql
            Type objType = typeof(T);
            StringBuilder sql = new StringBuilder(string.Format("select * from {0}{1}", prefix, objType.Name));
            sql.Append(" where");
            foreach (KeyValuePair<string, object> key in pkField)
            {
                sql.Append(string.Format(" {0}=:{0} and", key.Key));
            }
            sql = sql.Remove(sql.Length - 3, 3);

            List<SQLiteParameter> paraList = new List<SQLiteParameter>();
            foreach (KeyValuePair<string, object> key in pkField)
            {
                SQLiteParameter para = new SQLiteParameter();
                para.ParameterName = ":" + key.Key;
                para.DbType = Convert2SQLiteType(objType.GetProperty(key.Key).PropertyType.Name, objType.GetProperty(key.Key).PropertyType.FullName);
                if (key.Value == null)
                {
                    para.Value = DBNull.Value;
                }
                else
                {
                    para.Value = key.Value;
                }
                paraList.Add(para);
            }

            //使用SQLiteDataReader
            using (SQLiteDataReader dr = ExecuteReader(sql.ToString(), paraList.ToArray(), tran))
            {
                if (dr.Read())
                {
                    return DataReader2Model<T>(dr);
                }
                else
                {
                    return default(T);
                }
            }
        }


        /// <summary>
        /// 根据查询条件返回记录集
        /// 表结构数据集
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="sql">查询条件（不用写where）</param>
        /// <returns></returns>
        public List<T> GetList<T>(string sql, string prefix, SQLiteTransaction tran) where T : class
        {
            Type objType = typeof(T);
            StringBuilder sqlBuilder = new StringBuilder(string.Format("select * from {0}{1}", prefix, objType.Name));
            if (!string.IsNullOrEmpty(sql))
            {
                sqlBuilder.Append(" where ");
                sqlBuilder.Append(sql);
            }

            List<T> listT = default(List<T>);
            using (SQLiteDataReader dr = ExecuteReader(sqlBuilder.ToString(), tran))
            {
                while (dr.Read())
                {
                    T model = DataReader2Model<T>(dr);
                    listT.Add(model);
                }

            }
            return listT;
        }


        /// <summary>
        /// 执行查询语句，返回SQLiteDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SQLiteDataReader</returns>
        private SQLiteDataReader ExecuteReader(string strSQL, SQLiteTransaction tran)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand(strSQL, connection, tran);
            try
            {
                connection.Open();
                SQLiteDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新一条记录
        /// 实体类：
        /// 不支持部分字段更新
        /// 数据表格式：CRM_+类名
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="pkList">where条件字段列表</param>
        /// <param name="preFix">表明前缀</param>
        /// <returns></returns>
        public int Update<T>(T model, List<KeyValuePair<string, object>> pkList, string preFix, SQLiteTransaction tran)
        {
            var objType = model.GetType();
            var data = objType.GetProperties().ToDictionary(p => p.Name.ToUpper(), p => p.GetValue(model, null));
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("UPDATE {0}", preFix + objType.Name);
            sqlBuilder.AppendFormat(" SET ");

            foreach (var key in data.Keys.Except(pkList.Select(p => p.Key.ToUpper()).ToList()))
            {
                sqlBuilder.AppendFormat("{0}=:{0},", key);
            }
            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.Append(" WHERE ");
            foreach (var key in pkList)
            {
                sqlBuilder.AppendFormat(" {0}=:{0} AND", key);
            }
            sqlBuilder.Remove(sqlBuilder.Length - 3, 3);

            List<SQLiteParameter> paraList = new List<SQLiteParameter>();

            foreach (string key in data.Keys.Except(pkList.Select(p => p.Key.ToUpper()).ToList()))
            {
                SQLiteParameter para = new SQLiteParameter();
                para.ParameterName = ":" + key;
                para.DbType = Convert2SQLiteType(objType.GetProperty(key, BindingFlags.IgnoreCase).PropertyType.Name, objType.GetProperty(key, BindingFlags.IgnoreCase).PropertyType.FullName);

                if (data[key] == null)
                {
                    para.Value = DBNull.Value;
                }
                else
                {
                    para.Value = data[key];
                }

                paraList.Add(para);
            }

            //添加主键
            foreach (var key in pkList)
            {
                SQLiteParameter para = new SQLiteParameter();
                para.ParameterName = ":" + key.Key;
                para.DbType = Convert2SQLiteType(objType.GetProperty(key.Key, BindingFlags.IgnoreCase).PropertyType.Name, objType.GetProperty(key.Key, BindingFlags.IgnoreCase).PropertyType.FullName);

                if (key.Value == null)
                {
                    para.Value = DBNull.Value;
                }
                else
                {
                    para.Value = key.Value;
                }
                paraList.Add(para);
            }

            return ExecuteSql(sqlBuilder.ToString(), paraList.ToArray(), tran.Connection, tran);
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SQLiteParameter[]）</param>
        public void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    SQLiteCommand cmd = new SQLiteCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SQLiteParameter[] cmdParms = (SQLiteParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            trans.Commit();
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        #endregion

        #region 常规的ADO.NET操作

        public int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        public bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool Exists(string strSql, params SQLiteParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object getParameterValue(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return System.DBNull.Value;
            }
            else
            {
                return text;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SQLiteException ex)
                    {
                        connection.Close();
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        connection.Close();
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SQLiteDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SQLiteDataReader</returns>
        public SQLiteDataReader ExecuteReader(string strSQL)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
            try
            {
                connection.Open();
                SQLiteDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SQLiteDataAdapter command = new SQLiteDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (SQLiteException ex)
                {
                    throw ex;
                }
                return ds;
            }
        }

        public DataTable Query2DataTable(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    SQLiteDataAdapter command = new SQLiteDataAdapter(SQLString, connection);
                    command.Fill(dt);
                }
                catch (SQLiteException ex)
                {
                    throw ex;
                }
                return dt;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (SQLiteException ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SQLiteDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SQLiteDataReader</returns>
        public SQLiteDataReader ExecuteReader(string SQLString, params SQLiteParameter[] cmdParms)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                SQLiteDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (SQLiteException ex)
                    {
                        throw ex;
                    }
                    return ds;
                }
            }
        }

        private void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, SQLiteTransaction trans, string cmdText, SQLiteParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (SQLiteParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }


        /// <summary>
        /// 构建 SQLiteCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SQLiteCommand</returns>
        private SQLiteCommand BuildQueryCommand(SQLiteConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SQLiteCommand command = new SQLiteCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SQLiteParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        /// <summary>
        /// 创建 SQLiteCommand 对象实例(用来返回一个整数值)    
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SQLiteCommand 对象实例</returns>
        private SQLiteCommand BuildIntCommand(SQLiteConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SQLiteCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SQLiteParameter("ReturnValue",
                DbType.Int32, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }
        #endregion

        #region 包装的增删改查操作

        /// <summary>
        /// 插入一条记录
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">对象</param>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public int Insert<T>(T model, string prefix)
        {
            var objType = model.GetType();
            var data = objType.GetProperties().ToDictionary(p => p.Name, p => p.GetValue(model, null));

            string tableName = "";
            if (string.IsNullOrEmpty(prefix))
            {
                tableName = objType.Name;
            }
            else
            {
                tableName = prefix + objType.Name;
            }

            var sql = string.Format("INSERT INTO {0}({1}) VALUES ({2}{3})", tableName, string.Join(",", data.Keys.ToArray()), ":", string.Join(",:", data.Keys.ToArray()));
            List<SQLiteParameter> paraList = new List<SQLiteParameter>();

            foreach (string key in data.Keys)
            {
                SQLiteParameter para = new SQLiteParameter();
                para.ParameterName = ":" + key;
                para.DbType = Convert2SQLiteType(objType.GetProperty(key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).PropertyType.Name, objType.GetProperty(key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).PropertyType.FullName);
                if (data[key] == null)
                {
                    para.Value = DBNull.Value;
                }
                else
                {
                    para.Value = data[key];
                }
                paraList.Add(para);
            }

            return ExecuteSql(sql, paraList.ToArray());
        }

        /// <summary>
        /// 转化C#类型到SQLite数据库类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private DbType Convert2SQLiteType(string name, string fullName)
        {
            switch (name)
            {
                case "Int32":
                    return DbType.Int32;
                case "String":
                    return DbType.String;
                case "DateTime":
                    return DbType.DateTime;
                case "Double":
                    return DbType.Double;
                default:
                    if (fullName.Contains("Int32"))
                    {
                        return DbType.Int32;
                    }
                    else if (fullName.Contains("DateTime"))
                    {
                        return DbType.DateTime;
                    }
                    else if (fullName.Contains("Double"))
                    {
                        return DbType.Double;
                    }

                    break;
            }

            return DbType.String;
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="model">类</param>
        /// <param name="prefix">表前缀</param>
        /// <param name="pkList">键列表</param>
        /// <returns></returns>
        public int Delete<T>(T model, string prefix, List<KeyValuePair<string, object>> pkList)
        {
            var objType = model.GetType();
            var data = objType.GetProperties().ToDictionary(p => p.Name, p => p.GetValue(model, null));
            string tableName = "";
            if (string.IsNullOrEmpty(prefix))
            {
                tableName = objType.Name;
            }
            else
            {
                tableName = prefix + objType.Name;
            }

            StringBuilder sqlBuilder = new StringBuilder(string.Format("DELETE FROM {0}", tableName));

            sqlBuilder.Append(" WHERE ");
            foreach (var key in pkList)
            {
                sqlBuilder.AppendFormat(" {0}=:{0} AND", key.Key.ToUpper());
            }
            sqlBuilder.Remove(sqlBuilder.Length - 3, 3);

            //添加主键
            List<SQLiteParameter> paraList = new List<SQLiteParameter>();
            foreach (var key in pkList)
            {
                SQLiteParameter para = new SQLiteParameter();
                para.ParameterName = ":" + key.Key;
                para.DbType = Convert2SQLiteType(objType.GetProperty(key.Key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).PropertyType.Name, objType.GetProperty(key.Key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).PropertyType.FullName);

                if (key.Value == null)
                {
                    para.Value = DBNull.Value;
                }
                else
                {
                    para.Value = key.Value;
                }
                paraList.Add(para);
            }

            return ExecuteSql(sqlBuilder.ToString(), paraList.ToArray());
        }

        /// <summary>
        /// 更新一条记录
        /// 实体类：
        /// 不支持部分字段更新
        /// </summary>
        /// <param name="model">实体类</param>
        /// <param name="pkList">where条件字段列表</param>
        /// <param name="preFix">表明前缀</param>
        /// <returns></returns>
        public int Update<T>(T model, List<KeyValuePair<string, object>> pkList, string preFix)
        {
            var objType = model.GetType();
            var data = objType.GetProperties().ToDictionary(p => p.Name.ToUpper(), p => p.GetValue(model, null));
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("UPDATE {0}", preFix + objType.Name);
            sqlBuilder.AppendFormat(" SET ");

            foreach (var key in data.Keys.Except(pkList.Select(p => p.Key.ToUpper()).ToList()))
            {
                sqlBuilder.AppendFormat("{0}=@{0},", key);
            }
            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.Append(" WHERE ");
            foreach (var key in pkList)
            {
                sqlBuilder.AppendFormat(" {0}=@{0} AND", key.Key.ToUpper());
            }
            sqlBuilder.Remove(sqlBuilder.Length - 3, 3);

            List<SQLiteParameter> paraList = new List<SQLiteParameter>();

            foreach (string key in data.Keys.Except(pkList.Select(p => p.Key.ToUpper()).ToList()))
            {
                SQLiteParameter para = new SQLiteParameter();
                para.ParameterName = "@" + key;
                para.DbType = Convert2SQLiteType(objType.GetProperty(key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).PropertyType.Name, objType.GetProperty(key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).PropertyType.FullName);

                if (data[key] == null)
                {
                    para.Value = DBNull.Value;
                }
                else
                {
                    para.Value = data[key];
                }

                paraList.Add(para);
            }

            //添加主键
            foreach (var key in pkList)
            {
                SQLiteParameter para = new SQLiteParameter();
                para.ParameterName = "@" + key.Key;
                para.DbType = Convert2SQLiteType(objType.GetProperty(key.Key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).PropertyType.Name, objType.GetProperty(key.Key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).PropertyType.FullName);

                if (key.Value == null)
                {
                    para.Value = DBNull.Value;
                }
                else
                {
                    para.Value = key.Value;
                }
                paraList.Add(para);
            }

            return ExecuteSql(sqlBuilder.ToString(), paraList.ToArray());
        }

        /// <summary>
        /// 根据键返回一条记录
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="pkField">字段列表</param>
        /// <returns>会返回null</returns>
        public T First<T>(List<KeyValuePair<string, object>> pkField, string prefix) where T : class
        {
            if (pkField == null)
            {
                throw new ArgumentException("请设置表主键");
            }

            //生成sql
            Type objType = typeof(T);
            StringBuilder sql = new StringBuilder(string.Format("select * from {0}{1}", prefix, objType.Name));
            sql.Append(" where");
            foreach (KeyValuePair<string, object> key in pkField)
            {
                sql.Append(string.Format(" {0}=:{0} and", key.Key));
            }
            sql = sql.Remove(sql.Length - 3, 3);

            List<SQLiteParameter> paraList = new List<SQLiteParameter>();
            foreach (KeyValuePair<string, object> key in pkField)
            {
                SQLiteParameter para = new SQLiteParameter();
                para.ParameterName = ":" + key.Key;
                para.DbType = Convert2SQLiteType(objType.GetProperty(key.Key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).PropertyType.Name, objType.GetProperty(key.Key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).PropertyType.FullName);
                if (key.Value == null)
                {
                    para.Value = DBNull.Value;
                }
                else
                {
                    para.Value = key.Value;
                }
                paraList.Add(para);
            }

            //使用SQLiteDataReader
            using (SQLiteDataReader dr = ExecuteReader(sql.ToString(), paraList.ToArray()))
            {
                if (dr.Read())
                {
                    return DataReader2Model<T>(dr);
                }
                else
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// 根据查询条件返回记录集
        /// 表结构数据集
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="sql">查询条件（不用写where）</param>
        /// <returns></returns>
        public List<T> GetList<T>(string sql, string prefix) where T : class
        {
            Type objType = typeof(T);
            StringBuilder sqlBuilder = new StringBuilder(string.Format("select * from {0}{1}", prefix, objType.Name));
            if (!string.IsNullOrEmpty(sql))
            {
                sqlBuilder.Append(" where ");
                sqlBuilder.Append(sql);
            }

            List<T> listT = new List<T>();
            using (SQLiteDataReader dr = ExecuteReader(sqlBuilder.ToString()))
            {
                while (dr.Read())
                {
                    T model = DataReader2Model<T>(dr);
                    listT.Add(model);
                }
            }
            return listT;
        }

        /// <summary>
        /// 根据查询条件返回记录集
        /// 非表结构数据
        /// 例如：联合查询join
        /// </summary>
        /// <typeparam name="T">模型</typeparam>
        /// <param name="sql">查询语句</param>
        /// <returns></returns>
        public List<T> GetListNonTable<T>(string sql) where T : class
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentException("sql不能为空");
            }
            List<T> listT = new List<T>();
            using (SQLiteDataReader dr = ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    T model = DataReader2Model<T>(dr);
                    listT.Add(model);
                }
            }
            return listT;
        }

        /// <summary>
        /// DataReadr转化为Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        private T DataReader2Model<T>(SQLiteDataReader dr) where T : class
        {
            List<string> list = new List<string>(dr.FieldCount);
            for (int i = 0; i < dr.FieldCount; i++)
            {
                list.Add(dr.GetName(i).ToLower());
            }
            T model = Activator.CreateInstance<T>();
            foreach (PropertyInfo pi in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
            {
                if (list.Contains(pi.Name.ToLower()))
                {
                    Type tempType = pi.PropertyType;
                    object value = dr[pi.Name];
                    if (value != DBNull.Value)
                    {
                        //判断是否是泛型
                        if (tempType.IsGenericType)
                        {
                            tempType = tempType.GetGenericArguments()[0];
                        }

                        value = Convert.ChangeType(value, tempType);//改变数据类型
                        pi.SetValue(model, value, null);
                    }
                    else
                    {
                        pi.SetValue(model, null, null);
                    }
                }
            }
            return model;
        }
        #endregion
    }
}
