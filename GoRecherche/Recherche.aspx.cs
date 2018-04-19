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
    public  List<string> lstUrl = new List<string>();
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Session !=null)
        {
            if(Session["isLogin"] !=null)
            {
                if((Boolean) Session["isLogin"])
                {
                    tblRedsis.Visible = false;
                    pnlDataRedis.Visible = true;
                }
                else
                {
                    tblRedsis.Visible = false;
                    pnlDataRedis.Visible = true;
                }
            }
        }

    }

    protected void OnClick_GoRecherche(object sender, EventArgs e)
    {
        lblResulat.Text = string.Empty;
        if (!string.IsNullOrEmpty(txtGoRecherche.Text.Trim()))
        {
            GetMetaInfo(txtGoRecherche.Text);
        }
    }

    protected void OnClick_GoRecherche2(object sender, EventArgs e)
    {
        lblResulat.Text = string.Empty;
        if (!string.IsNullOrEmpty(txtGoRecherche.Text.Trim()))
        {
            GetMetaInfo(txtGoRecherche2.Text);
        }
    }


    private void GetMetaInfo(string strUrl)
    {
        ConnectionMultiplexer redisConn = Redis.connectionRedis();

        IServer server = redisConn.GetServer("localhost", 6379);
        server.FlushDatabase();

        List<object> lstObjetParent = new List<object>();
        Utils.SaveUrlQueue(strUrl , new List<string>());
        Redis.getUrlQueue();
        Redis.GetMetaDataFromUrl();




       List<string> lst =  Redis.getMetaData();
        foreach (var item in lst)
        {
            lblResulat.Text += item  + "<br />";
            lblResulat.Text += "<hr />";
        }

       /* int intIndex = 0;
        foreach (string URL_Parent in lstURL_Parents)
        {
            intIndex++;
            Utils metaInfo = Utils.GetMetaDataFromUrl(URL_Parent , intIndex);
            lstObjetParent.Add(metaInfo);
        }
        gvListUrlParents.DataSource = lstObjetParent;
        gvListUrlParents.DataBind();
        List<Tuple<string, string>> lstRedis = Redis.SaveBigData(gvListUrlParents);
        Affichage(lstRedis);*/
    }


    protected void OnRowDataBound_gvListUrlParents(object sender, GridViewRowEventArgs e)
    {
       /* if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string strUrlParent = gvListUrlParents.DataKeys[e.Row.RowIndex].Value.ToString();

            GridView gvListUrlFils = e.Row.FindControl("gvListUrlFils") as GridView;

            List<object> lstObjetFils = new List<object>();
            List<string> lstURL_Fils = Utils.getListUrl(strUrlParent);
            int intIndex = 0;
            foreach (string URL_Fils in lstURL_Fils)
            {
                intIndex++;
                Utils metaInfo = Utils.GetMetaDataFromUrl(URL_Fils, intIndex);
                lstObjetFils.Add(metaInfo);
            }
            gvListUrlFils.DataSource = lstObjetFils;
            gvListUrlFils.DataBind();
          

        }*/
    }


    protected void OnClick_Connexion(object sender, EventArgs e)
    {
        string strLogin = txtLogin.Text.Trim();
        string strPassword = txtPassword.Text.Trim();
        if (!string.IsNullOrEmpty(strLogin) && !string.IsNullOrEmpty(strPassword))
        {
            if(strLogin.Equals(_Login) && strPassword.Equals(_Password))
            {
                Session["isLogin"] = true;
                tblRedsis.Visible = false;
                pnlDataRedis.Visible = true;
            }
        }
    }


    private void Affichage(List<Tuple<string, string>> lstRedis)
    {
        lblRedis.Text = string.Empty;
        foreach (Tuple<string, string> redis in lstRedis)
        {
            lblRedis.Text += redis.Item1 + "  " + redis.Item2 + "<br />";
            lblRedis.Text += "<hr />";
        }
    }

}