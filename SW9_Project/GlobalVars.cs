using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class GlobalVars
{

    /// <summary>
    /// Global variable(s) - sorry guys i needed this info from an unaccesible place. - Elias
    /// </summary>
    static bool TP_lock;
    static bool _targetPractice;


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