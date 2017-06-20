using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Net;
using System.Net.Cache;
using System.IO;
using System.Net.NetworkInformation;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Collections;
using System.Linq;
using System.Xml;
using org.apache.http.protocol;


/// <summary>
/// HttpHelper 的摘要说明
/// </summary>
public static  class HttpHelper
{
    static  CookieContainer Cookie;
        static  HttpHelper()
    {
         Cookie = new CookieContainer();
    }
    /// <summary>
    /// 发送请求返回内容 使用cookie版本
    /// </summary>
    /// <param name="Url"></param>
    /// <param name="Prama"></param>
    /// <param name="Cookie"></param>
    /// <returns></returns>
    public static string GetRequstData(string Url, string Prama,bool Cookies)
    {
        XmlDocument XmlDoc = new XmlDocument();
        XmlDoc.Load(System.Environment.CurrentDirectory + "/AppConfig.xml");
        string Result = "";
        string url = Prama == "" ? Url : Url + "?" + Prama;
        HttpWebRequest RequestObj = (HttpWebRequest)WebRequest.Create(url);
        //RequestObj.Proxy = null;Accept: text/html, application/xhtml+xml, image/jxr, */*
        HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);//重要，配置本次缓存策略，默认不开，则可能导致503拒绝服务
        // RequestObj.Accept = "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";//重要：模拟浏览器行为，配置报文头部 不配置可导致503拒绝服务
        RequestObj.Accept = XmlDoc.SelectSingleNode("HttpRequestConfig/Accept").InnerText;
        RequestObj.Headers.Add(HttpRequestHeader.AcceptEncoding, XmlDoc.SelectSingleNode("HttpRequestConfig/AcceptEncoding").InnerText);
        RequestObj.Headers.Add("Accept-Language", XmlDoc.SelectSingleNode("HttpRequestConfig/Accept-Language").InnerText);
        RequestObj.UserAgent = XmlDoc.SelectSingleNode("HttpRequestConfig/UserAgent").InnerText;
        RequestObj.Referer = "http://js.tudouui.com/bin/lingtong/PortalPlayer_196.swf/[[DYNAMIC]]/1";
        RequestObj.CachePolicy = policy;//配置缓存策略
        RequestObj.CookieContainer = Cookie;
        RequestObj.Timeout = 500000;//超时时间
        RequestObj.Method = XmlDoc.SelectSingleNode("HttpRequestConfig/Method").InnerText; ;//请求类型
        RequestObj.KeepAlive = true;
        HttpWebResponse response = null;
        Stream myResponseStream = null;


        try
        {
            response = (HttpWebResponse)RequestObj.GetResponse();
            myResponseStream = response.GetResponseStream();
        }
        catch (Exception ex)
        {
            // return GetRequstData(Url,Prama,ref Cookie);
            Console.WriteLine(ex.ToString());
            return "";
        }
        Cookie.Add(response.Cookies);

