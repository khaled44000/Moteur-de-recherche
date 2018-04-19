using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using StackExchange.Redis;
using System.Web.Configuration;

/// <summary>
/// Description résumée de Utils
/// </summary>
public class Utils
{
   
    public string Url { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public Utils(string url)
    {
        Url = url;
    }

    /// <summary>
    /// Get meta data from url and save data in redis
    /// </summary>
    /// <param name="strUrl">url</param>
    public static void saveMetaDataFromUrl(string strUrl)
    {
        try
        {
            Utils metaInfo = new Utils(strUrl);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(strUrl));
            request.Method = WebRequestMethods.Http.Get;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            String responseString = reader.ReadToEnd();

            response.Close();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(responseString);
            metaInfo.Url = strUrl;
            String title = (from x in doc.DocumentNode.Descendants()
                            where x.Name.ToLower() == "title"
                            select x.InnerText).FirstOrDefault();
            if (title == null || title == string.Empty)
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
            Redis.SaveMetaDataFromUrl(metaInfo);
        }
        catch
        {
            // error serveur 
        }

    }

    /// <summary>
    /// save url in redis queue
    /// </summary>
    /// <param name="strUrl"></param>
    /// <param name="lstUrl"></param>
    public static void SaveUrlQueue(string strUrl , List<string>  lstUrl)
    {
        int NbreMax = int.Parse(WebConfigurationManager.AppSettings["NbreMax"]);
        if (lstUrl.Count >= NbreMax)
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
                    if (i == NbreMax)
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
                               
                                docFils = hw.Load(strLink);  // vérifier l'url si connecté
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

}