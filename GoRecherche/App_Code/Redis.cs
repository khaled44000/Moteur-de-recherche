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
    public int Index { get; set; }
    public string strKeyUrl { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    /// <summary>
    /// Connexion redis
    /// </summary>
    /// <returns></returns>
    public static ConnectionMultiplexer connectionRedis()
    {
        /* ("127.0.0.1") */
        ConnectionMultiplexer redisConn = ConnectionMultiplexer.Connect("localhost,allowAdmin=true, abortConnect = false");
        return redisConn;
    }

    /// <summary>
    /// Sauvegarder les urls dans queue redis
    /// </summary>
    /// <param name="strUrl"></param>
    public static void saveUrlQueue(string strUrl)
    {
        // Générer key Guid
        Guid keyGuidUrl = Guid.NewGuid();
        // connexion redis
        ConnectionMultiplexer redisConn = connectionRedis();

        //get  db
        IDatabase redDb = redisConn.GetDatabase();

        // sauvegarder url en redis avec StringSet
        redDb.StringSet(keyGuidUrl.ToString(), strUrl);
    }


    /// <summary>
    /// recuperer les url dans le queue redis
    /// </summary>
    public static void getUrlQueue()
    {
        List<string> lstUrlQueue = new List<string>();
        // connexion redis
        ConnectionMultiplexer redisConn = connectionRedis();
        //get db
        IDatabase redDb = redisConn.GetDatabase();
        // get serverur 
        IServer server = redisConn.GetServer("localhost", 6379);
        foreach (var key in server.Keys().OrderBy(o => o.ToString())) // extraire tous les keys 
        {
            RedisKey k = key;
            string strKey = k.ToString(); // get keys
            string strUrl = redDb.StringGet(k); // get value url
            // supprimé les url dans qui sont dans le queue
            redDb.KeyDelete(k);
            // sauvegarder a nouveau les url avec leurs meto info
            Utils.saveMetaDataFromUrl(strUrl);
        }
    }

 
   

    /// <summary>
    /// sauvegarder les meta info url dans redis
    /// </summary>
    /// <param name="metaInfo"></param>
    public static void SaveMetaDataFromUrl(Utils metaInfo)
    {
        List<Tuple<string, string>> lstRedis = new List<Tuple<string, string>>();
        Guid keyGuidUrl = Guid.NewGuid();
        ConnectionMultiplexer redisConn = connectionRedis();
        //get your db
        IDatabase redDb = redisConn.GetDatabase();
     
        // sauvegadre list object meta info url dans redis
        redDb.HashSet(keyGuidUrl.ToString(), new HashEntry[] { new HashEntry("Url", metaInfo.Url), new HashEntry("Title" , metaInfo.Title), new HashEntry("Description", metaInfo.Description) });
        
    }


    /// <summary>
    /// recupérer les meta info url dans redis
    /// </summary>
    /// <returns></returns>
    public static List<Redis> GetMetaDataFromUrl()
    {
        List<Redis> lstObjetRedis = new List<Redis>();
        ConnectionMultiplexer redisConn = connectionRedis();
        //get your db
        IDatabase redDb = redisConn.GetDatabase();
        IServer server = redisConn.GetServer("localhost", 6379);
        int index = 0;
        foreach (var key in server.Keys().OrderBy(o => o.ToString()))
        {
            index++;
            Redis lstRedis = new Redis();
            RedisKey k = key;
            string strKey = k.ToString();
            string strUrl = redDb.HashGet(strKey , "Url");
            string strTitle = redDb.HashGet(strKey , "Title");
            string strDescription = redDb.HashGet(strKey , "Description");
            lstRedis.Index = index;
            lstRedis.strKeyUrl = strKey;
            lstRedis.Url = strUrl;
            lstRedis.Title = strTitle;
            lstRedis.Description = strDescription;

            lstObjetRedis.Add(lstRedis);
        }

        return lstObjetRedis;
        
    }

}