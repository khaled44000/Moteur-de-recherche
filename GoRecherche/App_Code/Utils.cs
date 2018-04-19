using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Configuration;

/// <summary>
/// public static void saveMetaDataFromUrl(string strUrl)
/// public static void SaveUrlQueue(List<string> lstUrlGlobal)
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
            // load html document from url
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
            // sauvegarde meta info url dans redis
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
    /// <param name="lstUrlGlobal"></param>
    public static void SaveUrlQueue(List<string> lstUrlGlobal)
    {
        int NbreMax = int.Parse(WebConfigurationManager.AppSettings["NbreMax"]);
        List<string> lstUrl = new List<string>();
        if (lstUrlGlobal.Count >= NbreMax)
        {
            // arrêter 
        }
        else
        {
            foreach (string strUrl in lstUrlGlobal)
            {
                if (!lstUrl.Contains(strUrl))
                {
                    lstUrl.Add(strUrl);
                    Redis.saveUrlQueue(strUrl);
                }

                HtmlWeb hw = new HtmlWeb();
                // get html documents from page web
                HtmlDocument docParent = hw.Load(strUrl);
                // get list link 
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
            SaveUrlQueue(lstUrl);
        }
    }

}