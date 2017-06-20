using System;
using System.Linq;
using System.Net;
using System.Threading;
using java.security.cert;
using System.Collections;
using System.Web;
using System.Windows.Forms;
using System.Collections.Generic;

 
public partial class GetXml : System.Web.UI.Page
{
  
    private delegate string GetHtml(object Url);
    public string DoneState;
    public static int TheardCount = 0;

    public static  int GcServerEnable  = 0  ;
    public delegate void GcFunctionDelegate();
    public void onComplete(IAsyncResult asyncResult) { GcServerEnable = 1; }
    protected void Page_Load(object sender, EventArgs e)
    {
        
        GcFunctionDelegate GcFunction = delegate()
        {
            Gc();
        };
         object   FuckObject  = new object();
         if (GcServerEnable==0)
         {
                  GcFunction.BeginInvoke(onComplete, FuckObject); 
         }
        if (Request["vUrl"] != null)
        {
            string UrlForLog = Request["vUrl"].ToString();
            try
            {

                string Url = Request["vUrl"].ToString();

                string context="";
                string result = "";

                if (Request["DataType"] == "jsonp")
                {
                    DateTime StartTime = DateTime.Now;
                    if (Request["vUrl"].ToString().IndexOf("qq.com") > -1)
                    {
                        context = GetTencentDataEnableJs(Url);
                    }
                    else if (Request["vUrl"].ToString().IndexOf("youku.com")>-1)
                    {
                       // context = GetTencentDataEnableJs(Url);
                           string  YouKUId =""; 
                        if (Url.IndexOf("id_")<0)
	                  {
                       
	                	context  = "";return; 
                	}else
                        {   
                           
                             string ep = "",oip="",sid="",token="",type="",vid="";
                            string DownLoadUrl =  "http://pl.youku.com/playlist/m3u8?ctype=12&ep={0}&ev=1&keyframe=1&oip={1}&sid={2}&token={3}&type={4}&vid={5}";
                            int StratIndex = Url.IndexOf("id_") + "id_".Length - 1;
                            string UrlTemp = Url.Substring(StratIndex, Url.Length - StratIndex );
                            int EndIndex = UrlTemp.IndexOf("/") > 0 ? UrlTemp.IndexOf("/") : UrlTemp.IndexOf(".");
                            YouKUId = Url.Substring(StratIndex+1, EndIndex-1);
                            context = GetTuDouData("http://play.youku.com/play/get.json?vid=" + YouKUId + "&ct=12");
                            string Ep = FormatingTuDouDate(context, ref result, "\"encrypt_string\"").Replace("}", "").Replace("]", "").Replace("\"","").Split(',')[0];
                          //  Ep = EncodeStuff.Rc4(Ep,EncodeStuff.Decode64(Ep),true);
                             result = "";//同上
                             oip = FormatingTuDouDate(context, ref result, "\"ip\"").Replace("}", "").Replace("]", "").Split(',')[0];
                        
                             result = "";//同上
                             getEp(YouKUId,Ep,ref ep, ref token, ref sid);
                             ep = HttpUtility.UrlEncode(ep);
                             DownLoadUrl = "http://pl-ali.youku.com/playlist/m3u8?vid=" + YouKUId + "&type=mp4&ts=1481090715&keyframe=0&ep=" + ep
                                 + "&sid=" + sid
                                 + "&token=" + token
                                 + "&ctype=12&ev=1&oip=" + oip; 
                             //DownLoadUrl = "http://pl.youku.com/playlist/m3u8?ctype=12&ep="+ep+"&ev=1&keyframe=1&oip="+oip+"&sid="+sid+"&token="+token+"&type="+type+"&vid="+YouKUId;
                             context = GetTuDouData(DownLoadUrl);


                         http://pl-ali.youku.com/playlist/m3u8?vid=XMTg1MTY4ODI4MA==&type=mp4&ts=1481090715&keyframe=0&ep=dyacG0mEVskG5irYjz8bY3m0cSJaXJZ3kkiE%2FLYLBMRQMezC6DPcqJ%2B1TfY%3D&sid=5481090763977124ef5ee&token=2539&ctype=12&ev=1&oip=242794994 
                             result = "";//同上
                            
	                 }


                       
                    }
                    else
                    {
                     
                        context = GetTuDouDataEnableJs(Url); 
                    }
                    DateTime EndTime = DateTime.Now;  
                    double Ts = (EndTime - StartTime).TotalSeconds;
                  //  SqlStuff.RunTimeLog("RunTimeLog", Request.UserHostAddress, Url,Request.UserAgent+"请求用时 "+Ts.ToString()+",请求类型Jsonp");
                    Response.ContentType = "text";
                    Response.Clear();
                    Response.Write("VdoCallBack('"+@context.ToString().Replace("\r\n", "\\r\\n")+"');"); 
                }
                else
                {
                    DateTime StartTime = DateTime.Now;  
                    context = TuDouVdoStuff(Url);
                    DateTime EndTime = DateTime.Now; 
                    Response.ContentType = "text/xml";
                    double Ts = (EndTime - StartTime).TotalSeconds;
                    SqlStuff.RunTimeLog("RunTimeLog", Request.UserHostAddress, Url, Request.UserAgent + "请求用时 " + Ts.ToString() + ",请求类型Xml"); 
                    Response.Clear();
                    Response.Write(@context.ToString());
                    Response.End();
                }
        }
            catch (Exception ex)
        {
          // throw;
            if (!(ex is ThreadAbortException))
            {
                SqlStuff.RunTimeLog("ErrorLog", Request.UserHostAddress, UrlForLog, Request.UserAgent + "试图请求但" + ex.Message);
                Response.Write("参数不正确");
            }
        }

    }
        else
        {
            SqlStuff.RunTimeLog("ErrorLog", Request.UserHostAddress,"来自"+Request.UrlReferrer.ToString(), Request.UserAgent+"试图访问但提供的参数不足");  
            Response.Clear();
            Response.Write("禁止访问");
        }
    }


