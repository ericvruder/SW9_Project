using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

public static class GlobalVars
{

    /// <summary>
    /// Global variable(s) - sorry guys i needed this info from an unaccesible place. - Elias
    /// </summary>
    static bool TP_lock;
    static bool _targetPractice;
    public static double canvasHeight, canvasWidth;
    //TODO dictionary for imgID lookup
    /*
    Cat: 2130837566
    Tiger: 2130837580
    Japan: 2130837578
    Sky: 2130837574
    Flower: 2130837572
    */
    public static readonly Dictionary<int,BitmapImage > imgDict =
        new Dictionary<int, BitmapImage>
        {
            { 0, new BitmapImage(new Uri("resources/ImageShape.png", UriKind.RelativeOrAbsolute)) }, //document / default
            { 2130837566, new BitmapImage(new Uri("resources/ImageShape.png", UriKind.RelativeOrAbsolute)) }, //cat
            { 2130837572, new BitmapImage(new Uri("resources/ImageShape.png", UriKind.RelativeOrAbsolute)) }, //flower
            { 2130837574, new BitmapImage(new Uri("resources/ImageShape.png", UriKind.RelativeOrAbsolute)) }, //sky
            { 2130837578, new BitmapImage(new Uri("resources/ImageShape.png", UriKind.RelativeOrAbsolute)) }, //japan
            { 2130837580, new BitmapImage(new Uri("resources/ImageShape.png", UriKind.RelativeOrAbsolute)) }  //tiger


        };

    //static Dictionary<string, string> _dict = new Dictionary<string, string>
    //{
    //{"entry", "entries"},
    //{"image", "images"},
    //{"view", "views"},
    //{"file", "files"},
    //{"result", "results"},
    //{"word", "words"},
    //{"definition", "definitions"},
    //{"item", "items"},
    //{"megabyte", "megabytes"},
    //{"game", "games"}
    //};

    public static bool isTargetPractice
    {
        get
        {
            return _targetPractice;
        }
        set
        {
            if (!TP_lock) //lazy singleton
            {
                _targetPractice = value;
                TP_lock = true;
            }
        }
    }

    public static readonly List<string> docStrings = new List<string>(new string[] { "Job advertisement", "School regulations", "Party invitation", "Fitness brochure", "Study survey", "Cantina menu", "Health consulting", "Student offers in Aalborg", "Thesis final_v2", "Keys missing, help!”, ” Chocolate cake recipe", "Used books sale", "Free beer on Friday", "Student of the year", "Professor of the year", "Annual F-club meeting minutes"  });

}