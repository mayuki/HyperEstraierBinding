<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // アプリケーションのスタートアップで実行するコードです
        Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + System.Web.Configuration.WebConfigurationManager.AppSettings["HyperEstraierPath"], EnvironmentVariableTarget.Process);

        HyperEstraier.Database database = new HyperEstraier.Database(
            System.Web.Configuration.WebConfigurationManager.AppSettings["DatabaseName"]
        , HyperEstraier.DatabaseModes.Reader);
        
        Context.Application["HyperEstraier.DatabaseInstance"] = database;
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  アプリケーションのシャットダウンで実行するコードです
        HyperEstraier.Database database = Context.Application["HyperEstraier.DatabaseInstance"] as HyperEstraier.Database;
        if (database != null)
        {
            database.Dispose();
        }
    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // ハンドルされていないエラーが発生したときに実行するコードです

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // 新規セッションを開始したときに実行するコードです

    }

    void Session_End(object sender, EventArgs e) 
    {
        // セッションが終了したときに実行するコードです 
        // メモ: Web.config ファイル内で sessionstate モードが InProc に設定されているときのみ、
        // Session_End イベントが発生します。session モードが StateServer か、または SQLServer に 
        // 設定されている場合、イベントは発生しません。

    }
       
</script>