    private static string myEncoder(string a, byte[] c, bool isToBase64)
    {
        string result = "";
        
        List<Byte> bytesR = new List<byte>();
        int f = 0, h = 0, q = 0;
        int[] b = new int[256];
        for (int i = 0; i < 256; i++)
            b[i] = i;
        while (h < 256)
        {
            f = (f + b[h] + a[h % a.Length]) % 256;
            int temp = b[h];
            b[h] = b[f];
            b[f] = temp;
            h++;
        }
        f = 0; h = 0; q = 0;
        while (q < c.Length)
        {
            h = (h + 1) % 256;
            f = (f + b[h]) % 256;
            int temp = b[h];
            b[h] = b[f];
            b[f] = temp;
            byte[] bytes = new byte[] { (byte)(c[q] ^ b[(b[h] + b[f]) % 256]) };
            bytesR.Add(bytes[0]);
            result += System.Text.ASCIIEncoding.ASCII.GetString(bytes);
            q++;
        }
        if (isToBase64)
        {
            Byte[] byteR = bytesR.ToArray();
            result = Convert.ToBase64String(byteR);
        }
        return result;
    }


    public static void getEp(string vid, string ep, ref string epNew, ref string token, ref string sid)
    {
        string template1 = "becaf9be";
        string template2 = "bf7e5f01";
        byte[] bytes = Convert.FromBase64String(ep);
        ep = System.Text.ASCIIEncoding.ASCII.GetString(bytes);
        string temp = myEncoder(template1, bytes, false);
        string[] part = temp.Split('_');
        sid = part[0];
        token = part[1];
        string whole = string.Format("{0}_{1}_{2}", sid, vid, token);
        byte[] newbytes = System.Text.ASCIIEncoding.ASCII.GetBytes(whole);
        epNew = myEncoder(template2, newbytes, true);
    }


