using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// Description résumée de Redis
/// </summary>
public class Redis
{
    public static ConnectionMultiplexer connectionRedis()
    {
        /* ("127.0.0.1") */
        ConnectionMultiplexer redisConn = ConnectionMultiplexer.Connect("localhost,allowAdmin=true");
        return redisConn;
    }

    public static void saveUrlQueue(string strUrl)
    {
        Guid keyGuidUrl = Guid.NewGuid();
        ConnectionMultiplexer redisConn = connectionRedis();
        //get your db
        IDatabase redDb = redisConn.GetDatabase();
        redDb.StringSet(keyGuidUrl.ToString(), strUrl);
    }

    public static void getUrlQueue()
    {
        List<string> lstUrlQueue = new List<string>();
        ConnectionMultiplexer redisConn = connectionRedis();
        //get your db
        IDatabase redDb = redisConn.GetDatabase();
        IServer server = redisConn.GetServer("localhost", 6379);
        foreach (var key in server.Keys().OrderBy(o => o.ToString()))
        {
            RedisKey k = key;
            string strKey = k.ToString();
            string strUrl = redDb.StringGet(k);
            redDb.KeyDelete(k);
            Utils.saveMetaDataFromUrl(strUrl);
        }
    }

    public static List<string> getMetaData()
    {
        List<string> lstUrlQueue = new List<string>();
        ConnectionMultiplexer redisConn = connectionRedis();
        //get your db
        IDatabase redDb = redisConn.GetDatabase();
        IServer server = redisConn.GetServer("localhost", 6379);
        foreach (var key in server.Keys().OrderBy(o => o.ToString()))
        {
            try
            {
                RedisKey k = key;
                string strKey = k.ToString();
                if (redDb.StringGet(k).ToString() != null)
                {
                    string strUrl = redDb.StringGet(k).ToString();
                    lstUrlQueue.Add(strUrl);
                }
            }
            catch
            { }

        }
        return lstUrlQueue;
    }

    public static List<Tuple<string, string>> SaveBigData(GridView gvListUrl)
    {
        bool isSave = false;
        List<Tuple<string, string>> lstRedis = new List<Tuple<string, string>>();

        /* ("127.0.0.1") */
        ConnectionMultiplexer redisConn = ConnectionMultiplexer.Connect("localhost,allowAdmin=true");

        IServer server = redisConn.GetServer("localhost", 6379);
        server.FlushDatabase();
        //get your db
        IDatabase redDb = redisConn.GetDatabase();


        foreach (GridViewRow row in gvListUrl.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 1; i < gvListUrl.Columns.Count; i++)
                {
                    String strIndex = "url" + row.Cells[1].Text;
                    String strIndexTitre = "Titre" + row.Cells[1].Text;
                    String strIndexDesc = "Desc" + row.Cells[1].Text;
                    String strUrl = row.Cells[2].Text + " Titre" + row.Cells[3].Text + " Desc" + row.Cells[4].Text;
                    redDb.StringSet(strIndex, strUrl);
                   /* string titre = row.Cells[3].Text;
                    string desc = row.Cells[4].Text;
                   
                   redDb.StringSet(strIndexTitre, titre);
                    redDb.StringSet(strIndexDesc, desc);*/



                }
            }
        }
     
        foreach (var key in server.Keys().OrderBy(o=> o.ToString()))
        {
           RedisKey k = key;
            string strKey = k.ToString();
            string value = redDb.StringGet(k);
            lstRedis.Add(new Tuple<string, string>(strKey, value));
        }

        return lstRedis;
    }

    public static void SaveMetaDataFromUrl(Utils metaInfo)
    {
        List<Tuple<string, string>> lstRedis = new List<Tuple<string, string>>();
        Guid keyGuidUrl = Guid.NewGuid();
        ConnectionMultiplexer redisConn = connectionRedis();
        //get your db
        IDatabase redDb = redisConn.GetDatabase();
       // redDb.StringSet(keyGuidUrl.ToString(), metaInfo);
        
        redDb.HashSet(keyGuidUrl.ToString(), new HashEntry[] { new HashEntry("Url", metaInfo.Url), new HashEntry("Title" , metaInfo.Title), new HashEntry("Description", metaInfo.Description) });
        
    }

    public static List<Tuple<string, string>> GetMetaDataFromUrl()
    {
        List<Tuple<string, string>> lstRedis = new List<Tuple<string, string>>();
        ConnectionMultiplexer redisConn = connectionRedis();
        //get your db
        IDatabase redDb = redisConn.GetDatabase();
        IServer server = redisConn.GetServer("localhost", 6379);
        foreach (var key in server.Keys().OrderBy(o => o.ToString()))
        {
            RedisKey k = key;
            string strKey = k.ToString();
            string strUrl = redDb.HashGet(strKey , "Url");
            string strTitle = redDb.HashGet(strKey , "Title");
            string strDescription = redDb.HashGet(strKey , "Description");

          //  string stUrl = infoUrl.Split('#')[0].ToString();
        }


        return lstRedis;
    }

}