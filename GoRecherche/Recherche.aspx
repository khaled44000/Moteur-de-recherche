<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Recherche.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="~/css/bootstrap.css" rel="stylesheet" />
    <link href="~/css/style.css" rel="stylesheet" />


    <link href="~/css/fontello.css" type="text/css" rel="stylesheet" />
    <!--[if lt IE 7]>
    <link href="css/fontello-ie7.css" type="text/css" rel="stylesheet">  
    ![endif]-->
    <link href='http://fonts.googleapis.com/css?family=Quattrocento:400,700' rel='stylesheet' type='text/css' />
    <link href='http://fonts.googleapis.com/css?family=Patua+One' rel='stylesheet' type='text/css' />
    <link href='http://fonts.googleapis.com/css?family=Open+Sans' rel='stylesheet' type='text/css' />
    <title>Go Recherche </title>
    <style type="text/css">
        body {
            padding-top: 60px; /* 60px to make the container go all the way to the bottom of the topbar */
        }
    </style>
    <link href="~/css/bootstrap-responsive.css" rel="stylesheet" />
    <!--[if lt IE 9]>
    <script src="js/html5.js"></script>
    <![endif]-->
    <script src="~/content/jquery.js"></script>
    <script src="~/content/jquery.scrollTo-1.4.2-min.js"></script>
    <script src="~/content/jquery.localscroll-1.2.7-min.js"></script>


</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div class="navbar-wrapper">
            <div class="navbar navbar-inverse navbar-fixed-top">
                <div class="navbar-inner">
                    <div class="container">
                        <a class="btn btn-navbar" data-toggle="collapse" data-target=".nav-collapse"><span class="icon-bar"></span><span class="icon-bar"></span><span class="icon-bar"></span></a>
                        <h1 class="brand"><a href="#top">Go Recherche!</a></h1>
                        <center>
                            <asp:Table runat="server">
           <asp:TableRow>
               <asp:TableCell HorizontalAlign="Center">
                                      
                  <asp:TextBox ID="txtGoRecherche" placeholder="Votre Url https://...."  runat="server" class="form-control input-lg cform-submit" Width="100%" size="40"
                    Text="https://www.google.fr/" Height="60"></asp:TextBox>
                   </asp:TableCell> 
                   <asp:TableCell HorizontalAlign="Right">
                       <input type="submit" value="Go Recherche" onserverclick="OnClick_GoRecherche" name="Recherche1" class="cform-submit" runat="server"  onclick="scroll(0, 600);" />
                    </asp:TableCell>
               </asp:TableRow>
            
      </asp:Table>
                        </center>
                    </div>
                </div>
            </div>
        </div>
        <div id="top"></div>
        <div id="headerwrap">
            <header class="clearfix">
                <h1><span>Go Recherche!
                    <br /></span>

                </h1>
                <div class="container">
                    <div class="row">
                        <div class="span12">
                            <h2>Saisir votre Url</h2>
                            <asp:TextBox ID="txtGoRecherche2" placeholder="Votre Url https://...." runat="server" class="form-control input-lg cform-submit" size="40"
                                Text="https://www.wikipedia.org/"></asp:TextBox>
                            <input type="submit" value="Go Recherche" onserverclick="OnClick_GoRecherche2" name="Recherche2" runat="server" class="cform-submit" />
                        </div>
                    </div>
                    <br />

                </div>
            </header>
        </div>
        <hr />
        <section id="portfolio" class="single-page scrollblock">
            <div class="container">
                <asp:GridView ID="gvListUrlRedis" AutoGenerateColumns="false" runat="server" CssClass="GridView"
                    DataKeyNames="Index">
                    <Columns>
                        <asp:BoundField DataField="Index" HeaderText="Index" />
                        <asp:BoundField DataField="Url" HeaderText="Url" HtmlEncode="False" DataFormatString="<a target='_blank' href='{0}'>{0}</a>" />
                        <asp:BoundField DataField="Title" HeaderText="Titre" HtmlEncode="False" />
                        <asp:BoundField DataField="Description" HeaderText="Description" HtmlEncode="False" />
                    </Columns>
                    <AlternatingRowStyle BackColor="#f0bf00" />
                </asp:GridView>
            </div>
        </section>
       

        <div class="footer-wrapper">
            <div class="container">
                <footer><small>&copy; 2018 Go Recherche</small> </footer>
            </div>
        </div>
        <script src="~/content/bootstrap.js"></script>
        <script src="~/content/jquery.prettyPhoto.js"></script>
        <script src="~/content/site.js"></script>
        <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>

    </form>
</body>
</html>