    public void OnDone(IAsyncResult  asyncResult) 
    {
        DoneState  = "1"; 
    }
    /// <summary>
    /// 获取土豆网视频的主程序
    /// </summary>
    /// <param name="Url">传入的土豆网地址</param>
    /// <returns></returns>
    public  string   TuDouVdoStuff(string Url){
        string HD = "";
        if (Request["HD"] != null)//获取传入的hd参数来过去指定清晰度的视频地址
        {
            HD = Request["HD"].ToString();
        }
        string result = "";
        Response.Clear();
        string Context = GetTuDouData(Url);//获取响应报文中的配置参数集合 格式为json集合
        string[] BaseUrl = FormatingTuDouDate(Context, ref result, "\"baseUrl\"").Replace("}", "").Replace("]", "").Split(',');//获取报文中JASON集合中的Baseurl参数的集合
        result = ""; //主要上面方法使用的递归 这个变量要清空一下
        string[] KArray = FormatingTuDouDate(Context, ref result, "\"k\"").Replace("}", "").Replace("]", "").Split(',');//同上
        result = "";//同上
        string[] SizeArray = FormatingTuDouDate(Context, ref result, "\"size\"").Replace("}", "").Replace("]", "").Replace("'", "").Split(',');//同上
        result = "";//同上
        string[] SecondsArray = FormatingTuDouDate(Context, ref result, "\"seconds\"").Replace("}", "").Replace("]", "").Replace("'", "").Split(',');//同上
        result = "";//同上
        string[] NoArray = FormatingTuDouDate(Context, ref result, "\"no\"").Replace("}", "").Replace("]", "").Replace("'", "").Split(',');//同上
        result = "";//同上
        string[] PtArray = FormatingTuDouDate(Context, ref result, "\"pt\"").Replace("}", "").Replace("]", "").Replace("'", "").Split(',');//同上
        
        for (int i = 0; i < SecondsArray.Length; i++)//由于土豆中配置的每段视频的长度是毫秒 而ckplayer中需要的是秒 这里有一个转换 四舍五入法
        {
            SecondsArray[i] = ToInt(SecondsArray[i]).ToString();
        }
        
        string[][] ArrayStuff = { BaseUrl, SecondsArray, NoArray, PtArray, KArray, SizeArray }; //构造二维数组
        
        //TuDouVdo[] IDonTCare = GernerateTuDouVdoList(ArrayStuff);
        result = GernerateXml(GernerateTuDouVdoList(ArrayStuff));//生成xml  
        return result;
        //Response.Close();
    }
    /// <summary>
    ///将二维数组转换为TuDouVdo对象集合
    /// </summary>
    /// <param name="VdoVar"></param>
    /// <returns>TuDouVdo集合</returns>
    public TuDouVdo[] GernerateTuDouVdoList(string [][] VdoVar)
    {
        TuDouVdo[] Result = new TuDouVdo[VdoVar[0].Length];
        for (int i = 0; i < VdoVar[0].Length; i++)
        {
            Result[i] = new TuDouVdo(VdoVar[0][i].ToString(), VdoVar[1][i].ToString(), VdoVar[2][i].ToString(), VdoVar[3][i].ToString(),VdoVar[4][i].ToString(), VdoVar[5][i].ToString());
        }
        return Result ; 
    }

    /// <summary>
    /// 生成xml
    /// </summary>
    /// <param name="ArrayStuff">二维数组</param>
    /// <returns>xml格式文本</returns>
    public string GernerateXml(string[][] ArrayStuff)
    {
        string result = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ckplayer>";
        for (int i = 0; i < ArrayStuff[0].Count(); i++)
        {
            result += "<video>";
            result += "<file><![CDATA[http://vr.tudou.com/v2proxy/v?sid=11000&id=" + ArrayStuff[0][i].ToString() + "&st=2]]></file>";//地址段
            result += " <size>" + ArrayStuff[1][i].ToString() + "</size>";//视频大小 单位是字节
            result += " <seconds>" + ArrayStuff[2][i].ToString() + "</seconds>";//视频长度 /秒
            result += "</video>";
        }
        result += "</ckplayer>";
        return result;
    }
    public static void Gc(){
        GcServerEnable = 1;
        Thread.Sleep(1000*60);//10 * 60 *
        GC.Collect();
        Gc();
    }
    /// <summary>
    /// 生成xml
    /// </summary>
    /// <param name="ArrayStuff">tudouvdo对象集合</param>
    /// <param name="pt"></param>
    /// <returns> xml格式文本</returns>
    public string GernerateXml(TuDouVdo[] ArrayStuff, string pt = "")
    {
        if (pt=="")
        {
            pt = ArrayStuff[0].Pt;
        }
        string result= "<?xml version=\"1.0\" encoding=\"utf-8\"?><ckplayer>";
        for (int i = 0; i < ArrayStuff.Count(); i++)
        {
            if (ArrayStuff[i].Pt==pt)
            {
                result += "<video>";
                result += "<file><![CDATA[http://vr.tudou.com/v2proxy/v?sid=11000&id=" + ArrayStuff[i].K.ToString() + "&st=2]]></file>";
                result += " <size>" + ArrayStuff[i].Size.ToString() + "</size>";
                result += " <seconds>" + ArrayStuff[i].Seconds.ToString() + "</seconds>";
                result += "</video>";
            }
        }
        result += "</ckplayer>";
        return result;
    }
    /// <summary>
    /// 将指定的参数从报文中提取出来（递归）
    /// </summary>
    /// <param name="date">请求报文</param>
    /// <param name="result">递归遗传变量</param>
    /// <param name="KeyName">参数名</param>
    /// <returns></returns>
    public string FormatingTuDouDate(string date, ref string result, string KeyName = "\"baseUrl\"")
    {

        string key = KeyName;
        if (date.IndexOf(key)>-1)//递归出口
        {
            int StratIndex = date.IndexOf(key) + key.Length;//获得起始下标
            int EndIndex;
            if (date.IndexOf(",", StratIndex) > -1)//以逗号结尾的情况
            {
                EndIndex = date.IndexOf(",", StratIndex);//获得结束下标
            }
            else if (date.IndexOf("}") > -1)//以花括弧结尾的情况
            {
                EndIndex = date.IndexOf(",", StratIndex);//获得结束下标
            }
            else { EndIndex = -1; }
            if (StratIndex>=0&&EndIndex>=0)//有效性验证
            {
                int Lenght = EndIndex - StratIndex;//计算长度
                result = result!=""?result + ","+ date.Substring(StratIndex +1, Lenght - 1): date.Substring(StratIndex + 1, Lenght - 1);//三元运算符 截取出来的数据压入Result变量中
                date = date.Substring(EndIndex);
                if (date.IndexOf(KeyName) > -1)
                {
                    FormatingTuDouDate(date,ref result,KeyName); //递归，遗传截取到的数据到下一层 
                }
            }
        }
        return result.Replace("\r","").Replace("\n","").Replace("\t","");//去除换行符
    }
    /// <summary>
    /// 将整数转换为指定长度的整数四舍五入
    /// </summary>
    /// <param name="data">需要转换数据</param>
    /// <param name="legth">指定的长度</param>
    /// <returns></returns>
    public  static double  ToInt(string data)
    {
        return   Convert.ToInt32((Convert.ToDouble(data)/1000));
    }

