using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

using System.Data;

/// <summary>
///SqlStuff 的摘要说明
/// </summary>
public class SqlStuff
{
    public static SqlConnection sc = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["connStr"].ToString()); 
   public static void RunTimeLog(string LogType ,string Ip,string RequestUrl , string Desc ){
       try
       {
           SqlCommand Sc = new SqlCommand("insert into SystemLog (Logtype,Ip,RequestUrl,[Desc]) values('" + LogType + "','" + Ip + "','" + RequestUrl + "','" + Desc + "')", sc);
           if (sc.State!=ConnectionState.Open)
           {
               sc.Open();
           }
          
           Sc.ExecuteNonQuery();
           if (sc.State == ConnectionState.Open)
           {
               sc.Close();
           }
       }
       catch(Exception ex){ }
   } 
	public SqlStuff()
	{ 
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}

}