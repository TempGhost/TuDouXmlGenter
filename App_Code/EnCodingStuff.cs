using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;
using System.Text;



public  static class EncodeStuff
{

    public static byte[] Decode64(string a)
  {
              if (string.IsNullOrEmpty(a))
             {
                 return null;
              }
             int f;
              int g;
              string h;
              List<byte> l = new List<byte>();
              int[] i =
              {
                 -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                  -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
                  52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                  11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1, -1, 26, 27, 28, 29,
                  30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1,
                -1, -1
              };
              for (g = a.Length, f = 0, h = ""; g > f;)
              {
                  int b;
                  do
                  {
                      b = i[255 & a[f++]];
              } while (g > f && -1 == b);
 
                  if (-1 == b) break;
 
                  int c;
                  do
                {
                     c = i[255 & a[f++]];
                 } while (g > f && -1 == c);
 
                  if (-1 == c) break;
 
                  byte[] bytes0 = { (byte)(b << 2 | (48 & c) >> 4) };
                  h += Encoding.ASCII.GetString(bytes0);
                  l.Add(bytes0[0]);
                  int d;
                 do
                  {
                      d = 255 & a[f++];
                    if (61 == d) return l.ToArray();
                     d = i[d];
                 } while (g > f && -1 == d);

                  if (-1 == d) break;
                byte[] bytes1 = { (byte)((15 & c) << 4 | (60 & d) >> 2) };
                 h += Encoding.ASCII.GetString(bytes1);
                  l.Add(bytes1[0]);
                  int e;
                  do
                  {
                      e = 255 & a[f++];
                      if (61 == e) return l.ToArray();
                      e = i[e];
                  } while (g > f && -1 == e);
 
                  if (-1 == e) break;
                  byte[] bytes2 = { (byte)((3 & d) << 6 | e) };
                 h += Encoding.ASCII.GetString(bytes2);
                  l.Add(bytes2[0]);
              }
             return l.ToArray();
  }

    public static string Rc4(string a, byte[] c, bool isToBase64)
  {
             // rc4加密算法
            int f = 0, h = 0, q;
           int[] b = new int[256];
             for (int i = 0; i < 256; i++)
             {
                  b[i] = i;
              }
            while (h < 256)
             {
                f = (f + b[h] + a[h % a.Length]) % 256;
                 int temp = b[h];
                 b[h] = b[f];
                b[f] = temp;
                 h++;
             }

           f = 0; h = 0; q = 0;
            string result = "";
             List<byte> bytesR = new List<byte>();
          while (q < c.Length)
            {
              h = (h + 1) % 256;
                 f = (f + b[h]) % 256;
                 int temp = b[h];
               b[h] = b[f];
                 b[f] = temp;
                 byte[] bytes = { (byte)(c[q] ^ b[(b[h] + b[f]) % 256]) };
                 bytesR.Add(bytes[0]);
                result += Encoding.ASCII.GetString(bytes);
                q++;
            }

             if (isToBase64)
             {
                 var byteR = bytesR.ToArray();
                 result = Convert.ToBase64String(byteR);
                 //result = Encode64(result);
            }

             return result;
 }
    }