    /// <summary>
    /// 将sessionid从请求到的报文中的js代码中抽离出来
    /// </summary>
    /// <returns> </returns>
    public static string GetParamFormJs(string Context, string SplitToken = ",", string ParamKey = "\"k\"")
    {

        string Result = "";
        string temp = Context;
        if (string.IsNullOrEmpty(temp))
        {
            return "";
        }
        if (temp.IndexOf(ParamKey) > -1)
        {
            int startIndex = temp.IndexOf(ParamKey) + ParamKey.Length + 1;

            if (temp.IndexOf("\"", startIndex) > 1)
            {
                int EndIndex = temp.IndexOf("\"", temp.IndexOf("\"", startIndex) + 1);
                Result = temp.Substring(startIndex + 1, EndIndex - startIndex - 1);
            }
        }

        return Result;
    }
     

    /// <summary>
    /// 程序入口
    /// </summary>
    /// <param name="Url"></param>
    /// <returns></returns>
    public string GetTuDouData(string Url)
    {
        string result= "";
      
        HttpWebRequest INRequest = (HttpWebRequest)System.Net.WebRequest.Create(Url);
        string Accept="";
        foreach (string  item in Context.Request.AcceptTypes)
        {
            Accept = Accept == "" ? item : Accept + ',' + item;//构造请求头部
        }
        INRequest.Accept = Accept;
        INRequest.UserAgent = Context.Request.UserAgent;
        INRequest.Referer = Url;
        INRequest.Headers.Add("Accept-Encoding", Context.Request.Headers.Get(""));
        INRequest.Headers.Add("Accept-Language", Context.Request.Headers.Get("Accept-Encoding"));
     //   INRequest.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
    //    INRequest.Referer = Context.Request.UrlReferrer.ToString();
        result = HttpHelper.GetRequstData(INRequest);
        return result;         
    }

