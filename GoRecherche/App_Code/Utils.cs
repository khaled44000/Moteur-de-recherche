using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using StackExchange.Redis;

/// <summary>
/// Description résumée de Utils
/// </summary>
public class Utils
{

    public int Index { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
  //  public string Keywords { get; set; }
  //  public string ImageUrl { get; set; }
  //  public string SiteName { get; set; }

    public Utils(string url , int intIndex)
    {
        Index = intIndex;
        Url = url;
 
    }

    

    public static void saveMetaDataFromUrl(string strUrl)
    {
        try
        {
            
            Utils metaInfo = new Utils(strUrl, 0);
            string MetaDataFromUrl = string.Empty;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(strUrl));
            request.Method = WebRequestMethods.Http.Get;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            String responseString = reader.ReadToEnd();

            response.Close();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(responseString);
           metaInfo.Index = 0;
           metaInfo.Url = strUrl;
            String title = (from x in doc.DocumentNode.Descendants()
                            where x.Name.ToLower() == "title"
                            select x.InnerText).FirstOrDefault();
            if(title == null || title == string.Empty)
            {
                title = "Pas Titre";
            }
           metaInfo.Title = title;
            String desc = (from x in doc.DocumentNode.Descendants()
                           where x.Name.ToLower() == "meta"
                           && x.Attributes["name"] != null
                           && x.Attributes["name"].Value.ToLower() == "description"
                           select x.Attributes["content"].Value).FirstOrDefault();
            if (desc == null || desc == string.Empty)
            {
                desc = "Pas Description";
            }
            metaInfo.Description = desc;
            MetaDataFromUrl = strUrl + " # " + title + " # " + desc;
            Redis.SaveMetaDataFromUrl(metaInfo);
        }
        catch
        {
            // error serveur 
        }

    }

  

    public static void SaveUrlQueue(string strUrl , List<string>  lstUrl)
    {
        if (lstUrl.Count >= 3)
        {
            // arrêter 
        }
        else
        {
            if (!lstUrl.Contains(strUrl))
            {
                lstUrl.Add(strUrl);
                Redis.saveUrlQueue(strUrl);
            }

            HtmlWeb hw = new HtmlWeb();
            HtmlDocument docParent = hw.Load(strUrl);
            HtmlNodeCollection _HtmlNodeCollection = docParent.DocumentNode.SelectNodes("//a[@href]");
            if (_HtmlNodeCollection != null)
            {
                int i = 0;
                foreach (HtmlNode _HtmlNode in _HtmlNodeCollection)
                {
                    i++;
                    if (i == 3)
                    {
                        break;
                    }
                    else
                    {
                        String strLink = _HtmlNode.Attributes["href"].Value.ToString();
                        HtmlDocument docFils = new HtmlDocument();
                        String strUrlFinal = string.Empty;
                        if (strLink.StartsWith("https") || strLink.StartsWith("http"))
                        {
                            try
                            {
                                docFils = hw.Load(strLink);
                                strUrlFinal = strLink;
                                if (!lstUrl.Contains(strLink))
                                {
                                    lstUrl.Add(strLink);
                                    Redis.saveUrlQueue(strLink);
                                    SaveUrlQueue(strLink, lstUrl);
                                }
                            }
                            catch
                            {
                                // url non connecté
                            }
                        }
                        else if (strLink.StartsWith("//"))
                        {

                            try
                            {
                                // Vérifier Url
                                String strUrlhttps = "https:" + strLink;
                                docFils = hw.Load(strUrlhttps);
                                strUrlFinal = strUrlhttps;
                            }
                            catch
                            {
                                String strUrlhttp = "http:" + strLink;
                                docFils = hw.Load(strUrlhttp);
                                strUrlFinal = strUrlhttp;
                            }
                            if (!lstUrl.Contains(strUrlFinal))
                                lstUrl.Add(strUrlFinal);
                        }
                        else
                        {
                            try
                            {
                                string strUrlPage = lstUrl + "/" + strLink;
                                docFils = hw.Load(strUrlPage);
                                if (!lstUrl.Contains(strUrlPage))
                                {
                                    lstUrl.Add(strUrlPage);
                                    Redis.saveUrlQueue(strUrlPage);
                                    SaveUrlQueue(strLink, lstUrl);
                                }
                            }
                            catch
                            {
                                try
                                {
                                    string strUrlPage = lstUrl + "//" + strLink;
                                    docFils = hw.Load(strUrlPage);
                                    if (!lstUrl.Contains(strUrlPage))
                                    {
                                        lstUrl.Add(strUrlPage);
                                        Redis.saveUrlQueue(strUrlPage);
                                        SaveUrlQueue(strLink, lstUrl);
                                    }
                                }
                                catch
                                {
                                    // url non valide
                                }
                            }
                        }
                    }
                }
            }
        }
    }



    public static Utils GetMetaDataFromUrl(string strUrl, int intIndex)
    {

        Utils metaInfo = new Utils(strUrl, intIndex);
        try
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(strUrl));
            request.Method = WebRequestMethods.Http.Get;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            String responseString = reader.ReadToEnd();

            response.Close();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(responseString);
            metaInfo.Index = intIndex;
            String title = (from x in doc.DocumentNode.Descendants()
                            where x.Name.ToLower() == "title"
                            select x.InnerText).FirstOrDefault();
            metaInfo.Title = title;
            String desc = (from x in doc.DocumentNode.Descendants()
                           where x.Name.ToLower() == "meta"
                           && x.Attributes["name"] != null
                           && x.Attributes["name"].Value.ToLower() == "description"
                           select x.Attributes["content"].Value).FirstOrDefault();
            metaInfo.Description = desc;
        }
        catch
        {
            // error serveur 
        }
        return metaInfo;
    }
}