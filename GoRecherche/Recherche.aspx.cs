using HtmlAgilityPack;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : Page
{
    public string _Login = WebConfigurationManager.AppSettings["Login"];
    public string _Password = WebConfigurationManager.AppSettings["Password"];
    public List<string> lstUrl = new List<string>();
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// Recherche google
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClick_GoRecherche(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtGoRecherche.Text.Trim()))
        {
            GetMetaInfo(txtGoRecherche.Text);
        }
    }

    /// <summary>
    /// Recherche wikipedia
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClick_GoRecherche2(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtGoRecherche.Text.Trim()))
        {
            GetMetaInfo(txtGoRecherche2.Text);
        }
    }


    /// <summary>
    /// Gestion des URLS
    /// </summary>
    /// <param name="strUrl"></param>
    private void GetMetaInfo(string strUrl)
    {
        ConnectionMultiplexer redisConn = Redis.connectionRedis();
        IServer server = redisConn.GetServer("localhost", 6379);
        server.FlushDatabase();

        // sauvegarder les url dans queue redis
        Utils.SaveUrlQueue(strUrl, new List<string>());
        // récuperer les url qui sont été stockées dans queue redis
        Redis.getUrlQueue();
        // afficher les données apres la sauvegrde des meta informations url dans redis
        List<object> lstObjetRedis = Redis.GetMetaDataFromUrl();

        gvListUrlRedis.DataSource = lstObjetRedis;
        gvListUrlRedis.DataBind();
    }

}