    public string GetTuDouDataEnableJs(string Url)
    {

        string result = "";

        HttpWebRequest INRequest = (HttpWebRequest)System.Net.WebRequest.Create(Url);
        string Accept = "";
        foreach (string item in Context.Request.AcceptTypes)
        {
            Accept = Accept == "" ? item : Accept + ',' + item;//构造请求头部
        }
        INRequest.Accept = Accept;
        INRequest.UserAgent = Context.Request.UserAgent;
        INRequest.Referer = Url;
        INRequest.Headers.Add("Accept-Encoding", Context.Request.Headers.Get(""));
        INRequest.Headers.Add("Accept-Language", Context.Request.Headers.Get("Accept-Encoding"));

        Url = HttpHelper.GetResponseUrl(INRequest);
            

          Accept = ""; 
        foreach (string item in Context.Request.AcceptTypes)
        {
            Accept = Accept == "" ? item : Accept + ',' + item;//构造请求头部
        } 
        string Result = "";
        NHtmlUnit.BrowserVersion Nb = null;
        string UserAgent = Context.Request.UserAgent;
        if (UserAgent.IndexOf("Firefox") > -1)
        {
            Nb = NHtmlUnit.BrowserVersion.FIREFOX_17;
        }
        else if (UserAgent.IndexOf("Trident")>-1)
        {
            if (UserAgent.IndexOf("MSIE 7.0") > -1)
            {
                Nb = NHtmlUnit.BrowserVersion.INTERNET_EXPLORER_8;
            }else
            if (UserAgent.IndexOf("MSIE 8.0")>-1)
            {
                Nb = NHtmlUnit.BrowserVersion.INTERNET_EXPLORER_8;
            }
            else if (UserAgent.IndexOf("MSIE 9.0") > -1)
            {
                Nb = NHtmlUnit.BrowserVersion.INTERNET_EXPLORER_9;
            }
            else if (UserAgent.IndexOf("Trident/7.0") > -1)
            {
                Nb = NHtmlUnit.BrowserVersion.CHROME;
            }
            else
            {
                Nb = NHtmlUnit.BrowserVersion.FIREFOX_17;
            }
        }
        else if (UserAgent.IndexOf(" Chrome")>-1&&UserAgent.IndexOf("KHTML")>-1)
        {
             Nb = NHtmlUnit.BrowserVersion.FIREFOX_17; 
        }
        else if (UserAgent.IndexOf(" Chrome") > -1)
        {
            Nb = NHtmlUnit.BrowserVersion.CHROME;
        }
        else if (UserAgent.IndexOf("Mobile") > -1) 
           {
            Nb = NHtmlUnit.BrowserVersion.CHROME;
    
	    }
        else { Nb = NHtmlUnit.BrowserVersion.FIREFOX_17; }
          NHtmlUnit.WebClient WbClinet = new NHtmlUnit.WebClient(Nb);
          if (UserAgent.IndexOf("Mobile") > -1) 
           {
          WbClinet.Options.ThrowExceptionOnScriptError = false;  
	    } 
           
        // System.Windows.Forms.WebBrowser wb = new WebBrowser();
         Uri u = new Uri(Url);
         //wb.Url = u;
         WbClinet.Options.ThrowExceptionOnScriptError = false;
     //    wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);
          //HtmlDocument htmldoc =  wb.DocumentText;
      //  Result =   wb.DocumentText; 
          //Result = htmldoc.GetElementById("tudouHomePlayer").OuterHtml;
        WbClinet.Options.RedirectEnabled = false;
         WbClinet.Options.JavaScriptEnabled = true;
         WbClinet.Options.Timeout = 10000;
         WbClinet.AddRequestHeader("Accept", Accept);
         WbClinet.Options.CssEnabled = false;
         WbClinet.AddRequestHeader("User-Agent", Context.Request.UserAgent);
    //    WbClinet.
       // WbClinet.AddRequestHeader("Accept-Encoding", "gzip, deflate, sdch");
         WbClinet.JavaScriptTimeout = 1000;
       // NHtmlUnit.WebResponse   Ws =    WbClinet.LoadWebResponse(); 
         
        
       NHtmlUnit.Html.HtmlPage  htmldoc = WbClinet.GetHtmlPage(Url);
           if (UserAgent.IndexOf("Mobile") > -1) 
	{
              Result = "{Error:\"Not Support Phone\"}";  
               return Result ;
	}
        Result = htmldoc.AsText();
        Result = htmldoc.AsXml();
          Result = htmldoc.GetElementById("CustomPlayer").AsXml();
         WbClinet.CloseAllWindows();
         WbClinet = null;
         return Result;
    }




