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
    
}