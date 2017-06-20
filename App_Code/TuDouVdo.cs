using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// TuDouBaseUrl 的摘要说明
/// </summary>
public class TuDouVdo
{
    public  string BaseUrl;
    public string Seconds;
    public string No;
    public string Pt;
    public string K;
    public string Size;
    public TuDouVdo(string BaseUrl,string Seconds, string No, string Pt,string K,string Size)
    {
        this.BaseUrl = BaseUrl;
        this.Seconds = Seconds;
        this.No = No;
        this.Pt = Pt;
        this.K = K;
        this.Size = Size;
    }
}