    public string GetTencentDataEnableJs(string Url)
    {
        string Accept = "";

        foreach (string item in Context.Request.AcceptTypes)
        {
            Accept = Accept == "" ? item : Accept + ',' + item;//构造请求头部
        }
        string Result = "";
        NHtmlUnit.BrowserVersion Nb = null;
        string UserAgent = Context.Request.UserAgent;
        if (UserAgent.IndexOf("Firefox") > -1)
        {
            Nb = NHtmlUnit.BrowserVersion.FIREFOX_17;
        }
        else if (UserAgent.IndexOf("Trident") > -1)
        {
            if (UserAgent.IndexOf("MSIE 7.0") > -1)
            {
                Nb = NHtmlUnit.BrowserVersion.INTERNET_EXPLORER_8;
            }
            else
                if (UserAgent.IndexOf("MSIE 8.0") > -1)
                {
                    Nb = NHtmlUnit.BrowserVersion.INTERNET_EXPLORER_8;
                }
                else if (UserAgent.IndexOf("MSIE 9.0") > -1)
                {
                    Nb = NHtmlUnit.BrowserVersion.INTERNET_EXPLORER_9;
                }
                else if (UserAgent.IndexOf("Trident/7.0") > -1)
                {
                    Nb = NHtmlUnit.BrowserVersion.CHROME;
                }
                else
                {
                    Nb = NHtmlUnit.BrowserVersion.FIREFOX_17;
                }
        }
        else if (UserAgent.IndexOf(" Chrome") > -1 && UserAgent.IndexOf("KHTML") > -1)
        {
            Nb = NHtmlUnit.BrowserVersion.FIREFOX_17;
        }
        else if (UserAgent.IndexOf(" Chrome") > -1)
        {
            Nb = NHtmlUnit.BrowserVersion.CHROME;
        }
        else { Nb = NHtmlUnit.BrowserVersion.FIREFOX_17; }
        NHtmlUnit.WebClient WbClinet = new NHtmlUnit.WebClient(Nb);
        // System.Windows.Forms.WebBrowser wb = new WebBrowser();
        Uri u = new Uri(Url);
        //wb.Url = u;
        // wb.ScriptErrorsSuppressed = false;
        //    wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);
        //HtmlDocument htmldoc =  wb.DocumentText;
        //  Result =   wb.DocumentText; 
        //Result = htmldoc.GetElementById("tudouHomePlayer").OuterHtml;
        try
        {
            WbClinet.Options.RedirectEnabled = false;
            WbClinet.Options.JavaScriptEnabled = true;
            WbClinet.Options.Timeout = 20000;
            WbClinet.AddRequestHeader("Accept", Accept);
            WbClinet.Options.CssEnabled = false;
            WbClinet.AddRequestHeader("User-Agent", Context.Request.UserAgent);
            WbClinet.Options.UseInsecureSsl = true; 
           
            WbClinet.JavaScriptTimeout = 10000;
            NHtmlUnit.Html.HtmlPage htmldoc = WbClinet.GetHtmlPage(Url);

            Result = htmldoc.GetElementByClassName("txp_player").AsXml();
            WbClinet.CloseAllWindows();
            WbClinet = null;
        }
        catch (Exception ex) { throw; }
        
        return Result;
    } 



    void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
    {
        DoneState = "1";
        throw new NotImplementedException();
    }




    private static string htmlstr;  
   private static void GetHtmlWithBrowser(object url)  
      {  
           htmlstr = string.Empty;  
         WebBrowser wb = new WebBrowser();  
          wb.AllowNavigation = true;  
          wb.Url = new Uri(url.ToString());  
          DateTime dtime = DateTime.Now;  
         double timespan = 0;
         while (timespan<100&&wb.ReadyState != WebBrowserReadyState.Complete)  
       {

           System.Windows.Forms.Application.DoEvents();
              DateTime time2 = DateTime.Now;  
            timespan = (time2 - dtime).TotalSeconds;  
        }  
 
        if (wb.ReadyState == WebBrowserReadyState.Complete)  
          {  
               HtmlDocument HtmlDoc  = wb.Document;
               htmlstr = HtmlDoc.GetElementById("tudouHomePlayer").OuterHtml;
            }  
    }  

      /// <summary>  
        /// 在单线程中启用浏览器  
        /// </summary>  
       public static void RunWithSingleThread(object url,ref string html)  
       {  
          ParameterizedThreadStart ps = new ParameterizedThreadStart(GetHtmlWithBrowser);
          TheardCount++;
           Thread t = new Thread(ps);  
            t.IsBackground = true;  
            t.ApartmentState = ApartmentState.STA;  
            t.Start(url);  
           html = htmlstr;  
       }  

}