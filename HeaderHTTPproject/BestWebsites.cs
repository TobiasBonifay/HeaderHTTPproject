namespace HeaderHTTPproject;

public abstract record BestWebsites
{
    public static List<string> MyNotSensitivePageUrls()
    {
        return new List<string>
        {
            "http://google.fr",
            "http://www.reddit.com/",
            "http://twitch.fr/",
            "http://www.amazon.fr/",
            "http://docs.oracle.com/en/java/javase/19/index.html",
            "http://www.microsoft.com/",
            "http://discord.com/",
            "http://wikipedia.org"
        };
    }


    public static List<string> UselessWebsites()
    {
        return new List<string>
        {
            // "http://www.koalastothemax.com/",
            // "http://www.therestartpage.com/",
            // "https://scatter.wordpress.com/2010/05/30/the-shortest-possible-game-of-monopoly-21-seconds/",
            "https://paint.toys/",
            // "https://remoji.com/",
            "https://www.wikihow.com/Recycle-Your-Socks",
            // "https://www.rrrgggbbb.com/",
            // "https://www.patience-is-a-virtue.org/",
            // "https://www.ouaismaisbon.ch/",
            "http://alwaysjudgeabookbyitscover.com/",
            // "https://garfieldminusgarfield.net/",
            // "https://www.pointerpointer.com/",
            // "https://www.ismycomputeron.com/",
            // "https://www.nullingthevoid.com/",
            // "https://www.muchbetterthanthis.com/",
            // "https://www.yesnoif.com/",
            // "https://iamawesome.com/",
            "https://www.spaghettimonster.org/",
            // "https://www.pleaselike.com/",
            // "https://crouton.net/",
            // "https://corgiorgy.com/",
            // "https://www.wutdafuk.com/",
            // "https://unicodesnowmanforyou.com/",
            // "https://www.crossdivisions.com/",
            "https://trypap.com/"
        };
    }

    public static List<string> NewsWebsite()
    {
        return new List<string>
        {
            "https://www.nytimes.com/",
            // "https://www.bbc.com/",
            "https://edition.cnn.com/",
            "https://www.theguardian.com/",
            "https://www.latimes.com/"
        };
    }

    public static List<string> BigCompanies()
    {
        return new List<string>
        {
            "https://www.google.com/",
            "https://www.youtube.com/",
            "https://www.facebook.com/",
            "https://www.yahoo.com/",
            "https://www.amazon.com/"
        };
    }

    public static List<string> WebServers()
    {
        return new List<string>
        {
            "http://httpd.apache.org/",
            // "https://www.iis.net/",
            // "https://www.nginx.com/",
            // "https://www.lighttpd.net/",
            "https://gunicorn.org/"
        };
    }

    public static List<string> DifferentPagesOfTheSameWebsites()
    {
        return new List<string>
        {
            "https://wikipedia.org/wiki/HTTP",
            "https://wikipedia.org/wiki/HTTPS",
            "https://wikipedia.org/wiki/HTTP/2",
            "https://wikipedia.org/wiki/HTTP_cookie",
            "https://wikipedia.org/wiki/HTTP_referer",
            "https://wikipedia.org/wiki/HTTP_tunnel"
        };
    }
}