        StreamReader TempReder = new StreamReader(myResponseStream, System.Text.Encoding.GetEncoding("utf-8"));
        Result = TempReder.ReadToEnd();
        TempReder.Close();
        myResponseStream.Close();
        return Result;

    }
    /// <summary>
    /// 不使用缓存版本
    /// </summary>
    /// <param name="Url"></param>
    /// <param name="Prama"></param>
    /// <returns></returns>
    public static string GetRequstData(string Url, string Prama)
    {
        XmlDocument XmlDoc = new XmlDocument();
        XmlDoc.Load(System.Environment.CurrentDirectory + "/AppConfig.xml");
        string Result = "";
        string url = Prama == "" ? Url : Url + "?" + Prama;
        HttpWebRequest RequestObj = (HttpWebRequest)WebRequest.Create(url);
        //RequestObj.Proxy = null;Accept: text/html, application/xhtml+xml, image/jxr, */*
        HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);//重要，配置本次缓存策略，默认不开，则可能导致503拒绝服务

        // RequestObj.Accept = "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";//重要：模拟浏览器行为，配置报文头部 不配置可导致503拒绝服务
        RequestObj.Accept = XmlDoc.SelectSingleNode("HttpRequestConfig/Accept").InnerText;
        RequestObj.Headers.Add(HttpRequestHeader.AcceptEncoding, XmlDoc.SelectSingleNode("HttpRequestConfig/AcceptEncoding").InnerText);
        RequestObj.Headers.Add("Accept-Language", XmlDoc.SelectSingleNode("HttpRequestConfig/Accept-Language").InnerText);
        RequestObj.UserAgent = XmlDoc.SelectSingleNode("HttpRequestConfig/UserAgent").InnerText;
        RequestObj.CachePolicy = policy;//配置缓存策略
                                        //RequestObj.CookieContainer = new CookieContainer();
        RequestObj.Timeout = 50000;//超时时间
        RequestObj.Method = XmlDoc.SelectSingleNode("HttpRequestConfig/Method").InnerText; ;//请求类型
        RequestObj.KeepAlive = true;
        HttpWebResponse response = null;
        Stream myResponseStream = null;
        try
        {
            response = (HttpWebResponse)RequestObj.GetResponse();
            myResponseStream = response.GetResponseStream();
        }
        catch (Exception ex)
        {
            // return GetRequstData(Url,Prama,ref Cookie);
            Console.WriteLine(ex.ToString());
            return "";
        }

        StreamReader TempReder = new StreamReader(myResponseStream, System.Text.Encoding.GetEncoding("utf-8"));
        Result = TempReder.ReadToEnd();
        TempReder.Close();
        myResponseStream.Close();
        return Result;

    }

    public static string GetResponseUrl(HttpWebRequest RequestObj)
    { 
        WebResponse Wrs  = (HttpWebResponse)RequestObj.GetResponse();
        return Wrs.ResponseUri.ToString();
    } 

    /// <summary>
    /// 自己构造请求头部版本
    /// </summary>
    /// <param name="Url"></param>
    /// <param name="Prama"></param>
    /// <param name="Cookie"></param>
    /// <returns></returns>
    public static string GetRequstData( HttpWebRequest RequestObj)
    {
        string Result = "";
     
      //  RequestObj = (HttpWebRequest)WebRequest.Create(url);
    //RequestObj.Proxy = null;Accept: text/html, application/xhtml+xml, image/jxr, */*
         HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);//重要，配置本次缓存策略，默认不开，则可能导致503拒绝服务
    // RequestObj.Accept = "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";//重要：模拟浏览器行为，配置报文头部 不配置可导致503拒绝服务
        //RequestObj.Accept = XmlDoc.SelectSingleNode("HttpRequestConfig/Accept").InnerText;
        //RequestObj.Headers.Add(HttpRequestHeader.AcceptEncoding, XmlDoc.SelectSingleNode("HttpRequestConfig/AcceptEncoding").InnerText);
        //RequestObj.Headers.Add("Accept-Language", XmlDoc.SelectSingleNode("HttpRequestConfig/Accept-Language").InnerText);
        //RequestObj.UserAgent = XmlDoc.SelectSingleNode("HttpRequestConfig/UserAgent").InnerText;
      //  RequestObj.Referer = "http://js.tudouui.com/bin/lingtong/PortalPlayer_196.swf/[[DYNAMIC]]/1";
       // RequestObj.CachePolicy = policy;//配置缓存策略
      //  RequestObj.CookieContainer = Cookie;
       // RequestObj.KeepAlive = true;
        HttpWebResponse response = null;
         Stream myResponseStream = null;
        try
        {
            response = (HttpWebResponse)RequestObj.GetResponse();
            myResponseStream = response.GetResponseStream();
            if (response.ContentEncoding.ToUpper().Equals("GZIP"))//以下为请求到的数据压缩方式为gzip是的解压方法
            {
                 
                if (response.StatusCode ==HttpStatusCode.Redirect)
                {
                    Result = response.GetResponseHeader("Location");
                }
                else
                {
                     myResponseStream = ZipHelper.BytesToStream(
                        ZipHelper.Decompress(
                            ZipHelper.StreamToBytes(
                                response.GetResponseStream()

                            )
                        )
                    );
                }
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Redirect)
                {
                    Result = response.GetResponseHeader("Location");
                }
                else
                {
                    myResponseStream = response.GetResponseStream();
                } 
            }
        }
        catch (Exception ex)
        {
            // return GetRequstData(Url,Prama,ref Cookie);
            Console.WriteLine(ex.ToString());
            return "";
        }
        Cookie.Add(response.Cookies);
        StreamReader TempReder = new StreamReader(myResponseStream, System.Text.Encoding.GetEncoding("utf-8"));
        Result = TempReder.ReadToEnd();
        TempReder.Close();
        myResponseStream.Close();
        return Result;

    }
}