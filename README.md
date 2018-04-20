# Moteur-de-recherche
## Contexte
Go recherche est un moteur de recherche qui permet d'extraire tous les liens existants à partir d'une URL fournit au départ.
Les enregistrers au premier lieu dans queue Redis, après récupéres ces derniers urls dans la queue Redis et extraire les metas informations (Titre, description, url) 
de chaque URL puis enregistrées a nouveau dans une liste d'objets (key, Titre, description, url) dans RediS (set).


## Installation
- Pour commencer il nous faut visual studio community 2017 vous pouvez le télécharger via le lien suivant https://www.visualstudio.com/fr/downloads/?rr=https%3A%2F%2Fwww.google.fr%2F
- Création d'un nouveau projet web ASP.net vide sur visual studio 
- Installer HtmlAgilityPack 1.8.0 via le console de du gestionnaire de package visual studio on exécutant cette commande " Install-Package HtmlAgilityPack -Version 1.8.0"
https://www.nuget.org/packages/HtmlAgilityPack/ et le toturiel http://html-agility-pack.net/ 
- Instaler StackExchange.Redis via le console de du gestionnaire de package visual studio on exécutant cette commande "Install-Package StackExchange.Redis" 
https://chrisbitting.com/2014/04/14/how-to-install-redis-on-windows-and-get-started-with-c/ et le toturiel https://docs.microsoft.com/fr-fr/azure/redis-cache/cache-web-app-howto
- Insaller bootstrap 4.1.0 via le console de du gestionnaire de package visual studio on exécutant cette commande "Install-Package bootstrap -Version 4.1.0"


## Usage

- Créer le front comme dans page "Recherche.aspx"
- Créer une class Utils.cs et importer les modules suivantes:
 
using HtmlAgilityPack;  
using System;  
using System.Collections.Generic;  
using System.IO;  
using System.Linq;  
using System.Net;  
using System.Web.Configuration;  

- La méthdoe SaveUrlQueue(List<string> lstUrlGlobal) c'est fonction récursive qui permet de recupérer tous les urls exisant dans chaque url garce à HtmlAgilityPack et
les enregister dans le redis (queue)  
- La saveMetaDataFromUrl(string strUrl) permet d'extraire les meta inforations (Titre , description ....)  de chaque url via HtmlAgilityPack et les sauvegarder dans redis (set)  
 
 - Créer une class Redis.cs et importer les modules suivantes:
using StackExchange.Redis;  
using System;  
using System.Collections.Generic;  
using System.Linq;  
- La méthode ConnectionMultiplexer connectionRedis() nosu permet à se connecter à redis
- La méthode saveUrlQueue(string strUrl) génere un key GUID pour chaque url et l'enregriste dans redis on utilisant StringSet(key , url)  
- La méthode getUrlQueue() permet de recupérer tout les key value enregistré dans redis (queue) on utilissant StringGet(key)
- La méthode SaveMetaDataFromUrl(Utils metaInfo) permet d'enregister la liste d'object des meta information de chaque url dans redis (set) 
on utilissant HashSet(keyGuidUrl.ToString(), new HashEntry[] { new HashEntry("Url", metaInfo.Url), new HashEntry("Title" , metaInfo.Title), new HashEntry("Description", metaInfo.Description) });
- La méthode List<Redis> GetMetaDataFromUrl() permet de récuperer les meta informations de chaque url enregistré dans redis (set) on utilssant comme suit :  

HashGet(strKey , "Url")  
HashGet(strKey , "Title")  
HashGet(strKey , "Description")  


